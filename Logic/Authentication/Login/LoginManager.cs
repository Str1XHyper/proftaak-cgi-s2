using DAL.Authentication.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Authentication.Login
{
    public class LoginManager
    {
        private LoginHandler loginHandler;
        public LoginManager()
        {
            loginHandler = new LoginHandler();
        }

        public void CreateNewLogin(string naam, string ID, string email) => loginHandler.AddLogin(naam, ID, email);
        public void ChangePasswordAdmin(string email, string password) => loginHandler.ChangeLoginAdmin(email, password);
        public void AddLoginRecord(string authCode, bool succes, string ip, string tijd) => loginHandler.NewLoginRecord(authCode, succes, ip, tijd);

        public bool ChangePassword(string currentPassword, string newPassword, string userID)
        {
            if (currentPassword == null || newPassword == null) return false;
            else return loginHandler.ChangePassword(currentPassword, newPassword, userID);

        }
    }
}
