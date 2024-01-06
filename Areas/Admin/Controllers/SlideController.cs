using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Utilities.Extencions;

namespace MultiShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController:Controller
    {
        private const int LIMIT = 5;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            page.CheckPositiveNum();
            int count = await _context.Slides.Where(c => c.IsDeleted == false).CountAsync();
            int totalPages = (int)Math.Ceiling((double)count / LIMIT);

            IEnumerable<Slide> slides = await _context.Slides
                .Where(c => c.IsDeleted == false)
                .Skip((page - 1) * LIMIT)
                .Take(LIMIT)
                .ToListAsync();
            IEnumerable<SlideGetItemVM> slideGetItemVMs = _mapper.Map<IEnumerable<SlideGetItemVM>>(slides);
            PaginationVM<SlideGetItemVM> pagVM = new()
            {
                Items = slideGetItemVMs,
                CurrentPage = page,
                TotalPages = totalPages
            };
            return View(pagVM);
        }

        public async Task<IActionResult> Details(int id)
        {
            id.CheckPositiveNum();
            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id && s.IsDeleted == false);
            slide.CheckNull();
            return View(slide);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(SlideCreateVM vm)
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
            if (!vm.Photo.CheckFileSize(1000))
            {
                ModelState.AddModelError("Photo", "Invalid size of photo (longer than 1000Kb)!");
                return View(vm);

            }
            if (await _context.Slides.AnyAsync(c => c.Name == vm.Name))
            {
                ModelState.AddModelError("Name", "Slide with this name already exists!");
                return View(vm);
            }
            Slide slide = _mapper.Map<Slide>(vm);
            Slide existed =await  _context.Slides.FirstOrDefaultAsync(s => s.Order == vm.Order);
            if (existed is not null)
            {
                TempData["Order"] = $"Orders for {existed.Name} slide and {slide.Name} were swapped!";
                slide.Order = existed.Order;
                existed.Order = vm.Order;
            }
            slide.ImageUrl = await vm.Photo.CreateFileAsync(_env.WebRootPath, "uploads", "slide");
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            id.CheckPositiveNum();
            Slide slide = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);
            slide.CheckNull();
            SlideUpdateVM vm = _mapper.Map<SlideUpdateVM>(slide);
            return View(vm);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, SlideUpdateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);
            Slide slide = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new Exception("Slide didn't found");
            if (vm.Name != slide.Name)
            {
                if (await _context.Slides.AnyAsync(c => c.Name == vm.Name))
                {
                    ModelState.AddModelError("Name", "Slide with this name already exists!");
                    return View(vm);
                }
                slide.Name = vm.Name.Capitalize();
            }
            if (vm.Photo is not null)
            {
                if (!vm.Photo.CheckFileType())
                {
                    ModelState.AddModelError("Photo", "Invalid type of photo!");
                    return View(vm);

                }
                if (!vm.Photo.CheckFileSize(1000))
                {
                    ModelState.AddModelError("Photo", "Invalid size of photo (longer than 1000Kb)!");
                    return View(vm);

                }
                slide.ImageUrl = await vm.Photo.CreateFileAsync(_env.WebRootPath, "uploads", "slide");
            }
            slide.ButtonText = vm.ButtonText;
            slide.Description = vm.Description;
            if (vm.Order != slide.Order)
            {
                Slide existed = await _context.Slides.FirstOrDefaultAsync(s => s.Order == vm.Order);
                slide.Order = vm.Order;

                if (existed is not null)
                {
                    TempData["Order"] = $"Orders for {existed.Name} slide and {slide.Name} were swapped!";
                    existed.Order = vm.Order;
                }
                
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            id.CheckPositiveNum();
            Slide slide = await _context.Slides.FirstOrDefaultAsync(c => c.Id == id);
            slide.CheckNull();
            slide.ImageUrl.DeleteFile(_env.WebRootPath, "uploads", "slide");
            _context.Remove(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
