﻿using System;
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
        public static async Task Execute(string subject, string toEmail, string htmlContent, string plainTextContent)
        {
            var client = new SendGridClient("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");
            var from = new EmailAddress("noreply@cgi.com", "CGI");
            var to = new EmailAddress(toEmail, toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);
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
    }

    internal class MyClass
    {
        public string[] from { get; set; } = new string[2];
        public string StringData { get; set; }
    }
}
