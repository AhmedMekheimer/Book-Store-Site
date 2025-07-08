using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace Online_Book_Store.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("dodoxdmekheimer@gmail.com", "iytt ugkf erjo xjdu")
            };

            return client.SendMailAsync(
                new MailMessage(
                    from: "dodoxdmekheimer@gmail.com",
                    to: email,
                    subject,
                    htmlMessage
                    )
                {
                    IsBodyHtml = true
                }
                );
        }
    }
}
