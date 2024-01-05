using System.ComponentModel.DataAnnotations;

namespace MultiShop.Entities
{
    public class Product:BaseNameableEntity
    {
        public string SKU { get; set; } = null!;
        [Range(1, 999999)]

        public decimal CostPrice { get; set; }
        [Range(1, 999999)]

        public decimal SalePrice { get; set; }

        public string? Description { get; set; }

        // releation props
        [Range(1, int.MaxValue)]

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
        public List<ProductColor> ProductColors { get; set; } = new List<ProductColor>();



    }
}
