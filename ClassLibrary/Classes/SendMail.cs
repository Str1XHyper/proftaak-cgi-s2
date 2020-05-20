using System;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace ClassLibrary.Classes
{
    public class SendMail
    {
        public static async Task<bool> Execute(string subject, string toEmail, string htmlContent, string plainTextContent)
        {
            try
            {
                var client = new SendGridClient("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");
                var from = new EmailAddress("noreply@cgi.com", "CGI");
                var to = new EmailAddress(toEmail, toEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                await client.SendEmailAsync(msg);
                return true;
            } 
            catch
            {
                return false;
            }
        }
        //Password reset
        //curl --request "POST" \--url https://api.sendgrid.com/v3/mail/send \--header "Authorization: Bearer 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" \--header 'Content-Type: application/json' \--data '{"from":{"email":"noreply@cgi.com"},"personalizations":[{"to":[{"email":"bartdgp@outlook.com"}],"dynamic_template_data":{"firstName":"Bart","href":"https://bier-1.democgi.com/"}}],"template_id":"d-901d6b18be644d54864577cb680cd3be"}'
    }
}
