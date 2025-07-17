using Microsoft.AspNetCore.Identity;

namespace Online_Book_Store.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        [MinLength(3)]
        [MaxLength(256)]
        public override string UserName { get; set; } = null!;

        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string FirstName { get; set; } = null!;
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public override string Email { get; set; } = null!;

        [MinLength(10)]
        [MaxLength(100)]
        public string? Address { get; set; }
    }
}
