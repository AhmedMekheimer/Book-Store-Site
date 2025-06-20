namespace Online_Book_Store.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
        public List<string> SocialMedia { get; set; } = new List<string>();
        public string Picture { get; set; } = string.Empty;
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
