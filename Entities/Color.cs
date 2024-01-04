
namespace MultiShop.Entities
{
    public class Color:BaseNameableEntity
    {
        public IEnumerable<ProductColor> ProductColors { get; set; } = new List<ProductColor>();
    }
}
