namespace MultiShop.Entities
{
    public class Slide:BaseNameableEntity
    {
        public string ImageUrl { get; set; } = null!;
        public int Order { get; set; }
        public string ButtonText { get; set; } = "Go Fashion!";
        public string? Description { get; set; }


    }
}
