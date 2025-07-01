namespace Online_Book_Store.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public double Price { get; set; }
        public int AvailableCopies { get; set; }
        public int CategoryId { get; set; }
        public List<Author> Authors { get; set; } = new List<Author>();
        public List<PublishingHouse> PublishingHouses { get; set; } = new List<PublishingHouse>();
        public ICollection<BookFile> Files { get; set; } = new List<BookFile>();
    }
}
