using System;

namespace ClassLibrary.Classes
{
    public class AddLoginLog
    {
        public  void NewLogin(string authCode, bool succes, string IP, string tijd)
        {
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO `Logins`(`AuthCode`, `Succes`, `Tijd`, `IP`) VALUES ('{authCode}',{succes},'{tijd}','{IP}')");
        }
    }
}
