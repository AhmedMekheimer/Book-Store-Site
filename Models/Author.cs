namespace Online_Book_Store.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
        public string Picture { get; set; } = string.Empty;
        public List<string>? SocialMedias { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
