using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary.Classes
{
    public class PasswordResetHandler
    {
        public void AddPasswordReset(string email)
        {
            string customCode = GenerateAuthToken.GetUniqueKeyOriginal_BIASED(25);
            List<string> userID = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `Email`='{email.ToLower()}'");
            if(userID.Count > 0)
            {
                string datumPlusVijf = DateTime.Now.AddMinutes(5).ToString("yyyy-MM-dd HH:mm:ss");
                SQLConnection.ExecuteNonSearchQuery($"INSERT INTO `ResetRequest` VALUES ('{userID[0]}', '{datumPlusVijf}', '{customCode}')");
                List<string[]> response = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT `Voornaam`, `Tussenvoegsel`, `Achternaam`, `AuthCode` FROM `Werknemers` WHERE `Email`='{email.ToLower()}'");
                string name = $"{response[0][0]} {response[0][1]} {response[0][2]}";
                SendMail.SendReset(email, name, response[0][0], response[0][3], customCode);
            }
        }
    }
}
