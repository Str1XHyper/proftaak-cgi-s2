using DAL.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Authentication
{
    public class GetAccesLevel
    {
        private GetUserAccess getUserAccess;
        public GetAccesLevel()
        {
            getUserAccess = new GetUserAccess();
        }

        public List<string> GetPermissions(string authCode)
        {
            string rol = getUserAccess.GetUserRole(authCode);
            List<string> authentication = getUserAccess.GetPermissionsForRole(rol);
            authentication.RemoveAt(0);
            return authentication;
        }

        public string GetRol(string authCode)
        {
            return getUserAccess.GetUserRole(authCode);
        }
    }
}
