using System;
using System.IO;
using System.Net;

namespace ClassLibrary.Classes
{
    public class AddLoginLog
    {
        public void NewLogin(string authCode, bool succes, string ip, string tijd)
        {
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO `Logins`(`AuthCode`, `Succes`, `Tijd`, `IP`) VALUES ('{authCode}',{succes},'{tijd}','{ip}')");
            //SendMail.Help();
        }

        public void UpdateLogin(string authcode, string tijd, string IP)
        {
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Logins` SET `IP` = '{IP}' WHERE `AuthCode` = '{authcode}' AND `Tijd` = '{tijd}'");
        }

        public string CallUrl(string url)
        {
            WebRequest request = HttpWebRequest.Create(url);
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string urlText = reader.ReadToEnd(); // it takes the response from your url. now you can use as your need  
            return urlText;
        }
    }
}
