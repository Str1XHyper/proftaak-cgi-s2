using MySql.Data.MySqlClient;
//using RestSharp;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;

namespace ClassLibrary.Classes
{
    public class CreateLoginCookie
    {
        public static string getAuthToken(string username)
        {
            string authCode;
            string userID;
            userID = GetUserID(username);
            authCode = GetAuthCode(userID);
            return authCode;
        }

        private static string GetAuthCode(string UserID)
        {
            List<string> authCode = SQLConnection.ExecuteSearchQuery($"SELECT AuthCode FROM `Werknemers` WHERE UserId = '{UserID}'");
            if (authCode.Count == 0)
            {
                return "0";
            }
            else
            {
                return authCode[0];
            }
        }

        private static string GetUserID(string userName)
        {
            List<string> userID = SQLConnection.ExecuteSearchQuery($"SELECT UserId FROM `Login` WHERE Username = '{userName}'");
            if(userID.Count == 0)
            {
                return "0";
            }
            else
            {
                return userID[0];
            }
        }
    }
}
