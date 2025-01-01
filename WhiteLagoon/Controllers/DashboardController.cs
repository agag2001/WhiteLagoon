using Microsoft.AspNetCore.Mvc;

namespace WhiteLagoon.Controllers
{
	public class DashboardController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
