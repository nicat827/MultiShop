using System.ComponentModel.DataAnnotations;

namespace MultiShop.Entities
{
    public class Settings
    {
        [Key]
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
