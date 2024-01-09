using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Utilities.Extencions;
using System.Text;
using System.Text.RegularExpressions;

namespace MultiShop.Controllers
{
    public class UserController:Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm, string? returnUrl=null)
        {
            if (User.Identity.IsAuthenticated) return NotFound();
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            Regex regex = new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$");
            if (!regex.IsMatch(vm.Email))
            {
                ModelState.AddModelError("Email", "Email is incorrect!");
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
            await _signInManager.SignInAsync(newUser, isPersistent: false);
            if (returnUrl is not null)
            {
                return Redirect(returnUrl);
            }
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

    }
}
