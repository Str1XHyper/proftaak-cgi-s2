using Models.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Logic.Authentication.Password
{
    public class PasswordChecks
    {
        public bool ContainsNumber(string password)
        {
            Regex regex = new Regex("\\d");
            return regex.IsMatch(password);
        }

        public bool ContainsUpperCase(string password)
        {
            Regex regex = new Regex("[A-Z]");
            return regex.IsMatch(password);
        }

        public bool ContainsLowerCase(string password)
        {
            Regex regex = new Regex("[a-z]");
            return regex.IsMatch(password);
        }

        public bool ContainsSpecialChar(string password)
        {
            Regex regex = new Regex("[#?!@$%^&*-]");
            return regex.IsMatch(password);
        }

        public bool HasMinLenght(string password, int minLength)
        {
            int counter = 0;
            foreach (char c in password)
            {
                counter++;
            }
            return counter >= minLength;
        }
    }
}
