
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Stripe.Checkout;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using System.Drawing;
using System.Net;
using System.Security.Claims;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Application.Utilities;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.Infrastructure.Repository;

namespace WhiteLagoon.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly IVillaService _villaService;
        private readonly IVillaNumberService _villaNumberService;
        private readonly UserManager<AppUser> _userManager;
        public BookingController(IWebHostEnvironment webHostEnvironment, IBookingService bookingService, IVillaService villaService, IVillaNumberService villaNumberService, UserManager<AppUser> userManager)
        {
            _webHostEnvironment = webHostEnvironment;
            _bookingService = bookingService;
            _villaService = villaService;
            _villaNumberService = villaNumberService;
            _userManager = userManager;
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

            AppUser user = _userManager.FindByIdAsync(userId).GetAwaiter().GetResult();  

            Booking booking = new()
            {
                Nights = nights ,   
                CheckInDate = checkInDate ,
                CheckOutDate= checkInDate.AddDays(nights),
                VillaId = villaId ,
                Villa = _villaService.GetVillaById(villaId,IncludeProperties: "VillaAmenity")
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
            var Villa = _villaService.GetVillaById( bookingModel.VillaId, IncludeProperties: "VillaAmenity");
            var totalCost = Villa.Price * bookingModel.Nights;
            bookingModel.Status  =SD.StatusPending;
            bookingModel.BookingDate= DateTime.Now;


            // check availablity before we submit this book
            // this scenario is when there are alot of booking and if you late in the booking may
            // some one take the final book so that the villa is not available
            // ********************************************************************
            // get only the approved bookings and the checkin (Status) these books may overlapped with my Booking
           
             if(!_villaService.IsVillaRoomAvailable(bookingModel.VillaId,bookingModel.Nights,bookingModel.CheckInDate))
             {
                TempData["error"] = "Sorry, this villa has been Sold Out";
                return RedirectToAction(nameof(FinalizeBooking), new
                {
                    nights = bookingModel.Nights,
                    villaId = Villa.Id,
                    checkInDate = bookingModel.CheckInDate

                });

             }
 
             _bookingService.CreateBooking(bookingModel);       
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
            _bookingService.UpdateStripePaymentId(bookingModel.Id, session.Id, session.PaymentIntentId);
           
            // get the return Url of the Payment page and redirect admin to this page
            Response.Headers.Append("Location", session.Url);
            return new StatusCodeResult(303);
            
         }
        public IActionResult BookingConfirmation(int bookingId)
        {
            // get booking form Db
            var bookingDB =  _bookingService.GetBookingById(bookingId); 
            if(bookingDB is not null)
            {
                // get the session info and then check the status of Payment 
                // if successful then you can update the Booking Status and save the PaymentIntentId
                var service = new SessionService();
                Session session = service.Get(bookingDB.StripSessionId);
                if(session.PaymentStatus== "paid")
                {
                   _bookingService.UpdateStauts(bookingDB.Id, SD.StatusApproved,0);
                    _bookingService.UpdateStripePaymentId(bookingDB.Id, session.Id, session.PaymentIntentId);
                  
                }
            }
            return View(bookingId);
        }
        public IActionResult BookingDetails(int bookingId)
        {
            var booking  =_bookingService.GetBookingById(bookingId);
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
           _bookingService.UpdateStauts(bookingModel.Id, SD.StatusCheckIn, bookingModel.VillaNumber);
       
            TempData["success"] = "Check In completed Successfully";

            return RedirectToAction(nameof(BookingDetails),new {bookingId= bookingModel.Id});

        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckOut(Booking bookingModel)
        {
            _bookingService.UpdateStauts(bookingModel.Id, SD.StatusCompleted, bookingModel.VillaNumber);
            TempData["success"] = "Check out completed Successfully";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = bookingModel.Id });



        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Cancel(Booking bookingModel)
        {
            _bookingService.UpdateStauts(bookingModel.Id, SD.StatusCancelled, 0);
            TempData["error"] = " Booking Cancelled Successfully";

            return RedirectToAction(nameof(BookingDetails), new { bookingId = bookingModel.Id });

        }

        public IActionResult GenerateInvoice(int id,string downloadType)
        {
            var path = _webHostEnvironment.WebRootPath;
            WordDocument document = new WordDocument();
            var dataPath = path + @"/Exports/BookingDetails.docx";

            // load template
            FileStream stream =  new FileStream(dataPath,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
            document.Open(stream,FormatType.Automatic); 

            // update template
            var booking  =  _bookingService.GetBookingById(id);

            TextSelection textSelection = document.Find("xx_customer_name", false, true);
            WTextRange textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.Name;

            textSelection = document.Find("xx_customer_phone", false, true);
            textRange =  textSelection.GetAsOneRange(); 
            textRange.Text  = booking.Phone;


            textSelection = document.Find("xx_customer_email", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.Email;


            textSelection = document.Find("xx_payment_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.PaymentDate.ToShortDateString();


            textSelection = document.Find("xx_checkin_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.CheckInDate.ToShortDateString();


            textSelection = document.Find("xx_checkout_date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.CheckOutDate.ToShortDateString();


            textSelection = document.Find("xx_booking_total", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = booking.TotalCost.ToString("c");


            textSelection = document.Find("xx_booking_Number", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = "Booking ID - " +booking.Id.ToString();


            textSelection = document.Find("xx_BOOKING_Date", false, true);
            textRange = textSelection.GetAsOneRange();
            textRange.Text = "Booking Date - " + booking.BookingDate.ToShortDateString();

            WTable table = new(document);

            table.TableFormat.Borders.LineWidth = 1f;
            table.TableFormat.Borders.Color =Syncfusion.Drawing.Color.Black;
            table.TableFormat.Paddings.Top = 7f;
            table.TableFormat.Paddings.Bottom = 7f;
            table.TableFormat.Borders.Horizontal.LineWidth = 1f;

            int rows = booking.VillaNumber > 0 ? 3 : 2;
            table.ResetCells(rows, 4);

            WTableRow row0 =  table.Rows[0];
            row0.Cells[0].AddParagraph().AppendText("NIGHTS");
            row0.Cells[0].Width = 80;
            row0.Cells[1].AddParagraph().AppendText("VILLA");
            row0.Cells[1].Width = 220;

            row0.Cells[2].AddParagraph().AppendText("PRICE PER NIGHT");
            row0.Cells[3].AddParagraph().AppendText("TOTAL COST");
            row0.Cells[3].Width = 80;

         
            WTableRow row1 = table.Rows[1];
            row1.Cells[0].AddParagraph().AppendText(booking.Nights.ToString());
            row1.Cells[0].Width = 80;
            row1.Cells[1].AddParagraph().AppendText(booking.Villa.Name.ToString());
            row1.Cells[1].Width = 220;

            row1.Cells[2].AddParagraph().AppendText((booking.TotalCost/ booking.Nights).ToString());
            row1.Cells[3].AddParagraph().AppendText(booking.TotalCost.ToString() );
            row1.Cells[3].Width = 80;

            if (booking.VillaNumber > 0)
            {
                WTableRow row2 = table.Rows[2];

                row2.Cells[0].Width = 80;
                row2.Cells[1].AddParagraph().AppendText("Villa Number - " + booking.VillaNumber.ToString());
                row2.Cells[1].Width = 220;
                row2.Cells[3].Width = 80;
            }

            WTableStyle tableStyle = document.AddTableStyle("CustomStyle") as WTableStyle;
            tableStyle.TableProperties.RowStripe = 1;
            tableStyle.TableProperties.ColumnStripe = 2;
            tableStyle.TableProperties.Paddings.Top = 2;
            tableStyle.TableProperties.Paddings.Bottom = 1;
            tableStyle.TableProperties.Paddings.Left = 5.4f;
            tableStyle.TableProperties.Paddings.Right = 5.4f;
             
            ConditionalFormattingStyle firstRowStyle = tableStyle.ConditionalFormattingStyles.Add(ConditionalFormattingType.FirstRow);
            firstRowStyle.CharacterFormat.Bold = true;
            firstRowStyle.CharacterFormat.TextColor = Syncfusion.Drawing.Color.FromArgb(255, 255, 255, 255);
            firstRowStyle.CellProperties.BackColor = Syncfusion.Drawing.Color.Black;

            table.ApplyStyle("CustomStyle");


            TextBodyPart bodyPart = new TextBodyPart(document);
            bodyPart.BodyItems.Add(table);

            document.Replace("<ADDTABLEHERE>", bodyPart, false, false);

            using DocIORenderer renderer = new();
            MemoryStream memoryStream = new();
            if (downloadType == "word")
            {

                document.Save(memoryStream, FormatType.Docx);
                memoryStream.Position = 0;

                return File(memoryStream, "application/docx", "BookingDetails.docx");
            }
            else
            {
                PdfDocument pdfDocument = renderer.ConvertToPDF(document);
                pdfDocument.Save(memoryStream);
                memoryStream.Position = 0;

                return File(memoryStream, "application/pdf", "BookingDetails.pdf");
            }

        }
        private List<VillaNumber> GetAvailableRoomsInVill(int villaId)
        {
            List<VillaNumber> availableRooms = new();
            var roomsInVilla = _villaNumberService.GetAllVillaNumbers().Where(x => x.Villa_id == villaId);    
            // get the booking that have status of >> checkIn and the same of our villa
            // and then select this RoomNumber in this booking
            var bookedRooms = _bookingService.GetBookedRoomInVilla(villaId);    
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
            string userId = "";
            if(string.IsNullOrEmpty(status))
            {
                status = "";
            }
            if (!User.IsInRole(SD.Role_Admin)) 
            
           
            {
                // the user is customer >> get it's bookings
                var claimsIdentity =  (ClaimsIdentity)User.Identity;
                 userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            }

            bookings = _bookingService.GetAllBooking(userId,status);// if user is admin now retrive all the booking because the status is null 

            return Json(new {data=bookings});
        }
        #endregion
    }
}
