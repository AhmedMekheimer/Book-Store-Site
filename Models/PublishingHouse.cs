namespace Online_Book_Store.Models
{
    public class PublishingHouse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
