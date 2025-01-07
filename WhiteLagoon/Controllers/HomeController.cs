using Microsoft.AspNetCore.Mvc;
using Syncfusion.Presentation;
using System.Diagnostics;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Utilities;
using WhiteLagoon.Models;
using WhiteLagoon.ViewModels;

namespace WhiteLagoon.Controllers
{
    public class HomeController : Controller
    {
		private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll(includeProperties:"VillaAmenity"),
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),
                

            };
            return View(homeVM);
        }


        [HttpPost]
        public IActionResult GetVillasByDate(int nights , DateOnly checkInDate)
        {
            var villalist = _unitOfWork.Villa.GetAll(includeProperties:"VillaAmenity");
            // get only the approved bookings and the checkin (Status) these books may overlapped with my Booking
            var bookings = _unitOfWork.Booking.GetAll(b => b.Status == SD.StatusApproved || b.Status == SD.StatusCheckIn).ToList();

            var villaNumbers = _unitOfWork.VillaNumber.GetAll().ToList();
            

            foreach (var Villa in villalist)
            {
                var availabelRooms = SD.VillaRoomAvailable_Count(Villa.Id, bookings, villaNumbers, nights, checkInDate);
                Villa.IsAvilable = availabelRooms > 0 ? true : false;   
                
               
            }
            HomeVM homeVM = new()
            {
                VillaList = villalist,
                Nights = nights,
                CheckInDate = checkInDate

            };
            return PartialView("_VillaListPartial",homeVM);

        }
        public IActionResult GeneratePPTExport(int id)
        {
            var villa = _unitOfWork.Villa.Get(x => x.Id == id,includeProperties:"VillaAmenity");
            if (villa is null)
            {
                RedirectToAction(nameof(Error));
            }
            var path = _webHostEnvironment.WebRootPath;
            var filePath = path + @"/Exports/ExportVillaDetails.pptx";

            using IPresentation presentation = Presentation.Open(filePath);

            ISlide slide  = presentation.Slides[0];

            IShape? shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtVillaName") as IShape; 

            if(shape is not null)
            {
                shape.TextBody.Text = villa.Name;
            }

            shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtVillaDescription") as IShape;

            if (shape is not null)
            {
                shape.TextBody.Text = villa.Description;
            }


             shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtOccupancy") as IShape;

            if (shape is not null)
            {
                shape.TextBody.Text = "Max Occupancy : " + villa.Occupancy.ToString();
            }


            shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtVillaSize") as IShape;

            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("Villa Size: {0} sqft", villa.Sqft);
            }


            shape = slide.Shapes.FirstOrDefault(x => x.ShapeName == "txtPricePerNight") as IShape;

            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("USD {0}/night", villa.Price.ToString("C"));
            }



            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtVillaAmenitiesHeading") as IShape;
            if (shape is not null)
            {
                List<string> listItems = villa.VillaAmenity.Select(x => x.Name).ToList();

                shape.TextBody.Text = "";

                foreach (var item in listItems)
                {
                    IParagraph paragraph = shape.TextBody.AddParagraph();
                    ITextPart textPart = paragraph.AddTextPart(item);

                    paragraph.ListFormat.Type = ListType.Bulleted;
                    paragraph.ListFormat.BulletCharacter = '\u2022';
                    textPart.Font.FontName = "system-ui";
                    textPart.Font.FontSize = 18;
                    textPart.Font.Color = ColorObject.FromArgb(144, 148, 152);

                }

            }
            shape =  slide.Shapes.FirstOrDefault(x=>x.ShapeName== "imgVilla") as IShape;
            if (shape is not null)
            {
                byte[] imageData;
                string imageUrl;
                try
                {
                    imageUrl = string.Format("{0}{1}", path, villa.ImageUrl);
                    imageData = System.IO.File.ReadAllBytes(imageUrl);
                }
                catch
                {
                    imageUrl = string.Format("{0}{1}", path, "/Images/placeholder.png");
                    imageData = System.IO.File.ReadAllBytes(imageUrl);
                }
                // remove old image
                slide.Shapes.Remove(shape); 
                using MemoryStream ImageStream =  new MemoryStream(imageData);
                IPicture picture = slide.Pictures.AddPicture(ImageStream, 60, 120, 300, 200);
            }


            MemoryStream memoryStream = new MemoryStream(); 
            presentation.Save(memoryStream);    
            memoryStream.Position  = 0;
            return File(memoryStream, "application/pptx", "Villa.pptx");

        }


        public IActionResult Privacy()
        {
            return View();
        }

       
        public IActionResult Error()
        {
            return View();
        }
    }
}
