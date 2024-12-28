using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WhiteLagoon.Application.Common.Interfaces;
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

            foreach (var Villa in villalist)
            {
                if (Villa.Id % 2 == 0)
                    Villa.IsAvilable = false;
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
