﻿using System;
using ClassLibrary.Classes;
using MySql.Data.MySqlClient;

namespace ClassLibrary
{
    public class LoginClass
    {
        private SQLConnection sqlConnection = new SQLConnection();
        private string sql;
        private string[] passwords;
        private string[] usernames;
        private enum responses { redirectHome, wrongEntry, multipleEntries, massiveError};
        public Enum LoginUserFunction(string userName, string password)
        {
            sql = $"SELECT `Username` FROM `Login` WHERE Username='{userName.ToLower()}'";
            usernames = sqlConnection.ExecuteSearchQuery(sql);
            sql = $"SELECT `Username`, AES_DECRYPT(Password,'CGIKey')  FROM `Login` WHERE Username='{userName.ToLower()}'";
            passwords = sqlConnection.ExecuteGetStringQuery(sql);
            if(passwords.Length == 2)
            {
                string retrievedPassword = passwords[1];
                string retrieved = passwords[0];
                if(password == retrievedPassword)
                {
                    return responses.redirectHome;
                }
                else
                {
                    return responses.wrongEntry;
                }
            }else if(passwords.Length < 2)
            {
                return responses.wrongEntry;
            }
            else if(passwords.Length > 2)
            {
                return responses.multipleEntries;
            }
            
            return responses.massiveError;
        }
    }
}