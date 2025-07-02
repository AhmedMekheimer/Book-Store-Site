namespace Online_Book_Store.Models
{
    public class BookFile : FileEntity
    {
        public int? BookId { get; set; }
        public Book Book { get; set; } = null!;
    }

}
