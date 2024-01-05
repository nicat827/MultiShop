namespace MultiShop.Areas.Admin.ViewModels
{
    public class ProductGetVM
    {
        public int Id { get; set; }
        public string Name { get; set; }= null!;
        public string? Description { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public string SKU { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public List<ProductImage> Images { get; set; } = null!;
        public ICollection<Color>? Colors { get; set; }
        public List<Product>? RelatedProducts { get; set; } = new List<Product>();
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

    }
}
