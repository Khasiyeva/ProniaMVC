using Microsoft.AspNetCore.Mvc;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingController:Controller
    {
        AppDbContext _context { get; set; }
        IWebHostEnvironment _env { get; set; }


        public SettingController(AppDbContext context, IWebHostEnvironment env = null)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
              
            return View();
        }
    }
}
