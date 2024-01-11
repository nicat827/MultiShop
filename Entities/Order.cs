namespace MultiShop.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? ConfirmerId { get; set; } = null!;
        public AppUser? Confirmer { get; set; }
        public DateTime? DeliveredAt { get; set; }
        
        public OrderStatus Status { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;



    }
}
