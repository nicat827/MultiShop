namespace MultiShop.ViewModels
{
    public class HomeVM
    {
        public IList<Slide>? Slides { get; set; }
        public ICollection<Category>? Categories { get; set; }
        public ICollection<Product>? FeaturedProducts { get; set; }
        public ICollection<Product>? LastProducts { get; set; }
    }
}
