using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Reset
{
    public class ResetHandler
    {
        public bool ResetPassword(string confPass, string newPassword, string authCode, string secretCode)
        {
            List<string> userIDs = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `AuthCode` = '{authCode}'");
            if (userIDs.Count > 0)
            {
                string userID = userIDs[0];
                List<string> EndDate = SQLConnection.ExecuteSearchQuery($"SELECT `EindTijd` FROM `ResetRequest` WHERE `UserID`='{userID}' AND `ResetCode`='{secretCode}'");
                if (EndDate.Count > 0)
                {
                    DateTime dagLimit = DateTime.Parse(EndDate[0]);
                    if (dagLimit > DateTime.Now)
                    {
                        if (confPass == newPassword)
                        {
                            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Login` SET `Password` = AES_ENCRYPT('{newPassword}', 'CGIKey') WHERE `UserId` = '{userID}'");
                            SQLConnection.ExecuteNonSearchQuery($"DELETE FROM `ResetRequest` WHERE `ResetCode`='{secretCode}'");
                            return true;
                        }
                        SQLConnection.ExecuteNonSearchQuery($"DELETE FROM `ResetRequest` WHERE `ResetCode`='{secretCode}'");
                    }
                }
            }
            return false;
        }
    }
}
