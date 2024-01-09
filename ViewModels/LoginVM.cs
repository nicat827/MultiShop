using System.ComponentModel.DataAnnotations;

namespace MultiShop.ViewModels
{
    public class LoginVM
    {
        [MinLength(5)]
        [MaxLength(100)]
        public string UserNameOrEmail { get; set; } = null!;

        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public bool IsPersistence { get; set; }

        public bool? IsLocked { get; set; }
    }
}
