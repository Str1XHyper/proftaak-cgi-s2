using System;
using System.Collections.Generic;
using ClassLibrary.Classes;
using MySql.Data.MySqlClient;

namespace ClassLibrary
{
    public class LoginClass
    {
        private SQLConnection sqlConnection = new SQLConnection();
        private string sql;
        private List<string> passwords;
        private List<string> usernames;
        private enum responses { redirectHome, wrongEntry, multipleEntries, massiveError};
        public Enum LoginUserFunction(string userName, string password)
        {
            sql = $"SELECT `Username` FROM `Login` WHERE Username='{userName.ToLower()}'";
            usernames = sqlConnection.ExecuteSearchQuery(sql);
            sql = $"SELECT AES_DECRYPT(Password,'CGIKey')  FROM `Login` WHERE Username='{userName.ToLower()}'";
            passwords = sqlConnection.ExecuteGetStringQuery(sql);
            if(usernames.Count == 1)
            {
                string retrievedPassword = passwords[0];
                if(password == retrievedPassword)
                {
                    return responses.redirectHome;
                }
                else
                {
                    return responses.wrongEntry;
                }
            }else if(usernames.Count != 2)
            {
                return responses.wrongEntry;
            }
            else if(usernames.Count == 2)
            {
                return responses.multipleEntries;
            }
            
            return responses.massiveError;
        }
    }
}