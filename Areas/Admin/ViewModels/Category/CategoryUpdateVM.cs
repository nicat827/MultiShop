using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.Admin.ViewModels
{
    public class CategoryUpdateVM
    {
        [MinLength(3)]
        public string Name { get; set; } = null!;

        public IFormFile? Photo { get; set; }
    }
}
