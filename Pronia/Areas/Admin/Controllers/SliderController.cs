using Microsoft.AspNetCore.Mvc;
using Pronia.DAL;
using Pronia.Helpers;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SliderController : Controller
    {
        AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SliderController(AppDbContext context,IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index()
        {
            List<Slider> sliderList = _context.Sliders.ToList();
            return View(sliderList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (!slider.ImageFile.ContentType.Contains("image"))
            {
                ModelState.AddModelError("ImageFile", "You can only upload images");
                return View();
            }
            

            slider.ImgUrl = slider.ImageFile.Upload(_environment.WebRootPath, @"\Upload\SliderImage\");

            if (!ModelState.IsValid)
            {
                return View();
            }

            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var slider = _context.Sliders.FirstOrDefault(s => s.Id == id);

            _context.Sliders.Remove(slider);
            _context.SaveChanges();
            FileManager.DeleteFile(slider.ImgUrl, _environment.WebRootPath, @"\Upload\SliderImage\");
            return RedirectToAction("Index");
        }


        public IActionResult Update (int id)
        {
            Slider slider = _context.Sliders.Find(id);
            return View(slider);
        }

        [HttpPost]
        public IActionResult Update(Slider newSlider)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Slider oldSlider = _context.Sliders.Find(newSlider.Id);
            oldSlider.Title = newSlider.Title;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
