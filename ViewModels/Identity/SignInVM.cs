using System.ComponentModel;

namespace Online_Book_Store.ViewModels.Identity
{
    public class SignInVM
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(256)]
        public string UserNameOrEmail { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)] 
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; }
    }
}