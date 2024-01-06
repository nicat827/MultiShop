using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Utilities.Extencions;

namespace MultiShop.Controllers
{
    public class ShopController:Controller
    {
        private const int LIMIT = 5;
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int sortBy=1, int? catId=null, int page=1)
        {
            IEnumerable<Product> products = new List<Product>();
            int count;
            int totalPages;
            if (catId is not  null)
            {
                if (catId <= 0) return BadRequest();
                count = await _context.Products.Where(p => p.CategoryId == catId && p.IsDeleted == false).CountAsync();
                totalPages = (int)Math.Ceiling((double)count / LIMIT);
                switch (sortBy)
                {
                    case 1:
                        products = await _context.Products
                            .Where(p => p.CategoryId == catId && p.IsDeleted == false)
                            .OrderByDescending(p => p.Id)
                            .Skip((page -1) * LIMIT)
                            .Take(LIMIT)
                            .Include(p => p.Images.Where(pi => pi.Type == ImageType.Main))
                            .ToListAsync();
                        break;
                    case 2:
                        products = await _context.Products
                           .Where(p => p.CategoryId == catId && p.IsDeleted == false)
                           .OrderBy(p => p.Name)
                           .Skip((page - 1) * LIMIT)
                           .Take(LIMIT)
                           .Include(p => p.Images.Where(pi => pi.Type == ImageType.Main))
                           .ToListAsync();
                        break;
                    case 3:
                        products = await _context.Products
                           .Where(p => p.CategoryId == catId && p.IsDeleted == false)
                           .OrderBy(p => p.SalePrice)
                           .Skip((page - 1) * LIMIT)
                           .Take(LIMIT)
                           .Include(p => p.Images.Where(pi => pi.Type == ImageType.Main))
                           .ToListAsync();
                        break;

                }
            }
            else
            {
               
                count = await _context.Products.Where(p => p.IsDeleted == false).CountAsync();
                totalPages = (int)Math.Ceiling((double)count / LIMIT);
                switch (sortBy)
                {
                    case 1:
                        products = await _context.Products
                           .Where(p => p.IsDeleted == false)
                            .OrderByDescending(p => p.Id)
                            .Skip((page - 1) * LIMIT)
                            .Take(LIMIT)
                            .Include(p => p.Images.Where(pi => pi.Type == ImageType.Main))
                            .ToListAsync();
                        break;
                    case 2:
                        products = await _context.Products
                           .Where(p => p.IsDeleted == false)
                           .OrderBy(p => p.Name)
                           .Skip((page - 1) * LIMIT)
                           .Take(LIMIT)
                           .Include(p => p.Images.Where(pi => pi.Type == ImageType.Main))
                           .ToListAsync();
                        break;
                    case 3:
                        products = await _context.Products
                           .Where(p => p.IsDeleted == false)
                           .OrderBy(p => p.SalePrice)
                           .Skip((page - 1) * LIMIT)
                           .Take(LIMIT)
                           .Include(p => p.Images.Where(pi => pi.Type == ImageType.Main))
                           .ToListAsync();
                        break;

                }
                
            }
            return View(new ShopVM
            {
                CurrentPage = page,
                TotalPages = totalPages,
                Products = products,
                SortBy = sortBy,
                CategoryId = catId,
                Colors = await _context.Colors.Where(c => c.IsDeleted == false).Include(c => c.ProductColors).ToListAsync(),

            });


        }

        public async Task<IActionResult> Details(int id)
        {
            id.CheckPositiveNum();
            Product? product = await _context.Products
                .Where(p => p.Id == id && p.IsDeleted == false)
                .Include(p => p.Images)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .Include(p=> p.Category)
                .FirstOrDefaultAsync();
            product.CheckNull();
            ICollection<Product> similarProducts = await _context.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.IsDeleted == false)
                .Include(p => p.Images.Where(pi => pi.Type == ImageType.Main))
                .ToListAsync();
            return View(new DetailVM { Product = product, SimilarProducts = similarProducts });
        }
    }
}
