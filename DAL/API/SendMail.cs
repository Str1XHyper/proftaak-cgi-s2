using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.API
{
    public class SendMail
    {
        public static void SendReset(string toEmail, string toName, string firstName, string authCode, string resetCode)
        {
            var client = new RestClient("https://api.sendgrid.com/v3/mail/send");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer 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");
            request.AddParameter("application/json", "{\"personalizations\":[{\"to\":[{\"email\":\"" + toEmail + "\",\"name\":\"" + toName + "\"}],\"dynamic_template_data\":{\"firstName\":\"" + firstName + "\",\"href\":\"https://bier-1.democgi.com/Account/PasswordChange?authCode=" + authCode + "&code=" + resetCode + "\"},\"subject\":\"Wachtwoord reset\"}],\"from\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"reply_to\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"template_id\":\"d-901d6b18be644d54864577cb680cd3be\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        public static void SendNotification(string toEmail, string toName, string firstName, string titel, string beschrijving, string functie)
        {
            var client = new RestClient("https://api.sendgrid.com/v3/mail/send");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer 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");
            request.AddParameter("application/json", "{\"personalizations\":[{\"to\":[{\"email\":\"" + toEmail + "\",\"name\":\"" + toName + "\"}],\"dynamic_template_data\":{\"firstName\":\"" + firstName + "\",\"titel\":\"" + titel + "\",\"beschrijving\":\"" + beschrijving + "\",\"functie\":\"" + functie + "\"},\"subject\":\"Wachtwoord reset\"}],\"from\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"reply_to\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"template_id\":\"d-3140b8fa87aa48dcb85e16694f841293\"}", ParameterType.RequestBody); ;
            IRestResponse response = client.Execute(request);
        }

        public static void SendKlantIncident(string toEmail, string toName, string incidentmsg, string incidentTitel)
        {
            var client = new RestClient("https://api.sendgrid.com/v3/mail/send");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer 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");
            request.AddParameter("application/json", "{\"personalizations\":[{\"to\":[{\"email\":\"" + toEmail + "\",\"name\":\"" + toName + "\"}],\"dynamic_template_data\":{\"klantNaam\":\"" + toName + "\",\"incidentMsg\":\"" + incidentmsg + "\",\"incidentTitle\":\"" + incidentTitel + "\",\"href\":\"https://bier-1.democgi.com/Account/PasswordChange?authCode=" + 5 + "&code=" + 5 + "\"},\"subject\":\"Incident gemeld\"}],\"from\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"reply_to\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"template_id\":\"d-db923082ba2d4fb784fd24072101feeb\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        public static void SendEmployeeIncident(string toEmail, string toName, string incidentmsg, string incidentTitel)
        {
            var client = new RestClient("https://api.sendgrid.com/v3/mail/send");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer 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");
            request.AddParameter("application/json", "{\"personalizations\":[{\"to\":[{\"email\":\"" + toEmail + "\",\"name\":\"" + toName + "\"}],\"dynamic_template_data\":{\"klantNaam\":\"" + toName + "\",\"incidentMsg\":\"" + incidentmsg + "\",\"incidentTitle\":\"" + incidentTitel + "\",\"href\":\"https://bier-1.democgi.com/Account/PasswordChange?authCode=" + 5 + "&code=" + 5 + "\"},\"subject\":\"Incident gemeld\"}],\"from\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"reply_to\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"template_id\":\"d-2c865786caef4aa3b94a752572556103\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        public static void SendEmployeeIncidentSolved(string toEmail, string toName, string incidentmsg, string incidentTitel)
        {
            var client = new RestClient("https://api.sendgrid.com/v3/mail/send");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer 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");
            request.AddParameter("application/json", "{\"personalizations\":[{\"to\":[{\"email\":\"" + toEmail + "\",\"name\":\"" + toName + "\"}],\"dynamic_template_data\":{\"klantNaam\":\"" + toName + "\",\"incidentMsg\":\"" + incidentmsg + "\",\"incidentTitle\":\"" + incidentTitel + "\",\"href\":\"https://bier-1.democgi.com/Account/PasswordChange?authCode=" + 5 + "&code=" + 5 + "\"},\"subject\":\"Incident gemeld\"}],\"from\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"reply_to\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"template_id\":\"d-bc568bca3ef74ce1b903390f43093105 \"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        public static void SendKlantIncidentSolved(string toEmail, string toName, string incidentmsg, string incidentTitel)
        {
            var client = new RestClient("https://api.sendgrid.com/v3/mail/send");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddHeader("authorization", "Bearer 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");
            request.AddParameter("application/json", "{\"personalizations\":[{\"to\":[{\"email\":\"" + toEmail + "\",\"name\":\"" + toName + "\"}],\"dynamic_template_data\":{\"klantNaam\":\"" + toName + "\",\"incidentMsg\":\"" + incidentmsg + "\",\"incidentTitle\":\"" + incidentTitel + "\",\"href\":\"https://bier-1.democgi.com/Account/PasswordChange?authCode=" + 5 + "&code=" + 5 + "\"},\"subject\":\"Incident gemeld\"}],\"from\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"reply_to\":{\"email\":\"noreply@cgi.com\",\"name\":\"CGI\"},\"template_id\":\"d-c5db6e3c0a1d4872869f067c598023e9\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }
    }
}
