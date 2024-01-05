using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Utilities.Extencions;

namespace MultiShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ColorController : Controller
    {
        private const int LIMIT = 5;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public ColorController(AppDbContext context, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            page.CheckPositiveNum();
            int count = await _context.Colors.Where(c => c.IsDeleted == false).CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / LIMIT);

            IEnumerable<Color> colors = await _context.Colors
                .Where(c => c.IsDeleted == false)
                .Skip((page - 1) * LIMIT)
                .Take(LIMIT)
                .ToListAsync();
            IEnumerable<ColorGetItemVM> colorGetItemVMs = _mapper.Map<IEnumerable<ColorGetItemVM>>(colors);
            PaginationVM<ColorGetItemVM> pagVM = new()
            {
                Items = colorGetItemVMs,
                CurrentPage = page,
                TotalPages = totalPages
            };
            return View(pagVM);
        }

        public async Task<IActionResult> Details(int id)
        {
            id.CheckPositiveNum();
            Color color = await _context.Colors
                .Include(c => c.ProductColors).ThenInclude(pc => pc.Product).ThenInclude(pc => pc.Images.Where(pi => pi.Type == ImageType.Main))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);
            color.CheckNull();
            return View(color);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ColorCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            if (await _context.Colors.AnyAsync(c => c.Name == vm.Name))
            {
                ModelState.AddModelError("Name", "Color with this name already exists!");
                return View(vm);
            }
            Color color = new Color { Name = vm.Name.Capitalize() };
            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            id.CheckPositiveNum();
            Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            color.CheckNull();
            ColorUpdateVM vm = new()
            {
                Name = color.Name
            };
            return View(vm);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, ColorUpdateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            Color color = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new Exception("Color didn't found");
            if (vm.Name != color.Name)
            {
                if (await _context.Colors.AnyAsync(c => c.Name == vm.Name))
                {
                    ModelState.AddModelError("Name", "Color with this name already exists!");
                    return View(vm);
                }
                color.Name = vm.Name.Capitalize();
            }
        
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            id.CheckPositiveNum();
            Color cat = await _context.Colors.FirstOrDefaultAsync(c => c.Id == id);
            cat.CheckNull();
            _context.Remove(cat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }

}
