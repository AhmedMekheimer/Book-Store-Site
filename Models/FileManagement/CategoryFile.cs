namespace Online_Book_Store.Models
{
    public class CategoryFile : FileEntity
    {
        public int? CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
