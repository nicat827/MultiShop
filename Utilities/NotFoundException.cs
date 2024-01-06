namespace MultiShop.Utilities
{
    public class NotFoundException:Exception
    {
        public NotFoundException(string mess = "Not Found"):base(mess) { }
       
    }
}
