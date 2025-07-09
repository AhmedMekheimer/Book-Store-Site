using Microsoft.AspNetCore.Identity;

namespace Online_Book_Store.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? Address { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public override string? Email { get; set; }

        [Required]
        [MaxLength(256)]
        public override string? UserName { get; set; }
    }
}
