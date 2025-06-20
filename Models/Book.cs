namespace Online_Book_Store.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public double Price { get; set; }
        public string Picture { get; set; } = string.Empty;
        public int AvailableCopies { get; set; }
        public int CategoryId { get; set; }
        public int PublishingHouseId { get; set; }
    }
}
