using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Models.Authentication;

namespace ClassLibrary.Classes
{
    public class PasswordCheck
    {
        private CheckPasswordModel cpm;
        private string authCode;
        public PasswordCheck(CheckPasswordModel cpm, string authCode)
        {
            this.cpm = cpm;
            this.authCode = authCode;
        }

        public bool CheckCurrentPassword(string password)
        {
            List<string> UID = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Login` WHERE `Password` = AES_ENCRYPT('{password}', 'CGIKey')");
            if (UID.Count > 0)
            {
                List<string> result = SQLConnection.ExecuteSearchQuery($"SELECT `AuthCode` FROM `Werknemers` WHERE `UserId` = '{UID[0]}'");
                return result[0] == authCode;
            } else
            {
                return false;
            }
        }

        public CheckPasswordModel CheckPassword(string password)
        {
            if (ContainsNumber(password) || cpm.NumberRequired == false)
                cpm.Number = true;
            if (ContainsSpecialChar(password) || cpm.SpecialCharRequired == false)
                cpm.SpecialChar = true;
            if (ContainsUpperCase(password) || cpm.UpperRequired == false)
                cpm.Upper = true;
            if (ContainsLowerCase(password) || cpm.LowerRequired == false)
                cpm.Lower = true;
            if (HasMinLenght(password))
                cpm.Length = true;

            return cpm;
        }

        #region Logic
        private bool ContainsNumber(string password)
        {
            Regex regex = new Regex("\\d");
            return regex.IsMatch(password);
        }

        private bool ContainsUpperCase(string password)
        {
            Regex regex = new Regex("[A-Z]");
            return regex.IsMatch(password);
        }

        private bool ContainsLowerCase(string password)
        {
            Regex regex = new Regex("[a-z]");
            return regex.IsMatch(password);
        }

        private bool ContainsSpecialChar(string password)
        {
            Regex regex = new Regex("[#?!@$%^&*-]");
            return regex.IsMatch(password);
        }

        private bool HasMinLenght(string password)
        {
            int counter = 0;
            foreach(char c in password)
            {
                counter++;
            }
            return counter >= cpm.MinLength;
        }
        #endregion
    }
}
