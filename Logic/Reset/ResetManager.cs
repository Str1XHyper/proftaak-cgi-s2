using DAL.Employees;
using DAL.Reset;
using Logic.Authentication;
using Logic.Employees;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Reset
{
    public class ResetManager
    {
        private readonly ResetHandler resetHandler;
        public ResetManager()
        {
            resetHandler = new ResetHandler();
        }
        public void SendRequest()
        {

        }

        public bool ChangeResetPassword(string confPass, string newPassword, string authCode, string secretCode)
        {
            if (confPass != null && newPassword != null) return resetHandler.ResetPassword(confPass, newPassword, authCode, secretCode);
            else return false;
        }

        public void SendReset(string email)
        {
            EmployeeInfoManager employee = new EmployeeInfoManager();
            string customCode = GenerateAuthToken.GetUniqueKeyOriginal_BIASED(25);
            int userID = employee.IDFromMail(email);
            if (userID != 0) resetHandler.SendReset(email, customCode, userID);
        }
    }
}
