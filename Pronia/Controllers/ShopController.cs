using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;

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
            Product product = _db.Products.Include(p => p.Category)
                .Include(p=>p.ProductImages)
                .Include(p=>p.ProductTags)
                .ThenInclude(pt=>pt.Tag)
                .FirstOrDefault(product=>product.Id == id);

            return View(product);
        }
    }
}
