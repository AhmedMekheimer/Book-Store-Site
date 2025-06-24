namespace Online_Book_Store.ViewModels
{
    public class EditBookVM
    {
        public Book book { get; set; } = new Book();
        public BooksCatAuthPubsVM BooksCatAuthPubsVM { get; set; } = new BooksCatAuthPubsVM();
    }
}
