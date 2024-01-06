using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using System.Diagnostics;

namespace MultiShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IList<Slide> slides = await _context.Slides.Where(s => s.IsDeleted == false).ToListAsync();
            ICollection<Category> categories = await _context.Categories.Include(c => c.Products).Where(c => c.IsDeleted == false && c.Products.Count > 0).ToListAsync();
            ICollection<Product> lastProducts = await _context.Products
                .Where(p => p.IsDeleted == false)
                .OrderByDescending(p => p.Id)
                .Take(10)
                .Include(p => p.Images.Where(pi => pi.Type == ImageType.Main))
                .ToListAsync();
            ICollection<Product> featured = await _context.Products
                .Where(p => p.IsDeleted == false)
                .Take(10)
                .Include(p => p.Images.Where(pi => pi.Type == ImageType.Main))
                .ToListAsync();
            HomeVM vm = new HomeVM
            {
                Slides = slides,
                Categories = categories,
                LastProducts = lastProducts,
                FeaturedProducts = featured
                
                
            };
            return View(vm);
        }
    }
}