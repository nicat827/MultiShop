using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.Admin.ViewModels
{
    public class CategoryCreateVM
    {
        [MinLength(3)]
        public string Name { get; set; } = null!;

        public IFormFile Photo { get; set; } = null!;
    }
}
