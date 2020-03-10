using MySql.Data.MySqlClient;
//using RestSharp;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ClassLibrary.Classes
{
    public class CreateLoginCookie
    {
        private string authCode;
        private SQLConnection sqlConnection = new SQLConnection();
        private string userID;
        public string getAuthToken(string username)
        {
            userID = GetUserID(username);
            authCode = GetAuthCode(userID);
            return authCode;
        }

        private string GetAuthCode(string UserID)
        {
            string[] authCode = sqlConnection.ExecuteSearchQuery($"SELECT AuthCode FROM `Werknemers` WHERE UserId = '{UserID}'");
            return authCode[0];
        }

        private string GetUserID(string userName)
        {
            string[] userID = sqlConnection.ExecuteSearchQuery($"SELECT UserId FROM `Login` WHERE Username = '{userName}'");
            return userID[0];
        }
    }
}
