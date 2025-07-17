using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Online_Book_Store.ViewModels.Identity
{
    public class UserCreateVM
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(256)]
        public string UserName { get; set; } = null!;

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
        public string Email { get; set; } = null!;

        public bool ConfirmEmail { get; set; } = false;

        [Required]
        [DataType(DataType.Password)] 
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)] 
        [Compare("Password", ErrorMessage = "Passwords don't match.")] 
        public string ConfirmPassword { get; set; } = null!;

        [MinLength(10)]
        [MaxLength(100)]
        public string? Address { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}