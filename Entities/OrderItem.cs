namespace MultiShop.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public  int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        public decimal Price { get; set; }

        public int Count { get; set; }

    }
}
