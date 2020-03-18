using System;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace ClassLibrary.Classes
{
    public class SendMail
    {
        public static async Task Execute(string frommail, string tomail, string subjectmail, string message, string htmlMessage)
        {
            var client = new SendGridClient("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");
            var from = new EmailAddress(frommail, frommail);
            var subject = subjectmail;
            var to = new EmailAddress(tomail, tomail);
            var plainTextContent = message;
            var htmlContent = htmlMessage;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
