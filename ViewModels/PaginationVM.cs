namespace MultiShop.ViewModels
{
    public class PaginationVM<T> where T : class, new()
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();

        public int CurrentPage { get; set; }    

        public int TotalPages { get; set; }
    }
}
