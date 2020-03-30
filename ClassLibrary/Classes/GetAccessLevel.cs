using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Classes
{
    public class GetAccessLevel
    {
        public static List<string> GetPermissions(string authCode)
        {
            string rol = SQLConnection.ExecuteSearchQuery($"SELECT `Rol` FROM `Werknemers` WHERE `AuthCode` = '{authCode}'")[0];
            List<string> authentication = SQLConnection.ExecuteSearchQuery($"SELECT * FROM `Rollen` WHERE `Naam` = '{rol}'");
            authentication.RemoveAt(0);
            return authentication;
        }
    }
}
