namespace Online_Book_Store.ViewModels.Customer
{
    public class AuthorSearchVM
    {
        public List<Author> Authors { get; set; } = new List<Author>();
        public string Search { get; set; } = null!;
        public int PageId { get; set; } = 1;
        public int NoPages { get; set; }
    }
}
