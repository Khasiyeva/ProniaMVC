﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.ViewModels;

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
                .Where(p => p.IsDeleted == false)
                .Include(p=>p.ProductImages)
                .Include(p=>p.ProductTags)
                .ThenInclude(pt=>pt.Tag)
                .FirstOrDefault(product=>product.Id == id);
            if(product == null)
            {
                return NotFound();
            }

            DetailVM detailVM = new DetailVM()
            {
                Product = product,
                Products = _db.Products.Where(p => p.IsDeleted == false).Include(p => p.ProductImages).Include(p => p.Category).Where(p=>p.CategoryId==product.CategoryId&&p.Id!=product.Id).ToList()
            };


            return View(detailVM);
        }
    }
}
