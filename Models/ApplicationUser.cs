using Microsoft.AspNetCore.Identity;

namespace Online_Book_Store.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? Address { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }
}
