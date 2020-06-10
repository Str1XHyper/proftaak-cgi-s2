using DAL.Authentication.Password;
using Models.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Logic.Authentication.Password
{
    public class PasswordRequirements
    {
        private CheckPasswordModel cpm;
        private PasswordData passwordData;
        private PasswordChecks passwordChecks;
        public PasswordRequirements()
        {
            passwordData = new PasswordData();
            passwordChecks = new PasswordChecks();
            cpm = passwordData.CreatePasswordModel();
        }

        public bool CheckCurrentPassword(string password, string userid)
        {
            return password == passwordData.GetUserPass(userid); ;
        }

        public CheckPasswordModel CheckPassword(string password)
        {
            if (passwordChecks.ContainsNumber(password) || cpm.NumberRequired == false)
                cpm.Number = true;
            if (passwordChecks.ContainsSpecialChar(password) || cpm.SpecialCharRequired == false)
                cpm.SpecialChar = true;
            if (passwordChecks.ContainsUpperCase(password) || cpm.UpperRequired == false)
                cpm.Upper = true;
            if (passwordChecks.ContainsLowerCase(password) || cpm.LowerRequired == false)
                cpm.Lower = true;
            if (passwordChecks.HasMinLenght(password, cpm.MinLength))
                cpm.Length = true;

            return cpm;
        }
    }
}
