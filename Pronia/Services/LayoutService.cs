using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pronia.DAL;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Services
{
    public class LayoutService
    {
        AppDbContext _context;
        IHttpContextAccessor _http;
        public LayoutService(AppDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }

        public async Task<Dictionary<string,string>> GetSetting()
        {
            Dictionary<string, string> setting = _context.Settings.ToDictionary(s=>s.Key,s=>s.Value);
            return setting;
        }

        public async  Task<List<BasketItemVM>> GetBasket()
        {
            
            var cookieJson = _http.HttpContext.Request.Cookies["Basket"];
            List<BasketItemVM> basketItems = new List<BasketItemVM>();
            if (cookieJson!=null)
            {
                var cookieItems = JsonConvert.DeserializeObject<List<CookieItemVM>>(cookieJson);

                bool countCheck = false;
                List<CookieItemVM> deletedCookie = new List<CookieItemVM>();
                foreach (var item in cookieItems)
                {
                    Product product = await _context.Products.Include(p => p.ProductImages.Where(p => p.IsPrime == true)).FirstOrDefaultAsync(p => p.Id == item.Id);
                    if (product == null)
                    {
                        deletedCookie.Add(item);
                        continue;
                    }

                    basketItems.Add(new BasketItemVM()
                    {
                        Id = item.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Count = item.Count,
                        ImgUrl = product.ProductImages.FirstOrDefault().ImgUrl
                    });
                }
                if (deletedCookie.Count > 0)
                {
                    foreach (var delete in deletedCookie)
                    {
                        cookieItems.Remove(delete);
                    }
                    _http.HttpContext.Response.Cookies.Append("Basket", JsonConvert.SerializeObject(cookieItems));
                }



            }
            return basketItems;

        }
    }
}
