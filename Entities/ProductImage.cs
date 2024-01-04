using MultiShop.Utilities.Enums;

namespace MultiShop.Entities
{
    public class ProductImage:BaseEntity
    {
        public ImageType Type { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;
    }
}
