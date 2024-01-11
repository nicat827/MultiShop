using System.ComponentModel.DataAnnotations;

namespace MultiShop.ViewModels
{
    public class CheckoutVM
    {
        public ICollection<CheckoutItemVM> CheckoutItems { get; set; } = new List<CheckoutItemVM>();

        [MinLength(5, ErrorMessage = "Address cant be short than 5!")]
        public string Address { get; set; } = null!;

    }
    public class CheckoutItemVM
    {
        public string ProductName { get; set; } = null!;
        public decimal Subtotal { get; set; }

    }
}
