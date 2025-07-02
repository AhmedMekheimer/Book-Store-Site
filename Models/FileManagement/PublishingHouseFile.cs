namespace Online_Book_Store.Models
{
    public class PublishingHouseFile : FileEntity
    {
        public int? PublishingHouseId { get; set; }
        public PublishingHouse PublishingHouse { get; set; } = null!;
    }
}
