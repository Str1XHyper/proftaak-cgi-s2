﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Authentication.Login
{
    public class LoginHandler
    {
        public enum responses { redirectHome, wrongEntry, multipleEntries, massiveError };

        public void AddLogin(string naam, string ID, string email)
        {
            string password = naam + "WW";
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO Login(UserId, Username, Password) VALUES( '{ID}', '{email.ToLower()}', AES_ENCRYPT('{password}', 'CGIKey'))");
        }

        public void ChangeLoginAdmin(string email, string password)
        {
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Login` SET `Password` = AES_ENCRYPT('{password}', 'CGIKey') WHERE `Username` = '{email.ToLower()}'");
        }

        public void NewLoginRecord(string authCode, bool succes, string ip, string tijd)
        {
            SQLConnection.ExecuteNonSearchQuery($"INSERT INTO `Logins`(`AuthCode`, `Succes`, `Tijd`, `IP`) VALUES ('{authCode}',{succes},'{tijd}','{ip}')");
        }

        public bool ChangePassword(string currentPassword, string newPassword, string userID)
        {
            string currectDBPassword = SQLConnection.ExecuteGetStringQuery($"SELECT AES_DECRYPT(Password,'CGIKey')  FROM `Login` WHERE UserId = '{userID}'")[0];
            if (currentPassword == currectDBPassword)
            {
                SQLConnection.ExecuteNonSearchQuery($"UPDATE `Login` SET `Password` = AES_ENCRYPT('{newPassword}', 'CGIKey') WHERE `UserId` = '{userID}'");
                return true;
            }
            return false;
        }

        public string UnencryptedPassword(string username)
        {
            List<string> passwords = SQLConnection.ExecuteGetStringQuery($"SELECT AES_DECRYPT(Password,'CGIKey')  FROM `Login` WHERE Username='{username.ToLower()}'");
            if (passwords.Count > 0) return passwords[0];
            else return string.Empty;
        }
    }
}
