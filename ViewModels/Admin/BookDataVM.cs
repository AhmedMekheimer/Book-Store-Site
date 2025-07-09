namespace Online_Book_Store.ViewModels.Admin
{
    public class BookDataVM
    {
        public Book? Book { get; set; }
        public List<int> AuthorsIds { get; set; } = new List<int>();
        public List<int> PublishersIds { get; set; } = new List<int>();
    }
}
