using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Classes
{
    public class GetUserData
    {
        public static string RoleNameAuth(string authcode)
        {
            if (authcode != null)
            {
                string[] authResponse;
                authResponse = SQLConnection.ExecuteSearchQuery($"SELECT `Rol` FROM `Werknemers` WHERE `AuthCode` = '{authcode}'").ToArray();
                return authResponse[0];
            }
            else
            {
                return "nietIngelogd";
            }
        }

        public static string RoleNameID(string userID)
        {
            if(userID != null)
            {
                string[] idResponse;
                idResponse = SQLConnection.ExecuteSearchQuery($"SELECT `Rol` FROM `Werknemers` WHERE `UserId` = '{userID}'").ToArray();
                return idResponse[0];
            }
            else
            {
                return "geenUserID";
            }
        }

        public static string UserIDAuth(string authcode)
        {
            if(authcode != null)
            {
                string[] authResponse;
                authResponse = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `AuthCode` = '{authcode}'").ToArray();
                return authResponse[0];
            }
            else
            {
                return "geenAuthCode";
            }
        }
    }
}
