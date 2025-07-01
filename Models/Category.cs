namespace Online_Book_Store.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<Book> Books { get; set; } = new List<Book>();
        public ICollection<CategoryFile> Files { get; set; } = new List<CategoryFile>();
    }
}
