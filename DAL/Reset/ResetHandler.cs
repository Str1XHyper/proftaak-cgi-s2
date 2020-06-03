using ClassLibrary.Classes;
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

        public void SendReset(string email, string customCode, int userID)
        {
            string datumPlusVijf = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-dd HH:mm:ss");
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO `ResetRequest` VALUES ('{userID}', '{datumPlusVijf}', '{customCode}')");
            List<string[]> response = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT `Voornaam`, `Tussenvoegsel`, `Achternaam`, `AuthCode` FROM `Werknemers` WHERE `Email`='{email.ToLower()}'");
            string name = $"{response[0][0]} {response[0][1]} {response[0][2]}";
            SendMail.SendReset(email, name, response[0][0], response[0][3], customCode);
        }
    }
}
