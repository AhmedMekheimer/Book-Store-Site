using System.ComponentModel;

namespace Online_Book_Store.ViewModels.Identity
{
    public class EmailConfirmationVM
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(256)]
        public string UserNameOrEmail { get; set; } = null!;
    }
}