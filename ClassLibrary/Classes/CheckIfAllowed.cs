using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Classes
{
    public class CheckIfAllowed
    {
        public static bool IsAllowed(string authCode, string page)
        {
            List<string> r = SQLConnection.ExecuteSearchQuery($"SELECT `Rol` FROM `Werknemers` WHERE `AuthCode` = '{authCode}'");
            if (r.Count > 0)
            {
                List<string> rr = SQLConnection.ExecuteSearchQuery($"SELECT `{page}` FROM `Rollen` WHERE `Naam` = '{r[0]}'");
                if (rr.Count > 0)
                {
                    if (rr[0] == "True" || rr[0] == "1") return true;
                    else return false;
                }
                else return false;
            }
            else return false;
        }
    }
}
