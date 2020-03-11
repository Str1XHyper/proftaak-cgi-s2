using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Classes
{
    public class GetRole
    {
        SQLConnection sQLConnection = new SQLConnection();
        private string[] authResponse;
        private string[] idResponse;
        public string RoleNameAuth(string authcode)
        {
            authResponse = sQLConnection.ExecuteSearchQuery($"SELECT `Rol` FROM `Werknemers` WHERE `AuthCode` = '{authcode}'").ToArray();
            return authResponse[0];
        }

        public string RoleNameID(string userID)
        {
            idResponse = sQLConnection.ExecuteSearchQuery($"SELECT `Rol` FROM `Werknemers` WHERE `UserId` = '{userID}'").ToArray();
            return idResponse[0];
        }
    }
}
