namespace MultiShop.ViewModels
{
    public class BasketItemVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public decimal Price { get; set; }

        public int Count { get; set; }

        public decimal Subtotal { get; set; }
    }
}
