namespace Online_Book_Store.ViewModels.Identity
{
    public class UserEditVM
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

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

        [MinLength(10)]
        [MaxLength(100)]
        public string? Address { get; set; }
    }
}
