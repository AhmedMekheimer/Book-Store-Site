namespace Online_Book_Store.ViewModels.Admin
{
    public class BookCatAuthPubsVM
    {
        public Book Book { get; set; } = null!;
        public IEnumerable<Category> Categories { get; set; } = null!;
        public IEnumerable<Author> Authors { get; set; } = null!;
        public IEnumerable<PublishingHouse> PublishingHouses { get; set; } = null!;
    }
}
