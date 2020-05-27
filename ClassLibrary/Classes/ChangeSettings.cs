using System;
using System.Collections.Generic;
using System.Text;
using ClassLibrary.Classes;
using Models.Settings;

namespace ClassLibrary.Classes
{
    public class ChangeSettings
    {
        public static int InitSettings(string email, string emailSetting, string smssSetting, string whatsAppSetting)
        {
            int emailSettings;
            int smsSettings;
            List<string> userIDlist = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `Email` = '{email}'");
            int userID = Convert.ToInt32(userIDlist[0]);
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `ReceiveMail`='{(Convert.ToBoolean(emailSetting) ? 1 : 0)}',`ReceiveSMS`='{(Convert.ToBoolean(smssSetting) ? 1 : 0)}',`ReceiveWhatsApp`='{(Convert.ToBoolean(whatsAppSetting) ? 1 : 0)}' WHERE `UserId` = {userID}");
            return userID;
        }

        public static string[] getSettings(string userID)
        {
            return SQLConnection.ExecuteSearchQuery($"SELECT * FROM `Settings` WHERE `UserId` = {userID}").ToArray();
        }

        public void SetPasswordSettings(List<string> values)
        {
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `PasswordRequirements` SET `NumberRequired` = '{values[0]}', `SpecialCharRequired` = '{values[1]}', `UpperRequired` = '{values[2]}', `LowerRequired` = '{values[3]}', `MinimumLength` = '{values[4]}'");
        }
    }
}
