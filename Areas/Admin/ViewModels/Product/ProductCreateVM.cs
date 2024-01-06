using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.Admin.ViewModels
{
    public class ProductCreateVM
    {
        public IFormFile MainPhoto { get; set; } = null!;

        public IFormFile HoverPhoto { get; set; } = null!;

        public ICollection<IFormFile>? OtherPhotos { get; set; }
        public string Name { get; set; } = null!;

        [Range(1,999999)]
        public decimal CostPrice { get; set; }

        [Range(1, 999999)]
        public decimal SalePrice { get; set; }
        [Range(0, 999999)]
        public decimal Discount { get; set; }

        public string? Description { get; set; }

        // releation props
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Category is required!")]
        public int CategoryId { get; set; }

        public ICollection<int>? ColorIds { get; set; }

        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<Color> Colors { get; set; } = new List<Color>();
    }
}
