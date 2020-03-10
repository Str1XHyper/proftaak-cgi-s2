using MySql.Data.MySqlClient;
using RestSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Net;
using Microsoft.AspNetCore.Http.Features.Authentication;
using System.Web;

namespace ClassLibrary.Classes
{
    public class CreateLoginCookie
    {
        IFeatureCollection keyValuePairs;
        private string authCode;
        private SQLConnection sqlConnection = new SQLConnection();
        private string userID;
        public void CreateCookie(string username)
        {
            userID = GetUserID(username);
            authCode = GetAuthCode(userID);

            HttpCookie newCookie = new HttpCookie("UserInfo");
            newCookie.Domain = "cgi.com";
            newCookie.Expires = DateTime.Now.AddDays(1);
            newCookie.Name = "UserInfo";
            newCookie.Path = "/";
            newCookie.Secure = false;
            newCookie.Value = authCode;


            //ResponseCookiesFeature responseCookiesFeature = new ResponseCookiesFeature(keyValuePairs);
            //responseCookiesFeature.Cookies.Append("Authentication", authCode);
            //CookieOptions option = new CookieOptions();
            //ResponseCookiesFeature.Cookies.Append("UserAuth", authCode, option);
            //IRequestCookieCollection pairs = new IRequestCookieCollection();
            
            

            //HttpCookie cookie = new HttpCookie();
            //cookie.Name = "UserInfo";
            //cookie.Value = authCode;
            //cookie.Domain = "https://www.cgi.com";
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
