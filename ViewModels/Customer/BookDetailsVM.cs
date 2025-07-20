namespace Online_Book_Store.ViewModels.Customer
{
    public class BookDetailsVM
    {
        public Book Book { get; set; } = new Book();
        public List<Book> RelatedBooks { get; set; } = new List<Book>();
    }
}
