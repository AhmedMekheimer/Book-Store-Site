using Microsoft.EntityFrameworkCore;

namespace Online_Book_Store.Models
{
    public enum FileType
    {
        Image,
        Video
    }
    public class UploadedFile
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public FileType FileType { get; set; }

        // Nullable FKs
        public int? AuthorId { get; set; }
        public int? BookId { get; set; }
        public int? PublishingHouseId { get; set; }
        public int? CategoryId { get; set; }

        // Navigation properties
        public Author? Author { get; set; }
        public Book? Book { get; set; }
        public PublishingHouse? PublishingHouse { get; set; }
        public Category? Category { get; set; }
    }


}
