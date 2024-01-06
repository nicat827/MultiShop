namespace MultiShop.Utilities.Extencions
{
    public static class DataHelper
    {
        public static string Capitalize(this string str )
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower();
        }
        public static void CheckPositiveNum(this int id)
        {
            if (id <= 0) throw new Exception("Invalid request!");
        }
        public static void CheckNull<T>(this T entity)
        {
            if (entity is null) throw new NotFoundException($"{nameof(T)} doesn't exist!");
        }
    }
}
