namespace Online_Book_Store.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string Picture { get; set; } = string.Empty;
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
