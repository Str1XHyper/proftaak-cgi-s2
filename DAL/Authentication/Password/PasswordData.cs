using Models.Authentication;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Authentication.Password
{
    public class PasswordData
    {
        public string GetUserPass(string userid)
        {
            return SQLConnection.ExecuteSearchQuery($"Select AES_DECRYPT(`Password`, 'CGIKey') FROM `Login` WHERE `UserID` = '{userid}'")[0];
        }

        public CheckPasswordModel CreatePasswordModel()
        {
            List<string> result = SQLConnection.ExecuteSearchQuery($"SELECT * FROM `PasswordRequirements`");
            return new CheckPasswordModel(Convert.ToBoolean(Convert.ToInt16(result[0])), Convert.ToBoolean(Convert.ToInt16(result[1])), Convert.ToBoolean(Convert.ToInt16(result[2])), Convert.ToBoolean(Convert.ToInt16(result[3])), Convert.ToInt32(result[4]));
        }
    }
}
