using Microsoft.EntityFrameworkCore;

namespace Online_Book_Store.Models
{
    public enum FileType
    {
        Image,
        Video
    }
    public abstract class FileEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public FileType FileType { get; set; }
    }
}
