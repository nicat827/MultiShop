using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Entities;
using MultiShop.Utilities.Extencions;
using MultiShop.ViewModels.Cookies;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MultiShop.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CartController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async  Task<IActionResult> Index()
        {
            ICollection<BasketItemVM> basketItemsVM = new List<BasketItemVM>();
            if (!User.Identity.IsAuthenticated)
            {

                if (Request.Cookies["Basket"] is not null)
                {
                    IEnumerable<CookiesBasketVM> cookiesBasket = JsonConvert.DeserializeObject<IEnumerable<CookiesBasketVM>>(Request.Cookies["Basket"]);
                    foreach (var item in cookiesBasket)
                    {
                        Product? product = await _context.Products
                            .Where(p => p.IsDeleted == false && p.Id == item.Id)
                            .Include(p => p.Images.Where(pi => pi.Type == ImageType.Main))
                            .FirstOrDefaultAsync();
                        if (product is not null) basketItemsVM.Add(new BasketItemVM { 
                            Id = product.Id,
                            Name = product.Name,
                            ImageUrl = product.Images[0].ImageUrl,
                            Price = product.SalePrice,
                            Count = item.Count,
                            Subtotal = product.SalePrice * item.Count

                        });
                        
                    }
                }
            }
            else
            {
                IEnumerable<BasketItem> itemsFromDb  = await _context.BasketItems
                    .Where(bi => bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Include(bi => bi.Product).ThenInclude(p => p.Images.Where(i => i.Type == ImageType.Main))
                    .ToListAsync();
                foreach (BasketItem item in itemsFromDb)
                {
                    basketItemsVM.Add(new BasketItemVM
                    {
                        Id = item.ProductId,
                        Name = item.Product.Name,
                        ImageUrl = item.Product.Images[0].ImageUrl,
                        Price = item.Product.SalePrice,
                        Count = item.Count,
                        Subtotal = item.Product.SalePrice * item.Count

                    });
                }

            }
            return View(new CartVM { BasketItems = basketItemsVM });
           
        }


        public async Task<IActionResult> Add(int id, string? returnUrl= null)
        {
            id.CheckPositiveNum();
            Product? product = await _context.Products.Where(p => p.IsDeleted == false && p.Id == id).FirstOrDefaultAsync();
            product.CheckNull();
            if (!User.Identity.IsAuthenticated)
            {
                ICollection<CookiesBasketVM> currentBasket = new List<CookiesBasketVM>();
                if (Request.Cookies["Basket"] is not null)
                {
                    currentBasket = JsonConvert.DeserializeObject<ICollection<CookiesBasketVM>>(Request.Cookies["Basket"]);
                    CookiesBasketVM? findedItem = currentBasket.FirstOrDefault(i => i.Id == id);
                    if (findedItem is null) currentBasket.Add(new CookiesBasketVM { Id = id, Count = 1 });
                    else findedItem.Count++;
                } 
                else
                {
                    currentBasket.Add(new CookiesBasketVM { Count = 1, Id = id });
                }
                Response.Cookies.Append("Basket", JsonConvert.SerializeObject(currentBasket));
            }
            else
            {
                AppUser? user = await _userManager.Users
                    .Include(u => u.BasketItems).ThenInclude(bi => bi.Product)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                user.CheckNull();
                BasketItem? findedItem = user.BasketItems.FirstOrDefault(bi => bi.ProductId == id);
                if (findedItem is null) user.BasketItems.Add(new BasketItem { ProductId = id, Count = 1 });
                else findedItem.Count++;
                await _context.SaveChangesAsync();
            }


            if (returnUrl is not null) return Redirect(returnUrl);
            return RedirectToAction(nameof(Index), "Cart");
        }

        public async Task<IActionResult> Remove(int id,bool all=true)
        {
            id.CheckPositiveNum();
            if (!User.Identity.IsAuthenticated)
            {
                if (Request.Cookies["Basket"] is not null)
                {
                    ICollection<CookiesBasketVM> cookies = JsonConvert.DeserializeObject<ICollection<CookiesBasketVM>>(Request.Cookies["Basket"]);
                    CookiesBasketVM findedItem = cookies.FirstOrDefault(i => i.Id == id);

                    if (findedItem is not null)
                    {
                        if (!all && findedItem.Count > 1) findedItem.Count--;
                        else cookies.Remove(findedItem);
                    }
                    Response.Cookies.Append("Basket", JsonConvert.SerializeObject(cookies));
                }

            }
            else
            {
                BasketItem? basketItem = await _context.BasketItems
                     .Include(bi => bi.Product)
                     .Include(bi => bi.AppUser)
                     .Where(bi => bi.ProductId == id && bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                     .FirstOrDefaultAsync();
                if (basketItem is null) throw new Exception("Basket item with this Id doesnt exist!");
                if (!all && basketItem.Count > 1) basketItem.Count--;
                else _context.BasketItems.Remove(basketItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
