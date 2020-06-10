using System;
using System.Collections.Generic;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Logic
{
    public class Twilio
    {
        const string accountSid = "AC25ea6272c065a8865c0350a2c11cba36";
        const string authToken = "2fa63a80bc1082e1eafa1278d2201c07";

        public static void SendWhatsapp(string body, string ToPhonenumber)
        {

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                from: new global::Twilio.Types.PhoneNumber("whatsapp:+14155238886"),
                body: body,
                to: new global::Twilio.Types.PhoneNumber($"whatsapp:{ToPhonenumber}")
            );
        }
        public static void SendSMS(string body, string ToPhonenumber)
        {
            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                from: new global::Twilio.Types.PhoneNumber("+12057827560"),
                messagingServiceSid: "MG1a145514ca819b1104e2458842100de1",
                body: body,
                to: new global::Twilio.Types.PhoneNumber(ToPhonenumber)
            );
        }
    }
}
