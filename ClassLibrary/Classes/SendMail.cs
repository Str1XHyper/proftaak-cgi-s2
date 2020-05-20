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

        public static void Help(string toEmail, string toName, string firstName, string authCode, string resetCode)
        {
            var client = new RestClient("https://api.sendgrid.com/v3/mail/send");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer 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");
            request.AddParameter("application/json", "{\"personalizations\":[{\"to\":[{\"email\":\"" + toEmail + "\",\"name\":\"" + toName + "\"}],\"dynamic_template_data\":{\"firstName\":\"" + firstName + "\",\"href\":\"https://bier-1.democgi.com/Account/PasswordChange?authCode=" + authCode + "&code=" + resetCode + "\"},\"subject\":\"Wachtwoord reset\"}],\"from\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"reply_to\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"template_id\":\"d-901d6b18be644d54864577cb680cd3be\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        public static void SendReset(string email)
        {
            List<string[]> info = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT `Voornaam`, `Tussenvoegsel`, `Achternaam`, `Authcode` FROM `Werknemers` WHERE `Email`='{email.ToLower()}'");
            if(info.Count > 0)
            {
                string fullName = string.Empty;
                if(info[0][1] != string.Empty)
                {
                    fullName = info[0][0] + " " + info[0][1] + " " + info[0][2];
                }
                else
                {
                    fullName = info[0][0] + " " + info[0][2];
                }
                //Help(email, fullName, info[0][0], info[0][3]);
            }
        }
    }
}
