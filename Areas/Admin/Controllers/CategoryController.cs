using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Areas.Admin.ViewModels;
using MultiShop.DAL;
using MultiShop.Entities;
using MultiShop.Utilities.Extencions;
using MultiShop.ViewModels;

namespace MultiShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController:Controller
    {
        private const int LIMIT = 5;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public CategoryController(AppDbContext context, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }
        public async Task<IActionResult> Index(int page= 1)
        {
            page.CheckPositiveNum();
            int count = await _context.Categories.Where(c => c.IsDeleted == false).CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / LIMIT);
            
            IEnumerable <Category> categories = await _context.Categories
                .Where(c => c.IsDeleted == false)
                .Skip((page - 1) * LIMIT)
                .Take(LIMIT)
                .ToListAsync();
            IEnumerable<CategoryGetItemVM> categoryGetItemVMs = _mapper.Map<IEnumerable<CategoryGetItemVM>>(categories);
            PaginationVM<CategoryGetItemVM> pagVM = new()
            {
                Items = categoryGetItemVMs,
                CurrentPage = page,
                TotalPages = totalPages
            };
            return View(pagVM);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            if (!vm.Photo.CheckFileType())
            {
                ModelState.AddModelError("Photo", "Invalid type of photo!");
                return View(vm);

            }
            if (!vm.Photo.CheckFileSize(500))
            {
                ModelState.AddModelError("Photo", "Invalid size of photo (longer than 500Kb)!");
                return View(vm);

            }
            if (await _context.Categories.AnyAsync(c => c.Name == vm.Name)) {
                ModelState.AddModelError("Name", "Category with this name already exists!");
                return View(vm);
            }
            Category category = new Category { Name = vm.Name.Capitalize() };
            category.ImageUrl = await vm.Photo.CreateFileAsync(_env.WebRootPath, "uploads", "category");
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            id.CheckPositiveNum();
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            category.CheckNull();
            CategoryUpdateVM vm = new()
            {
                Name = category.Name
            };
            return View(vm);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, CategoryUpdateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new Exception("Category didn't found");
            if (vm.Name != category.Name)
            {
                if (await _context.Categories.AnyAsync(c => c.Name == vm.Name))
                {
                    ModelState.AddModelError("Name", "Category with this name already exists!");
                    return View(vm);
                }
                category.Name = vm.Name.Capitalize();
            }
            if (vm.Photo is not null)
            {
                if (!vm.Photo.CheckFileType())
                {
                    ModelState.AddModelError("Photo", "Invalid type of photo!");
                    return View(vm);

                }
                if (!vm.Photo.CheckFileSize(500))
                {
                    ModelState.AddModelError("Photo", "Invalid size of photo (longer than 500Kb)!");
                    return View(vm);

                }
                category.ImageUrl = await vm.Photo.CreateFileAsync(_env.WebRootPath, "uploads", "category");
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            id.CheckPositiveNum();
            Category cat = await _context.Categories
                .Include(c => c.Products).ThenInclude(p => p.Images
                .Where(i => i.Type == ImageType.Main))
                .FirstOrDefaultAsync(c => c.Id == id);

            cat.CheckNull();
            return View(cat);
        }
        public async Task<IActionResult> Delete(int id)
        {
            id.CheckPositiveNum();
            Category cat = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            cat.CheckNull();
            cat.ImageUrl.DeleteFile(_env.WebRootPath, "uploads", "category");
            _context.Remove(cat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
