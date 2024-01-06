namespace MultiShop.ViewModels
{
    public class DetailVM
    {
        public Product Product { get; set; } = null!;
        public ICollection<Product>? SimilarProducts { get; set; }
    }
}
