using Microsoft.AspNetCore.Mvc;

namespace Pronia.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {


            return View();
        }
    }
}
