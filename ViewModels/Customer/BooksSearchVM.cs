namespace Online_Book_Store.ViewModels.Customer
{
    public class BooksSearchVM
    {
        public IEnumerable<Book> Books { get; set; } = new List<Book>();
        public string? Search { get; set; }
        public int PageId { get; set; }
        public double NoPages { get; set; }
    }
}
