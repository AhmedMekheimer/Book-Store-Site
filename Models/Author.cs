namespace Online_Book_Store.Models
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        public string Name { get; set; } = null!;
        [MinLength(20)]
        [MaxLength(1000)]
        public string? Bio { get; set; }
        [ValidateNever]
        public List<string> SocialMedias { get; set; } = new List<string>();
        [ValidateNever]
        public List<Book> Books { get; set; } = new List<Book>();
        [ValidateNever]
        public ICollection<AuthorFile> Files { get; set; } = new List<AuthorFile>();
    }
}
