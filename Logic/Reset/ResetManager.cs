using DAL.Reset;
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
    }
}
