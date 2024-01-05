using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Utilities.Enums;
using MultiShop.Utilities.Extencions;

namespace MultiShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController:Controller
    {
        private const int LIMIT = 5;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public ProductController(AppDbContext context, IWebHostEnvironment env, IMapper mapper)
        {
            _context = context;
            _env = env;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            page.CheckPositiveNum();
            // isDeleted false olan productlarin countu ( hamsi yox)
            int productsCount = await _context.Products.Where(p => p.IsDeleted == false).CountAsync();
            int totalPages = (int)Math.Ceiling((double)productsCount / LIMIT);

            //wheri yuxaarda vermek lazimdir ki , isDeleted true olsa duzgun islemir ( meselen: 2 -ci seyfede 3 dene product cekmelidir
            //, where asagida olsa hamsin cekecey ve birin gostermiyecey)
            IEnumerable<Product> products = await _context.Products
                .Where(p => p.IsDeleted == false)
                .Skip((page - 1) * LIMIT).Take(LIMIT)
                .Include(p => p.Images.Where(pi => pi.Type == ImageType.Main))
                .ToListAsync();
            IEnumerable<ProductGetItemVM> vm = _mapper.Map<IEnumerable<ProductGetItemVM>>(products);

            return View(new PaginationVM<ProductGetItemVM>
            {
                CurrentPage = page,
                TotalPages = totalPages,
                Items = vm
            });
        }

        public async Task<IActionResult> Create()
        {
            return View(new ProductCreateVM
            {
                Categories = await GetCategoriesAsync(),
                Colors = await GetColorsAsync()
                //Sizes = await GetSizesAsync()
            });

        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await GetCategoriesAsync();
                productVM.Colors = await GetColorsAsync();
                //productVM.Sizes = await GetSizesAsync();
                return View(productVM);
            }
            // main photo validation
            if (!productVM.MainPhoto.CheckFileType())
            {
                ModelState.AddModelError("MainPhoto", "Please, make sure, you uploaded a photo!");
                productVM.Categories = await GetCategoriesAsync();
                productVM.Colors = await GetColorsAsync();
                //productVM.Sizes = await GetSizesAsync();
                return View(productVM);
            }
            if (!productVM.MainPhoto.CheckFileSize(500))
            {
                ModelState.AddModelError("MainPhoto", "Photo size can't be bigger than 500kB!");
                productVM.Categories = await GetCategoriesAsync();
                productVM.Colors = await GetColorsAsync();
                //productVM.Sizes = await GetSizesAsync();
                return View(productVM);
            }
            // hover photo validation
            if (!productVM.HoverPhoto.CheckFileType())
            {
                ModelState.AddModelError("HoverPhoto", "Please, make sure, you uploaded a photo!");
                productVM.Categories = await GetCategoriesAsync();
                productVM.Colors = await GetColorsAsync();
                //productVM.Sizes = await GetSizesAsync();
                return View(productVM);
            }
            if (!productVM.HoverPhoto.CheckFileSize(500))
            {
                ModelState.AddModelError("HoverPhoto", "Photo size can't be bigger than 500kB!");
                productVM.Categories = await GetCategoriesAsync();
                productVM.Colors = await GetColorsAsync();
                //productVM.Sizes = await GetSizesAsync();
                return View(productVM);
            }
            if (await _context.Products.AnyAsync(p => p.Name == productVM.Name))
            {
                ModelState.AddModelError("Name", "Product with this name already exists!");
                return View(productVM);
            }
            if (!await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Please, make sure you choosed an exist category!");
                productVM.Categories = await GetCategoriesAsync();
                productVM.Colors = await GetColorsAsync();
                //productVM.Sizes = await GetSizesAsync();
                return View(productVM);
            }
            productVM.Name = productVM.Name.Capitalize();
            Product newProduct = _mapper.Map<Product>(productVM);
            if (productVM.ColorIds is not null)
            {
                foreach (int colorId in productVM.ColorIds)
                {
                    if (!await _context.Colors.AnyAsync(t => t.Id == colorId))
                    {
                        ModelState.AddModelError("ColorIds", "Please, make sure you chooced an existed color!");
                        productVM.Categories = await GetCategoriesAsync();
                        productVM.Colors = await GetColorsAsync();
                        //productVM.Sizes = await GetSizesAsync();
                        return View(productVM);
                    }
                    newProduct.ProductColors.Add(new ProductColor { ColorId = colorId });

                }

            }



            newProduct.Images.Add(new ProductImage
            {
                Type = ImageType.Main,
                ImageUrl = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "uploads", "product", "main")
            });

            newProduct.Images.Add(new ProductImage
            {
                Type = ImageType.Hover,
                ImageUrl = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "uploads", "product","hover")
            });

            if (productVM.OtherPhotos is not null)
            {
                TempData["ErrorMessages"] = "";

                foreach (IFormFile photo in productVM.OtherPhotos)
                {
                    if (!photo.CheckFileType())
                    {
                        TempData["ErrorMessages"] += $"<p class=\"text-danger\">Image with name {photo.FileName} wasnt created, because type is not valid!</p>";
                        continue;
                    }
                    if (!photo.CheckFileSize(500))
                    {
                        TempData["ErrorMessages"] += $"<p class=\"text-danger\">Image with name {photo.FileName} wasnt created, because size must be lower than 500kB!</p>";

                        continue;
                    }
                    newProduct.Images.Add(new ProductImage
                    {
                        Type = ImageType.Other,
                        ImageUrl = await photo.CreateFileAsync(_env.WebRootPath, "uploads", "product","other")
                    });
                }
            }
            newProduct.SKU = productVM.Name.Substring(0,3).ToUpper()+productVM.CostPrice.ToString();
            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Details(int id)
        {
            id.CheckPositiveNum();
            Product product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
            ProductGetVM vm = _mapper.Map<ProductGetVM>(product);
            vm.Colors = product.ProductColors.Select(pc => pc.Color).ToList();
            vm.RelatedProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images.Where(i => i.Type == ImageType.Main))
                .Where(p => p.CategoryId == product.CategoryId && p.IsDeleted == false)
                .ToListAsync();
            return View(vm);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();

            if (product.Images is not null)
            {
                foreach (ProductImage image in product.Images)
                {
                    switch (image.Type)
                    {
                        case ImageType.Main:
                            image.ImageUrl.DeleteFile(_env.WebRootPath, "uploads", "product","main");
                            break;
                        case ImageType.Hover:
                            image.ImageUrl.DeleteFile(_env.WebRootPath, "uploads", "product", "hover");
                            break;
                        case ImageType.Other:
                            image.ImageUrl.DeleteFile(_env.WebRootPath, "uploads", "product", "other");
                            break;
                    }
                }
            }



            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            id.CheckPositiveNum();
            Product product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            product.CheckNull();

            ProductUpdateVM productVM = _mapper.Map<ProductUpdateVM>(product);
            productVM.Colors = await GetColorsAsync();
            productVM.Categories = await GetCategoriesAsync();
            productVM.Images = productVM.Images;
            productVM.ColorIds = product.ProductColors.Select(pc => pc.ColorId).ToList();
            return View(productVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, ProductUpdateVM productVM)
        {
            Product product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);

            product.CheckNull();
            if (!ModelState.IsValid)
            {
                productVM.Categories = await GetCategoriesAsync();
                productVM.Colors = await GetColorsAsync();
                productVM.Images = product.Images;
                return View(productVM);
            }
            productVM.Name = productVM.Name.Capitalize();
            if (productVM.Name != product.Name)
            {
                if (await _context.Products.AnyAsync(p => p.Name == productVM.Name))
                {
                    ModelState.AddModelError("Name", "Product with this name already exists!");
                    productVM.Categories = await GetCategoriesAsync();
                    productVM.Colors = await GetColorsAsync();
                    productVM.Images = product.Images;
                    return View(productVM);
                }
            }
            if (productVM.CategoryId != product.CategoryId)
            {
                if (!await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId))
                {
                    ModelState.AddModelError("CategoryId", "Please, make sure you choosed an exist category!");
                    productVM.Categories = await GetCategoriesAsync();
                    productVM.Colors = await GetColorsAsync();
                    productVM.Images = product.Images;
                    //productVM.Sizes = await GetSizesAsync();
                    return View(productVM);
                }
                product.CategoryId = productVM.CategoryId;


            }
            if (productVM.MainPhoto is not null)    
            {
                if (!productVM.MainPhoto.CheckFileType())
                {
                    ModelState.AddModelError("MainPhoto", "Please, make sure, you uploaded a photo!");
                    productVM.Categories = await GetCategoriesAsync();
                    productVM.Colors = await GetColorsAsync();
                    productVM.Images = product.Images;
                    //productVM.Sizes = await GetSizesAsync();
                    return View(productVM);
                }
                if (!productVM.MainPhoto.CheckFileSize(500))
                {
                    ModelState.AddModelError("MainPhoto", "Photo size can't be bigger than 500kB!");
                    productVM.Categories = await GetCategoriesAsync();
                    productVM.Colors = await GetColorsAsync();
                    productVM.Images = product.Images;
                    //productVM.Sizes = await GetSizesAsync();
                    return View(productVM);
                }
                ProductImage mainPhoto = product.Images.FirstOrDefault(pi => pi.Type == ImageType.Main);
                mainPhoto.ImageUrl.DeleteFile(_env.WebRootPath, "uploads", "product", "main");
                mainPhoto.ImageUrl = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "uploads", "product", "main");
            }
            if (productVM.HoverPhoto is not null)
            { 
                if (!productVM.HoverPhoto.CheckFileType())
                {
                    ModelState.AddModelError("HoverPhoto", "Please, make sure, you uploaded a photo!");
                    productVM.Categories = await GetCategoriesAsync();
                    productVM.Colors = await GetColorsAsync();
                    productVM.Images = product.Images;
                    //productVM.Sizes = await GetSizesAsync();
                    return View(productVM);
                }
                if (!productVM.HoverPhoto.CheckFileSize(500))
                {
                    ModelState.AddModelError("HoverPhoto", "Photo size can't be bigger than 500kB!");
                    productVM.Categories = await GetCategoriesAsync();
                    productVM.Colors = await GetColorsAsync();
                    productVM.Images = product.Images;
                    //productVM.Sizes = await GetSizesAsync();
                    return View(productVM);
                }
                ProductImage hoverPhoto = product.Images.FirstOrDefault(pi => pi.Type == ImageType.Hover);
                hoverPhoto.ImageUrl.DeleteFile(_env.WebRootPath, "uploads", "product", "hover");
                hoverPhoto.ImageUrl = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "uploads", "product", "hover");

            }
            if (productVM.ColorIds is not null)
            {
                foreach (int colorId in productVM.ColorIds)
                {
                    if (!await _context.Colors.AnyAsync(t => t.Id == colorId))
                    {
                        ModelState.AddModelError("ColorIds", "Please, make sure you chooced an existed color!");
                        productVM.Categories = await GetCategoriesAsync();
                        productVM.Colors = await GetColorsAsync();
                        productVM.Images = product.Images;
                        //productVM.Sizes = await GetSizesAsync();
                        return View(productVM);
                    }
                    if (!product.ProductColors.Exists(pc => pc.ColorId == colorId)) product.ProductColors.Add(new ProductColor { ColorId = colorId });
                }
                product.ProductColors = product.ProductColors.Where(pc => productVM.ColorIds.Exists(id => id == pc.ColorId)).ToList();

            }
            if (productVM.ImageIds is not null)
            {
                foreach (var image in product.Images.Where(pi => pi.Type == ImageType.Other))
                {
                    if (!productVM.ImageIds.Exists(id => id == image.Id))
                    {
                        _context.ProductImages.Remove(image);

                    }
                }
            }
            else product.Images = product.Images.Where(pi => pi.Type != ImageType.Other).ToList();
            if (productVM.OtherPhotos is not null)
            {
                foreach (IFormFile photo in productVM.OtherPhotos)
                {
                    if (!photo.CheckFileType())
                    {
                        TempData["ErrorMessages"] += $"<p class=\"text-danger\">Image with name {photo.FileName} wasnt created, because type is not valid!</p>";
                        continue;
                    }

                    if (!photo.CheckFileSize(500))
                    {
                        TempData["ErrorMessages"] += $"<p class=\"text-danger\">Image with name {photo.FileName} wasnt created, because size bigger than we except (500kB) :(</p>";
                        continue;
                    }
                    product.Images.Add(new ProductImage 
                    { 
                        Type = ImageType.Other,
                        ImageUrl = await photo.CreateFileAsync(_env.WebRootPath,"uploads", "product","other")
                    });
                }

            }

            product.Name = productVM.Name;
            product.SalePrice = productVM.SalePrice;
            product.CostPrice = productVM.CostPrice;
            product.Description = productVM.Description;
            product.SKU = productVM.Name.Substring(0, 3).ToUpper() + productVM.CostPrice.ToString();
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        public async Task<List<Category>> GetCategoriesAsync()
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            return categories;
        }

     

        public async Task<List<Color>> GetColorsAsync()
        {
            return await _context.Colors.ToListAsync();
        }

        //public async Task<List<Size>> GetSizesAsync()
        //{
        //    return await _context.Sizes.ToListAsync();
        //}

    }
}
