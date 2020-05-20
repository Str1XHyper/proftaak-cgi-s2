using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Classes
{
    public class ChangePasswordFunc
    {
        public bool ChangePass(string currentPassword, string newPassword, string userID)
        {
            string currectDBPassword = SQLConnection.ExecuteGetStringQuery($"SELECT AES_DECRYPT(Password,'CGIKey')  FROM `Login` WHERE UserId = '{userID}'")[0];
            if(currentPassword == currectDBPassword)
            {
                Change(newPassword, userID);
                return true;
            }
            return false;
        }

        private void Change(string newPassword, string userID)
        {
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Login` SET `Password` = AES_ENCRYPT('{newPassword}', 'CGIKey') WHERE `UserId` = '{userID}'");
        }

        public bool ChangePassAuthCode(string confPass, string newPassword, string authCode, string secretCode)
        {
            List<string> userIDs = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `AuthCode` = '{authCode}'");
            if(userIDs.Count > 0)
            {
                string userID = userIDs[0];
                List<string> EndDate = SQLConnection.ExecuteSearchQuery($"SELECT `EindTijd` FROM `ResetRequest` WHERE `UserID`='{userID}' AND `ResetCode`='{secretCode}'");
                if(EndDate.Count > 0)
                {
                    DateTime dagLimit = DateTime.Parse(EndDate[0]);
                    if(dagLimit > DateTime.Now)
                    {
                        if (confPass == newPassword)
                        {
                            Change(newPassword, userID);
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
