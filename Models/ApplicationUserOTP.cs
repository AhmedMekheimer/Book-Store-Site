namespace Online_Book_Store.Models
{
    public class ApplicationUserOTP
    {
        public int Id { get; set; }
        public int OTPNumber { get; set; }
        public DateTime SendDate { get; set; }
        public bool Status { get; set; }
        public DateTime ValidTo { get; set; }
        public string? Reason { get; set; }

        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}
