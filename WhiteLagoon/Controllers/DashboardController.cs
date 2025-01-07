using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Interface;
using WhiteLagoon.Application.Utilities;
using WhiteLagoon.Domain.Entites;
using WhiteLagoon.ViewModels;

namespace WhiteLagoon.Controllers
{
	public class DashboardController : Controller
	{
	
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDashboardService _dashboardServices;


        public DashboardController(IUnitOfWork unitOfWork, IDashboardService dashboardServices)
        {
            _unitOfWork = unitOfWork;
            _dashboardServices = dashboardServices;
        }
        public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> GetBookingsRedialChartData()
		{

			return Json(await _dashboardServices.GetBookingsRedialChartData());	
        }

        public async Task<IActionResult> GetUsersRedialChartData()
        {

            return Json(await _dashboardServices.GetUsersRedialChartData());
        }

        public async Task<IActionResult> GetRevenuesRedialChartData()
        {

            return Json(await _dashboardServices.GetRevenuesRedialChartData());
        }

        public async Task<IActionResult> GetCustomerBookingsPieChartData()
        {
            return Json(await _dashboardServices.GetCustomerBookingsPieChartData());    
        }

        public async Task<IActionResult> GetCustomersAndBookingLineChart()
        {

            return Json( await _dashboardServices.GetCustomersAndBookingLineChart());     
        }

        


    }
}
