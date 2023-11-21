using Microsoft.AspNetCore.Mvc;
using Pronia.DAL;

namespace Pronia.Controllers
{
    public class ShopController:Controller
    {
        AppDbContext _db;

        public ShopController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Detail(int? id)
        {


            return View();
        }
    }
}
