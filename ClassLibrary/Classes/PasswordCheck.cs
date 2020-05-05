using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ClassLibrary.Classes
{
    public class PasswordCheck
    {
        private int minLen;
        public bool CheckPassword(string password)
        {
            bool number = false;
            bool specChar = false;
            bool upper = false;
            bool lower = false;
            bool length = false;
            minLen = 6;

            if (ContainsNumber(password))
                number = true;
            if (ContainsSpecialChar(password))
                specChar = true;
            if (ContainsUpperCase(password))
                upper = true;
            if (ContainsLowerCase(password))
                lower = true;
            if (HasMinLenght(password))
                length = true;

            if (lower && upper && number && specChar && length)
                return true;
            else
                return false;
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
            return counter >= minLen;
        }
        #endregion
    }
}
