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
    }
}
