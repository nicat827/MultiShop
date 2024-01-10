using Microsoft.AspNetCore.Identity;

namespace MultiShop.Entities
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;

        //relational

        public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

    }
}
