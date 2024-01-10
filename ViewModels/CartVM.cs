namespace MultiShop.ViewModels
{
    public class CartVM
    {
        public ICollection<BasketItemVM> BasketItems { get; set; }= new List<BasketItemVM>();
    }
}
