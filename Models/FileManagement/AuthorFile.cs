namespace Online_Book_Store.Models
{
    public class AuthorFile : FileEntity
    {
        public int? AuthorId { get; set; }
        public Author Author { get; set; } = null!;
    }
}
