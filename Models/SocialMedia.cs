namespace Online_Book_Store.Models
{
    public class SocialMedia
    {
        public int Id { get; set; }
        public string Platform { get; set; } = null!;
        public string Link { get; set; } = null!;
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;
    }
}
