namespace MultiShop.Entities
{
    public class BasketItem
    {
        public int Id { get; set; }
        public int Count { get; set; }
        //relational props
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;






    }
}
