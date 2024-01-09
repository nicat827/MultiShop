using Microsoft.AspNetCore.Identity;

namespace MultiShop.DAL
{
    public class AppDbContextInitializer
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AppDbContextInitializer(
            AppDbContext context,
            IConfiguration configuration,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeDbAsync()
        {
            await _context.Database.EnsureCreatedAsync();
        }
        public async Task CreateRolesAsync()
        {
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
                }
            }
        }

        public async Task CreateAdminAsync()
        {
            AppUser admin = new AppUser
            {
                Name = "admin",
                Surname = "admin",
                UserName = _configuration["AdminSettings:UserName"],
                Email = _configuration["AdminSettings:Email"],
            };
            var res = await _userManager.CreateAsync(admin, _configuration["AdminSettings:Password"]);
            if (res.Succeeded) 
            {
                await _userManager.AddToRoleAsync(admin, UserRole.Admin.ToString());
            }
        }
    }
}
