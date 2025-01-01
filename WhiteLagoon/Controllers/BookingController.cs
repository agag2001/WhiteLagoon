using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Stripe.Checkout;
using System.Net;
using System.Security.Claims;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Utilities;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.Infrastructure.Repository;

namespace WhiteLagoon.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
          [Authorize]
        public IActionResult Index()
        {
            return View();  
        }
        [Authorize]
       public IActionResult FinalizeBooking(int nights , DateOnly checkInDate, int villaId)
        {
            // get the claims of the current login user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            // get the userId
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            AppUser user = _unitOfWork.User.Get(u=>u.Id == userId);  

            Booking booking = new()
            {
                Nights = nights ,   
                CheckInDate = checkInDate ,
                CheckOutDate= checkInDate.AddDays(nights),
                VillaId = villaId ,
                Villa = _unitOfWork.Villa.Get(v=>v.Id==villaId,includeProperties: "VillaAmenity")
                ,UserId = user.Id,
                Name = user.Name,
                Phone = user.PhoneNumber,
                Email = user.Email
                
            };
            booking.TotalCost = booking.Villa.Price * nights;
            return View(booking);
        }


        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking bookingModel)
        {
            var Villa = _unitOfWork.Villa.Get(v => v.Id == bookingModel.VillaId, includeProperties: "VillaAmenity");
            var totalCost = Villa.Price * bookingModel.Nights;
            bookingModel.Status  =SD.StatusPending;
            bookingModel.BookingDate= DateTime.Now;


            // check availablity before we submit this book
            // this scenario is when there are alot of booking and if you late in the booking may
            // some one take the final book so that the villa is not available
            // ********************************************************************

            // get only the approved bookings and the checkin (Status) these books may overlapped with my Booking
            var bookings = _unitOfWork.Booking.GetAll(b => b.Status == SD.StatusApproved || b.Status == SD.StatusCheckIn).ToList();

            var villaNumbers = _unitOfWork.VillaNumber.GetAll().ToList();


             var availabelRooms = SD.VillaRoomAvailable_Count(Villa.Id, bookings, villaNumbers, bookingModel.Nights, bookingModel.CheckInDate);
             if(availabelRooms == 0)
             {
                TempData["error"] = "Sorry, this villa has been Sold Out";
                return RedirectToAction(nameof(FinalizeBooking), new
                {
                    nights = bookingModel.Nights,
                    villaId = Villa.Id,
                    checkInDate = bookingModel.CheckInDate

                });

             }
 
         
                _unitOfWork.Booking.Add(bookingModel);
                _unitOfWork.Save();
           

           


            //adding our custom domain
            var domain = Request.Scheme + "://" + Request.Host.Value +"/";

            // set the product info and the url if the success or cancel the payment
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"Booking/BookingConfirmation?bookingId={bookingModel.Id}",
                CancelUrl = domain + $"Booking/FinalizeBooking?nights={bookingModel.Nights}&checkInDate={bookingModel.CheckInDate}&villaId={bookingModel.VillaId}"

            };
            // if you have more than one catiogry you will use the foreach method
            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions()
                {
                    Currency = "usd",
                    UnitAmount = (long)bookingModel.TotalCost * 100,
                    ProductData = new SessionLineItemPriceDataProductDataOptions()
                    {
                        Name = bookingModel.Name,
                        Description = "You are Welcome"
                    }


                },
                Quantity = 1
            });

            // open the session between you and the API
                      
            var service = new SessionService();
            var session = service.Create(options);

            // save the session id in the database
            _unitOfWork.Booking.UpdateStripePaymentId(bookingModel.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            // get the return Url of the Payment page and redirect admin to this page
            Response.Headers.Append("Location", session.Url);
            return new StatusCodeResult(303);
            
         }
        public IActionResult BookingConfirmation(int bookingId)
        {
            // get booking form Db
            var bookingDB =  _unitOfWork.Booking.Get(b=>b.Id == bookingId); 
            if(bookingDB is not null)
            {
                // get the session info and then check the status of Payment 
                // if successful then you can update the Booking Status and save the PaymentIntentId
                var service = new SessionService();
                Session session = service.Get(bookingDB.StripSessionId);
                if(session.PaymentStatus== "paid")
                {
                    _unitOfWork.Booking.UpdateStauts(bookingDB.Id, SD.StatusApproved,0);
                    _unitOfWork.Booking.UpdateStripePaymentId(bookingDB.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save(); 
                }
            }
            return View(bookingId);
        }
        public IActionResult BookingDetails(int bookingId)
        {
            var booking  = _unitOfWork.Booking.Get(b=>b.Id == bookingId, includeProperties:"Villa,User");
            if (booking is null)
            {
                return NotFound();
            }
            if(booking.Status == SD.StatusApproved && booking.VillaNumber == 0)
            {
                booking.VillaNumbers = GetAvailableRoomsInVill(booking.VillaId);
            }

          
           
            return View(booking);
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]

        public IActionResult CheckIn(Booking bookingModel)
        {
            _unitOfWork.Booking.UpdateStauts(bookingModel.Id, SD.StatusCheckIn, bookingModel.VillaNumber);
            _unitOfWork.Save();
            TempData["success"] = "Check In completed Successfully";

            return RedirectToAction(nameof(BookingDetails),new {bookingId= bookingModel.Id});

        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckOut(Booking bookingModel)
        {
            _unitOfWork.Booking.UpdateStauts(bookingModel.Id, SD.StatusCompleted, bookingModel.VillaNumber);
            _unitOfWork.Save();
            TempData["success"] = "Check out completed Successfully";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = bookingModel.Id });



        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Cancel(Booking bookingModel)
        {
            _unitOfWork.Booking.UpdateStauts(bookingModel.Id, SD.StatusCancelled, 0);
            _unitOfWork.Save();
            TempData["error"] = " Booking Cancelled Successfully";

            return RedirectToAction(nameof(BookingDetails), new { bookingId = bookingModel.Id });

        }
        private List<VillaNumber> GetAvailableRoomsInVill(int villaId)
        {
            List<VillaNumber> availableRooms = new();
            var roomsInVilla = _unitOfWork.VillaNumber.GetAll(x=>x.Villa_id == villaId);    
            // get the booking that have status of >> checkIn and the same of our villa
            // and then select this RoomNumber in this booking
            var bookedRooms =  _unitOfWork.Booking.GetAll(b=>b.Status == SD.StatusCheckIn && b.VillaId == villaId)
                                                    .Select(x =>x.VillaNumber);
            foreach(var room in roomsInVilla)
            {// if the roomnumber is not in the booked rooms
                if (!bookedRooms.Contains(room.Villa_Number))
                    availableRooms.Add(room);
            }
            return availableRooms;  
        }
        #region API Calls
        [HttpGet]
        [Authorize]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Booking> bookings;
            if (User.IsInRole(SD.Role_Admin)) // if user is admin now retrive all the booking
            {
                 bookings =  _unitOfWork.Booking.GetAll(includeProperties:"Villa,User");
            }
            else
            {
                // the user is customer >> get it's bookings
                var claimsIdentity =  (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                bookings = _unitOfWork.Booking.GetAll(u => u.UserId == userId, includeProperties: "User,Villa"); 
            }
            if (!string.IsNullOrEmpty(status))
            {
                // applying the filter on the booking accoriding to the status 
                bookings = bookings.Where(b => b.Status.ToLower() == status.ToLower());
            }
            return Json(new {data=bookings});
        }
        #endregion
    }
}
