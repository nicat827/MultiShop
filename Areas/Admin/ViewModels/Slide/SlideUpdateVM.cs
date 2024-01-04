using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.Admin.ViewModels
{
    public class SlideUpdateVM
    {
        public IFormFile? Photo { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string ButtonText { get; set; } = null!;

        [Range(1,100)]
        public int Order { get; set; }

    }
}
