using System.Collections.Generic;

namespace DAL.Authentication
{
    public class GetUserAccess
    {
        public string GetUserRole(string authCode)
        {
            return SQLConnection.ExecuteSearchQuery($"SELECT `Rol` FROM `Werknemers` WHERE `AuthCode` = '{authCode}'")[0];
        }

        public List<string> GetPermissionsForRole(string role)
        {
            return SQLConnection.ExecuteSearchQuery($"SELECT * FROM `Rollen` WHERE `Naam` = '{role}'");
        }
    }
}
