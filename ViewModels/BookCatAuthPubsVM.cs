namespace Online_Book_Store.ViewModels
{
    public class BookCatAuthPubsVM
    {
        public Book Book { get; set; } = null!;
        public List<Category> Categories { get; set; } = null!;
        public List<Author> Authors { get; set; } = null!;
        public List<PublishingHouse> PublishingHouses { get; set; } = null!;
    }
}
