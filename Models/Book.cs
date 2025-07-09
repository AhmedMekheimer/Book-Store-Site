using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Online_Book_Store.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        public string Name { get; set; } = null!;
        [Range(100.0, 1_000_000.0)]
        public double Price { get; set; }
        [Range(0, 1_000)]
        public int AvailableCopies { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        [ValidateNever]
        public List<Author> Authors { get; set; } = new List<Author>();
        [ValidateNever]
        public List<PublishingHouse> PublishingHouses { get; set; } = new List<PublishingHouse>();
        [ValidateNever]
        public ICollection<BookFile> Files { get; set; } = new List<BookFile>();
    }
}
