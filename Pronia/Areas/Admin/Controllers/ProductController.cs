using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Areas.Admin.ViewModels.Product;
using Pronia.DAL;
using Pronia.Helpers;
using Pronia.Models;
using static Pronia.Areas.Admin.ViewModels.Product.UpdateProductVM;

namespace Pronia.Areas.Admin.Controllers
{
        [Area("Admin")]
    public class ProductController : Controller
    {
        AppDbContext _context {  get; set; }
        IWebHostEnvironment _env { get; set; }


        public ProductController(AppDbContext context, IWebHostEnvironment env = null)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> product = await _context.Products.Include(p => p.Category)
                .Include(p=>p.ProductTags).ThenInclude(pt=>pt.Tag).Include(p=>p.ProductImages).ToListAsync();
            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories =await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync(); 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM createProductVM)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View("Error");
            }

            bool resultCategory = await _context.Categories.AnyAsync(c=> c.Id==createProductVM.CategoryId);
            if (!resultCategory)
            {
                ModelState.AddModelError("CategoryId", "There is no such category");
                return View();
            }
            Product product = new Product() 
            {
               Name = createProductVM.Name,
               Price= createProductVM.Price,
               Description = createProductVM.Description,
               SKU= createProductVM.SKU,
               CategoryId= createProductVM.CategoryId,
               ProductImages=new List<ProductImage>()
            };

            if (createProductVM.TagIds != null)
            {
                foreach (var tagId in createProductVM.TagIds)
                {
                    bool resultTag = await _context.Tags.AnyAsync(c => c.Id == tagId);
                    if (!resultTag)
                    {
                        ModelState.AddModelError("TagIds", "There is no such tag");
                        return View();
                    }

                    ProductTag productTag = new ProductTag()
                    {
                        Product = product,
                        TagId = tagId
                    };

                    _context.ProductTags.Add(productTag);


                }
            }

            if (!createProductVM.MainPhoto.CheckContent("image/"))
            {
                ModelState.AddModelError("MainPhoto", "Enter the correct format");
                return View();
            }

            if (!createProductVM.MainPhoto.CheckLenght(3000))
            {
                ModelState.AddModelError("MainPhoto", "The maximum can be 3mb");
                return View();
            }

            if (!createProductVM.HoverPhoto.CheckContent("image/"))
            {
                ModelState.AddModelError("HoverPhoto", "Enter the correct format");
                return View();
            }

            if (!createProductVM.HoverPhoto.CheckLenght(3000))
            {
                ModelState.AddModelError("HoverPhoto", "The maximum can be 3mb");
                return View();
            }

            ProductImage mainImage = new ProductImage()
            {
                IsPrime = true,
                ImgUrl = createProductVM.MainPhoto.Upload(_env.WebRootPath, @"\Upload\Product\"),
                Product = product
            };

            ProductImage hoverImage = new ProductImage()
            {
                IsPrime = false,
                ImgUrl = createProductVM.HoverPhoto.Upload(_env.WebRootPath, @"\Upload\Product\"),
                Product = product
            };

            TempData["Error"] = "";
            product.ProductImages.Add(mainImage);
            product.ProductImages.Add(hoverImage);
            if (createProductVM.Photos != null)
            {
                foreach (var item in createProductVM.Photos)
                {
                    if (!item.CheckContent("image/"))
                    {
                        TempData["Error"] += $"{item.FileName} the type is not correct \t";
                        continue;
                    }

                    if (!item.CheckLenght(3000))
                    {
                        TempData["Error"] += $"{item.FileName} more than 3mb \t";
                        continue;
                    }

                    ProductImage photo = new ProductImage()
                    {
                        IsPrime = null,
                        ImgUrl = item.Upload(_env.WebRootPath, @"\Upload\Product\"),
                        Product = product
                    };
                    product.ProductImages.Add(photo);
                }
            }


            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Update(int id)
        {

            Product product = await _context.Products.Include(p=>p.Category)
                .Include(p=>p.ProductTags)
                .ThenInclude(p=>p.Tag)
                .Include(p=>p.ProductImages)
                .Where(p => p.Id == id).FirstOrDefaultAsync();
            if(product is null)
            {
                return View("Error");
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();
            UpdateProductVM updateProductVM = new UpdateProductVM()
            {
                Id = id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                SKU= product.SKU,
                CategoryId= product.CategoryId,
                TagIds=new List<int>(),
                productImages=new List<ProductImagesVm>()
            };
            foreach(var item in product.ProductTags)
            {
                updateProductVM.TagIds.Add(item.TagId);
            }

            foreach(var item in product.ProductImages)
            {
                ProductImagesVm productImages = new ProductImagesVm()
                {
                    IsPrime = item.IsPrime,
                    ImgUrl = item.ImgUrl,
                    Id = item.Id
                };

                updateProductVM.productImages.Add(productImages);
            }

            return View(updateProductVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductVM updateProductVM)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }
            Product existProduct = await _context.Products.Include(p=>p.ProductTags).ThenInclude(pt=>pt.Tag).Include(p=>p.ProductImages).Where(p => p.Id == updateProductVM.Id).FirstOrDefaultAsync();
            if (existProduct is null)
            {
                return View("Error");
            }
            bool resultCategory = await _context.Categories.AnyAsync(c => c.Id == updateProductVM.CategoryId);
            if (!resultCategory)
            {
                ModelState.AddModelError("CategoryId", "There is no such category");
                return View();
            }
            existProduct.Name=updateProductVM.Name;
            existProduct.Price=updateProductVM.Price;
            existProduct.SKU=updateProductVM.SKU;
            existProduct.Description=updateProductVM.Description;
            existProduct.CategoryId=updateProductVM.CategoryId;



            if (updateProductVM.TagIds != null)
            {
                foreach (int tagId in updateProductVM.TagIds)
                {
                    bool resultTag = await _context.Tags.AnyAsync(c => c.Id == tagId);
                    if (!resultTag)
                    {
                        ModelState.AddModelError("TagIds", "There is no such tag");
                        return View();
                    }
                }

                List<ProductTag> removeTags = existProduct.ProductTags.Where(pt => updateProductVM.TagIds.Contains(pt.TagId)).ToList();

                _context.ProductTags.RemoveRange(removeTags);


                List<int> createTags;

                if (existProduct.ProductTags != null)
                {
                    createTags = updateProductVM.TagIds.Where(ti => existProduct.ProductTags.Exists(pt => pt.TagId == ti)).ToList();
                }



                else
                {
                    createTags = updateProductVM.TagIds.ToList();

                }

                foreach (var tagId in createTags)
                {
                    ProductTag productTag = new ProductTag()
                    {
                        TagId = tagId,
                        ProductId = existProduct.Id
                    };


                    await _context.ProductTags.AddAsync(productTag);

                }

            }
            else
            {
                var productTagList=_context.ProductTags.Where(pt=>pt.ProductId == existProduct.Id).ToList();
                _context.ProductTags.RemoveRange(productTagList);
            }
            TempData["Error"] = "";

            if(updateProductVM.MainPhoto!= null)
            {
                if (!updateProductVM.MainPhoto.CheckContent("image/"))
                {
                    ModelState.AddModelError("MainPhoto", "Enter the correct format");
                    return View();
                }

                if (!updateProductVM.MainPhoto.CheckLenght(3000))
                {
                    ModelState.AddModelError("MainPhoto", "The maximum can be 3mb");
                    return View();
                }

                ProductImage newMainImages = new ProductImage()
                {
                    IsPrime = true,
                    ProductId = existProduct.Id,
                    ImgUrl = updateProductVM.MainPhoto.Upload(_env.WebRootPath, @"\Upload\Product\")
                    
                };

                var oldMainPhoto = existProduct.ProductImages?.FirstOrDefault(p => p.IsPrime == true);
                existProduct.ProductImages?.Remove(oldMainPhoto);
                existProduct.ProductImages.Add(newMainImages);
            }

            if (updateProductVM.HoverPhoto != null)
            {
                if (!updateProductVM.HoverPhoto.CheckContent("image/"))
                {
                    ModelState.AddModelError("HoverPhoto", "Enter the correct format");
                    return View();
                }

                if (!updateProductVM.HoverPhoto.CheckLenght(3000))
                {
                    ModelState.AddModelError("HoverPhoto", "The maximum can be 3mb");
                    return View();
                }

                ProductImage newHoverImages = new ProductImage()
                {
                    IsPrime = false,
                    ProductId = existProduct.Id,
                    ImgUrl = updateProductVM.HoverPhoto.Upload(_env.WebRootPath, @"\Upload\Product\")

                };

                var oldHoverPhoto = existProduct.ProductImages?.FirstOrDefault(p => p.IsPrime == false);
                existProduct.ProductImages?.Remove(oldHoverPhoto);
                existProduct.ProductImages.Add(newHoverImages);
            }

            if(updateProductVM.Photos != null)

            {
                foreach (var item in updateProductVM.Photos)
                {
                    if (!item.CheckContent("image/"))
                    {
                        TempData["Error"] += $"{item.FileName} the type is not correct \t";
                        continue;
                    }

                    if (!item.CheckLenght(3000))
                    {
                        TempData["Error"] += $"{item.FileName} more than 3mb \t";
                        continue;
                    }

                    ProductImage newPhoto = new ProductImage()
                    {
                        IsPrime = null,
                        ImgUrl = item.Upload(_env.WebRootPath, @"\Upload\Product\"),
                        Product = existProduct
                    };
                    existProduct.ProductImages.Add(newPhoto);
                }
            }

            if(updateProductVM.ImageIds != null)
            {
            var removeListImage=existProduct.ProductImages?.Where(p=>!updateProductVM.ImageIds.Contains(p.Id)&& p.IsPrime==null).ToList();
            foreach (var item in removeListImage)
            {
                existProduct.ProductImages.Remove(item);
                item.ImgUrl.DeleteFile(_env.WebRootPath,@"\Upload\Product\");
            }

            }
            else
            {
                existProduct.ProductImages.RemoveAll(p=>p.IsPrime==null);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id==id);
            if(product is null)
            {
                return View("Error");
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
