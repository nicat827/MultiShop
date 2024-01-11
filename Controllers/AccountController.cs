using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Services;
using MultiShop.Utilities.Extencions;
using MultiShop.ViewModels.Cookies;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace MultiShop.Controllers
{
    public class AccountController:Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(
            AppDbContext context,
            EmailService emailService,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager)
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (User.Identity.IsAuthenticated) return NotFound();
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
           
            if (await _userManager.Users.AnyAsync(u => u.UserName == vm.UserName))
            {
                ModelState.AddModelError("UserName", "User with this username already exists!");
                return View(vm);

            }
            if (await _userManager.Users.AnyAsync(u => u.Email == vm.Email)) 
            {
                ModelState.AddModelError("Email", "User with this email already exists!");
                return View(vm);
            }
            AppUser newUser = new AppUser
            {
                Name = vm.Name.Capitalize(),
                Surname = vm.Surname.Capitalize(),
                UserName = vm.UserName,
                Email = vm.Email
            };
            var res = await _userManager.CreateAsync(newUser, vm.Password);
            if (!res.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var err in res.Errors)
                {
                    sb.AppendLine(err.Description);
                }
                return View(string.Empty, sb.ToString());
            }

            await _userManager.AddToRoleAsync(newUser, UserRole.Member.ToString());
            string emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            string action = Url.Action(nameof(Confirm), "Account", new { Token = emailToken, Email = newUser.Email }, Request.Scheme);
            string body = $"<a style='height:50px; background-color:black;color:white;border:1px solid white; border-radius:8px; text-decoration:none; font-weight:700;padding:10px;font-size:24px;' href={action}>Confirm Email</a>";
            await _emailService.SendEmailAsync(body, "Confirm your account :)", newUser.Email);
            ViewBag.ConfirmModal = true;
            return View();   
        }

        public async Task<IActionResult> Confirm(string token, string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);
            user.CheckNull();
            var res = await _userManager.ConfirmEmailAsync(user, token);
            if (!res.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var err in res.Errors)
                {
                    sb.AppendLine(err.Description);
                }
                throw new Exception(sb.ToString());
            }
            if (Request.Cookies["Basket"] is not null)
            {
                ICollection<CookiesBasketVM> basket = JsonConvert.DeserializeObject<ICollection<CookiesBasketVM>>(Request.Cookies["Basket"]);
                foreach (var item in basket)
                {
                    if(await _context.Products.AnyAsync(p => p.Id == item.Id && p.IsDeleted == false)) 
                        user.BasketItems.Add(new BasketItem { ProductId = item.Id, Count = item.Count });
                }
                Response.Cookies.Delete("Basket");
                await _context.SaveChangesAsync();
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["Confirmed"] = true;
            return RedirectToAction("Index", "Home");


        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm, string? returnUrl= null)
        {
            if (User.Identity.IsAuthenticated) return NotFound();
            if (!ModelState.IsValid) return View(vm);
            AppUser user = await _userManager.FindByNameAsync(vm.UserNameOrEmail);
            if (user is null)
            {
                user = await _userManager.FindByEmailAsync(vm.UserNameOrEmail);
                if (user is null)
                {
                    ModelState.AddModelError(string.Empty, "UserName,Email or Password is incorrect!");
                    return View(vm);
                }
            }

            var res = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.IsPersistence, true);
            if (res.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Too many failed attempts, try later!");
                vm.IsLocked = true;
                return View(vm);
            }
            if (!res.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Userame, email or password is incorrect!");
                return View();
            }
            if (returnUrl is not null)
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> Logout(string? returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            if (returnUrl is not null) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

    }
}
