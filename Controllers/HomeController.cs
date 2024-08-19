using Microsoft.AspNetCore.Mvc;
using Nice_Admin_1.Models;
using System.Diagnostics;

namespace Nice_Admin_1.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}
        public IActionResult FormEmployee()
        {
            return View();
        }
        public IActionResult FormDepartment()
        {
            return View();
        }
        public IActionResult FormProject()
        {
            return View();
        }
        public IActionResult FormEmployeeProject()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
