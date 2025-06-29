namespace Online_Book_Store.ViewModels
{
    public class BookDataVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public double Price { get; set; }
        public string Picture { get; set; } = string.Empty;
        public int AvailableCopies { get; set; }
        public int CategoryId { get; set; }
        public List<int> AuthorsIds { get; set; } = new List<int>();
        public List<int> PublishersIds { get; set; } = new List<int>();
    }
}
