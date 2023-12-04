using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;

namespace Pronia.ViewComponents
{
    public class ProductViewComponent:ViewComponent
    {
        AppDbContext _context;

        public ProductViewComponent (AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int key=1)
        {

            List<Product> products;
            switch (key) 
            {
               case 1: 
                    products= _context.Products.Include(p => p.ProductImages).OrderByDescending(p=>p.Price).Take(3).ToList();
                    break;
                case 2: 
                    products= _context.Products.Include(p => p.ProductImages).Where(x => x.Price > 10 && x.Price < 30).Take(3).ToList();
                    break;
                case 3: 
                    products= _context.Products.Include(p => p.ProductImages).OrderByDescending(p=>p.Price).Take(3).ToList();
                    break;
                    
                default: 
                    products = _context.Products.Include(p => p.ProductImages).Take(2).ToList();
                    break;
            }


            return View(products);
        }

    }
}
