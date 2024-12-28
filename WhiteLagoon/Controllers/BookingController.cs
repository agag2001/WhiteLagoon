using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Stripe.Checkout;
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
                    _unitOfWork.Booking.UpdateStauts(bookingDB.Id, SD.StatusApproved);
                    _unitOfWork.Booking.UpdateStripePaymentId(bookingDB.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save(); 
                }
            }
            return View(bookingId);
        }
    }
}
