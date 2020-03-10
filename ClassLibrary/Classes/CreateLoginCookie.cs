using MySql.Data.MySqlClient;
//using RestSharp;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Session;


namespace ClassLibrary.Classes
{
    public class CreateLoginCookie
    {
        private string authCode;
        private SQLConnection sqlConnection = new SQLConnection();
        private string userID;
        public void CreateCookie(string username)
        {
            userID = GetUserID(username);
            authCode = GetAuthCode(userID);

            


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
