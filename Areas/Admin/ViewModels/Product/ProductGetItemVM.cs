namespace MultiShop.Areas.Admin.ViewModels
{
    public class ProductGetItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public IList<ProductImage> Images { get; set; } = null!;
        public decimal SalePrice { get; set; }

    }
}
