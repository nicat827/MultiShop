using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using System.Security.Claims;

namespace MultiShop.Controllers
{
    [Authorize(Roles = "Member")]
    public class CheckoutController:Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CheckoutController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<BasketItem> userItems = await _context.BasketItems
                .Where(bi => bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Include(bi => bi.Product)
                .ToListAsync();
            ICollection<CheckoutItemVM> checkoutItems = new List<CheckoutItemVM>();
            foreach (var item in userItems)
            {
                checkoutItems.Add(new CheckoutItemVM { ProductName = item.Product.Name, Subtotal = item.Product.SalePrice * item.Count });
            }
            return View(new CheckoutVM { CheckoutItems = checkoutItems});
        }

        [HttpPost]
        public async Task<IActionResult> Index(CheckoutVM checkoutVM)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<BasketItem> userItems = await _context.BasketItems
                .Where(bi => bi.AppUserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                .Include(bi => bi.Product)
                .ToListAsync();
                ICollection<CheckoutItemVM> checkoutItems = new List<CheckoutItemVM>();
                foreach (var item in userItems)
                {
                    checkoutItems.Add(new CheckoutItemVM { ProductName = item.Product.Name, Subtotal = item.Product.SalePrice * item.Count });
                }
                checkoutVM.CheckoutItems = checkoutItems;
                return View(checkoutVM);
            }
            AppUser? user = await _userManager.Users.Where(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Include(u => u.BasketItems).ThenInclude(bi => bi.Product)
                    .FirstOrDefaultAsync();
            if (user is null) throw new Exception("User wasnt defined!");
            Order order = new Order
            {
                AppUserId = user.Id,
                Status = OrderStatus.Pending
            };
            decimal totalPrice = 0;
            foreach (BasketItem item in user.BasketItems)
            {
                totalPrice += item.Count * item.Product.SalePrice;
                order.OrderItems.Add(new OrderItem 
                {
                    Count = item.Count,
                    ProductId = item.ProductId,
                    Price = item.Product.SalePrice
                });
            }
            order.TotalPrice = totalPrice;
            order.CreatedAt = DateTime.UtcNow;
            await _context.Orders.AddAsync(order);
            user.BasketItems = new List<BasketItem>();
            await _context.SaveChangesAsync();
            TempData["SuccessOrder"] = true;
            return RedirectToAction("Index", "Home");
            
        }
    }
}
