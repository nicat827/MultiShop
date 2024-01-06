using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.Admin.ViewModels
{
    public class ProductUpdateVM
    {
        public IFormFile? MainPhoto { get; set; } = null!;

        public IFormFile? HoverPhoto { get; set; } = null!;

        public ICollection<IFormFile>? OtherPhotos { get; set; }
        public string Name { get; set; } = null!;

        [Range(1, 999999)]
        public decimal CostPrice { get; set; }

        [Range(0, 999999)]
        public decimal Discount { get; set; }
       
        [Range(1, 999999)]
        public decimal SalePrice { get; set; }
        public string? Description { get; set; }

        // releation props
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Category is required!")]
        public int CategoryId { get; set; }

        public List<int>? ColorIds { get; set; }
        public List<int>? ImageIds { get; set; }

        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
        public IEnumerable<Color> Colors { get; set; } = new List<Color>();
    }
}
