namespace MultiShop.Entities
{
    public class ProductColor:BaseEntity
    {
        public int Stock {  get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int ColorId { get; set; }
        public Color Color { get; set; } = null!;

    }
}
