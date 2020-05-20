using System;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using RestSharp;
using RestSharp.Authenticators;

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

        public static async Task Test()
        {
            var client = new RestClient("https://api.sendgrid.com/v3/mail/send");
            client.Authenticator = new HttpBasicAuthenticator("username", "password");

            var request = new RestRequest("statuses/home_timeline.json", DataFormat.Json);
            string[] test = { "email", "noreply@cgi.com" };
            var param = new MyClass { from = test, StringData = "test123" };
            request.AddJsonBody(param);
            var response = client.Get(request);
        }
        //{"from":{"email":"noreply@cgi.com"},"personalizations":[{"to":[{"email":"bartdgp@outlook.com"}],"dynamic_template_data":{"firstName":"Bart","href":"https://bier-1.democgi.com/"}}],"template_id":"d-901d6b18be644d54864577cb680cd3be"}

        public static void Help()
        {
            var client = new RestClient("https://api.sendgrid.com/v3/mail/send");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer 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");
            request.AddParameter("application/json", "{\"personalizations\":[{\"to\":[{\"email\":\"alex.peek@hotmail.com\",\"name\":\"Bart Vermeulen\"}],\"dynamic_template_data\":{\"firstName\":\"<3 groetjes bart\",\"href\":\"https://bier-1.democgi.com\"},\"subject\":\"Wachtwoord reset\"}],\"from\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"reply_to\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"template_id\":\"d-901d6b18be644d54864577cb680cd3be\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }
    }
    internal class MyClass
    {
        public string[] from { get; set; } = new string[2];
        public string StringData { get; set; }
    }
}
