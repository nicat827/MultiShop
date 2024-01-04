
namespace MultiShop.Entities
{
    public class Category:BaseNameableEntity
    {
        public string ImageUrl { get; set; } = null!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
