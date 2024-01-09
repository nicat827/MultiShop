using Microsoft.AspNetCore.Identity;

namespace MultiShop.Entities
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }

    }
}
