namespace MultiShop.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime CreatedAt { get; set;}
        public DateTime LastUpdatedAt { get; set;}

        public bool IsDeleted { get; set; }
    }
}
