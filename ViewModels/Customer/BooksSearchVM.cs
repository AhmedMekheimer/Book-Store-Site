namespace Online_Book_Store.ViewModels.Customer
{
    public class BooksSearchVM
    {
        public List<Book> Books { get; set; } = new List<Book>();
        public string Search { get; set; } = null!;
        public int CategoryId { get; set; } = 0;
        public List<Category> Categories { get; set; } = new List<Category>();
        public int PageId { get; set; } = 1;
        public double NoPages { get; set; }
    }
}
