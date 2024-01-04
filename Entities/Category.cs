
namespace MultiShop.Entities
{
    public class Category:BaseNameableEntity
    {
        public IFormFile Photo { get; set; } = null!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
