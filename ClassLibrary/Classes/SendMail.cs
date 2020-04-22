using System;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace ClassLibrary.Classes
{
    public class SendMail
    {
        public static async Task Execute(string subject, string toEmail, string htmlContent, string plainTextContent)
        {
            var client = new SendGridClient("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");
            var from = new EmailAddress("noreply@cgi.com", "CGI");
            var to = new EmailAddress(toEmail, toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);
        }
    }
}
