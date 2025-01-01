using Microsoft.AspNetCore.Mvc;
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

		public HomeController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
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
