using System.ComponentModel.DataAnnotations;

namespace MultiShop.ViewModels
{
    public class RegisterVM
    {
        [MinLength(3, ErrorMessage = "Name length must be min 3")]
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        [MinLength(3, ErrorMessage = "Surname length must be min 3")]
        [MaxLength(100)]
        public string Surname { get; set; } = null!;
        [MaxLength(50)]
        [MinLength(5, ErrorMessage = "username length must be min 5")]
        public string UserName { get; set; } = null!;

        [MinLength(5)]
        [MaxLength(100)]

        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Email is incorrect!")]
        public string Email { get; set; } = null!;

        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password length must be min 8")]
        [MaxLength(50)]

        public string Password { get; set; } = null!;

        [MinLength(8, ErrorMessage = "Password length must be min 8")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password mismatch!")]
        public string RepeatPassword { get; set; } = null!;




    }
}
