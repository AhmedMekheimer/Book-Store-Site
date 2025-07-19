namespace Online_Book_Store.ViewModels.Customer
{
    public class BooksSearchVM
    {
        public List<Book> Books { get; set; } = new List<Book>();
        public string? Search { get; set; }
        public int PageId { get; set; }
        public double NoPages { get; set; }
        public int Seed { get; set; }
    }
}
