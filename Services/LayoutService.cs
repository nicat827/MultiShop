using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.ViewModels.Cookies;
using Newtonsoft.Json;
using System.Security.Claims;

namespace MultiShop.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public LayoutService(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            return await _context.Settings.ToDictionaryAsync(s => s.Key, s=> s.Value);
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();
        }
       
        public async Task<int> GetBasketItemsCountAsync(HttpContext context)
        {
            int count = 0;
            if (!context.User.Identity.IsAuthenticated)
            {
                if (context.Request.Cookies["Basket"] is not null)
                {
                    
                    IList<CookiesBasketVM> cookiesBasket = JsonConvert.DeserializeObject<IList<CookiesBasketVM>>(context.Request.Cookies["Basket"]);
                    bool hasCnanged = false;
                    for(int i = 0; i< cookiesBasket.Count; i++) 
                    {
                        Product? product = await _context.Products
                            .Where(p => p.IsDeleted == false && p.Id == cookiesBasket[i].Id)
                            .FirstOrDefaultAsync();
                        if (product is null)
                        {
                            hasCnanged = true;
                            cookiesBasket.Remove(cookiesBasket[i]);
                        }
                    }
                    if (hasCnanged)
                    {
                        context.Response.Cookies.Append("Basket", JsonConvert.SerializeObject(cookiesBasket));
                    }
                    count = cookiesBasket.Count;
                }
            }
            else
            {
                count = await _context.BasketItems.Where(bi => bi.AppUserId == context.User.FindFirstValue(ClaimTypes.NameIdentifier)).CountAsync();
            }
            return count;
        }
    }
}
