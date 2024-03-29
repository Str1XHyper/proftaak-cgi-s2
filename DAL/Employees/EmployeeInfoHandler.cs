﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Employees
{
    public class EmployeeInfoHandler
    {
        public string getAuthToken(string username) => GetAuthCode(GetUserID(username));

        private string GetAuthCode(string userID)
        {
            List<string> authCode = SQLConnection.ExecuteSearchQuery($"SELECT AuthCode FROM `Werknemers` WHERE UserId = '{userID}'");
            if (authCode.Count > 0) return authCode[0];
            else return "0";
        }

        private string GetUserID(string userName)
        {
            List<string> userID = SQLConnection.ExecuteSearchQuery($"SELECT UserId FROM `Login` WHERE Username = '{userName}'");
            if (userID.Count > 0) return userID[0];
            else return "0";
        }
        public string[] GetEmployeeNotificationsSettings(string userID) => SQLConnection.ExecuteSearchQuery($"SELECT * FROM `Settings` WHERE `UserId` = {userID}").ToArray();
        public void SetPasswordSettings(List<string> values) => SQLConnection.ExecuteNonSearchQuery($"UPDATE `PasswordRequirements` SET `NumberRequired` = '{values[0]}', `SpecialCharRequired` = '{values[1]}', `UpperRequired` = '{values[2]}', `LowerRequired` = '{values[3]}', `MinimumLength` = '{values[4]}'");
        public int SetSettingsAndReturnUserID(string email, int emailSetting, int smssSetting, int whatsAppSetting)
        {
            List<string> userIDlist = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `Email` = '{email}'");
            int userID = Convert.ToInt32(userIDlist[0]);
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO `Settings` (ReceiveMail, ReceiveSMS, ReceiveWhatsApp, UserID) VALUES('{emailSetting}', '{smssSetting}', '{whatsAppSetting}', {userID}) " );
            return userID;
        }
        
        public string RolFromAuth(string authcode)
        {
            string[] authResponse = SQLConnection.ExecuteSearchQuery($"SELECT `Rol` FROM `Werknemers` WHERE `AuthCode` = '{authcode}'").ToArray();
            if (authResponse.Length > 0) return authResponse[0];
            else return string.Empty;
        }

        public string RolFromID(string userID)
        {
            string[] idResponse = SQLConnection.ExecuteSearchQuery($"SELECT `Rol` FROM `Werknemers` WHERE `UserId` = '{userID}'").ToArray();
            if (idResponse.Length > 0) return idResponse[0];
            else return string.Empty;
        }

        public int IDFromAuth(string authcode)
        {
            string[] authResponse = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `AuthCode` = '{authcode}'").ToArray();
            if (authResponse.Length > 0) return Convert.ToInt32(authResponse[0]);
            else return 0;
        }

        public List<string> EmployeeInfo(string authCode) => SQLConnection.ExecuteSearchQuery($"SELECT * FROM `Werknemers` WHERE `AuthCode` = '{authCode}'");
        public int UserIDFromMail(string mail)
        {
            List<string> userIDs = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `email`='{mail}'");
            if (userIDs.Count > 0) return Convert.ToInt32(userIDs[0]);
            else return 0;
        }
        public bool UniqueEmail(string email)
        {
            List<string[]> response = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT Email FROM Werknemers WHERE Email='{email}'");
            if (response.Count > 0)
            {
                return false;
            }
            return true;
        }
    }
}
