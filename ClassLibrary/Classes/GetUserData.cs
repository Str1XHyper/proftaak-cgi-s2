using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary.Classes
{
    public class GetUserData : IGetUserData
    {
        public string RoleNameAuth(string authcode)
        {
            if (authcode != null)
            {
                string[] authResponse;
                authResponse = SQLConnection.ExecuteSearchQuery($"SELECT `Rol` FROM `Werknemers` WHERE `AuthCode` = '{authcode}'").ToArray();
                if (authResponse.Length > 0) return authResponse[0];
                else return string.Empty;
            }
            else
            {
                return "nietIngelogd";
            }
        }

        public string RoleNameID(string userID)
        {
            if (userID != null)
            {
                string[] idResponse;
                idResponse = SQLConnection.ExecuteSearchQuery($"SELECT `Rol` FROM `Werknemers` WHERE `UserId` = '{userID}'").ToArray();
                if (idResponse.Length > 0) return idResponse[0];
                else return string.Empty;
            }
            else
            {
                return "geenUserID";
            }
        }

        public string UserIDAuth(string authcode)
        {
            if (authcode != null)
            {
                string[] authResponse;
                authResponse = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `AuthCode` = '{authcode}'").ToArray();
                if (authResponse.Length > 0) return authResponse[0];
                else return string.Empty;
            }
            else
            {
                return "geenAuthCode";
            }
        }

        public static string UserNameAuth(string authCode)
        {

            if (authCode != null)
            {
                List<string> names = SQLConnection.ExecuteSearchQuery($"SELECT `Voornaam`, `Tussenvoegsel`, `Achternaam` FROM `Werknemers` WHERE `AuthCode` = '{authCode}'");
                string name = names[0] + " " + names[1] + " " + names[2];
                return name;
            }
            else
            {
                return "UserNietGevonden";
            }
        }
    }
}
