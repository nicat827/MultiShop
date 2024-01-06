namespace MultiShop.ViewModels
{
    public class ShopVM
    {
        public IEnumerable<Product>? Products { get; set; }
        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }
        public int? CategoryId { get; set; }
        public int SortBy { get; set; }

        public ICollection<Color>? Colors { get; set; }
    }
}
