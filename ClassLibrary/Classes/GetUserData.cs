using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Classes
{
    public class GetUserData
    {
        public static string RoleNameAuth(string authcode)
        {
            string[] authResponse;
            authResponse = SQLConnection.ExecuteSearchQuery($"SELECT `Rol` FROM `Werknemers` WHERE `AuthCode` = '{authcode}'").ToArray();
            return authResponse[0];
        }

        public static string RoleNameID(string userID)
        {
            string[] idResponse;
            idResponse = SQLConnection.ExecuteSearchQuery($"SELECT `Rol` FROM `Werknemers` WHERE `UserId` = '{userID}'").ToArray();
            return idResponse[0];
        }

        public static string UserIDAuth(string authcode)
        {
            string[] authResponse;
            authResponse = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `AuthCode` = '{authcode}'").ToArray();
            return authResponse[0];
        }
    }
}
