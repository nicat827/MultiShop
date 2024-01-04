namespace MultiShop.Entities
{
    public class Product:BaseNameableEntity
    {
        public string SKU { get; set; } = null!;

        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }

        public string? Description { get; set; }

        // releation props
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductColor> ProductColors { get; set; } = new List<ProductColor>();



    }
}
