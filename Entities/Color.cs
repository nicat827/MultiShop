
namespace MultiShop.Entities
{
    public class Color:BaseNameableEntity
    {
        public ICollection<ProductColor> ProductColors { get; set; } = new List<ProductColor>();
    }
}
