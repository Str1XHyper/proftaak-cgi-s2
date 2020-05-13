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

        public bool ChangePassAuthCode(string confPass, string newPassword, string authCode)
        {
            List<string> userIDs = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `AuthCode` = '{authCode}'");
            if(userIDs.Count > 0)
            {
                string userID = userIDs[0];
                if (confPass == newPassword)
                {
                    Change(newPassword, userID);
                    return true;
                }
            }
            return false;
        }
    }
}
