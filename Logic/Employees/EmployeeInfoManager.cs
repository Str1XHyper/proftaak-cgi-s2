using DAL;
using DAL.Employees;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Employees
{
    public class EmployeeInfoManager
    {
        private EmployeeInfoHandler employeeHandler = new EmployeeInfoHandler();
        public string getAuthToken(string username) => employeeHandler.getAuthToken(username);
        public void SetPasswordSettings(List<string> values) => employeeHandler.SetPasswordSettings(values);
        public int SetSettingsAndReturnUserID(string email, string emailSetting, string smssSetting, string whatsAppSetting) => employeeHandler.SetSettingsAndReturnUserID(email, (Convert.ToBoolean(emailSetting) ? 1 : 0), (Convert.ToBoolean(smssSetting) ? 1 : 0), (Convert.ToBoolean(whatsAppSetting) ? 1 : 0));
        public string[] GetEmployeeNotificationsSettings(string userID) => employeeHandler.GetEmployeeNotificationsSettings(userID);
    
        public string RolFromAuth(string authCode)
        {
            if (authCode != null) return employeeHandler.RolFromAuth(authCode);
            else return "nietIngelogd";
        }

        public string RolFromID(string userID)
        {
            if (userID != null) return employeeHandler.RolFromID(userID);
            else return "geenUserID";
        }

        public int IDFromAuth(string authCode)
        {
            if (authCode != null) return employeeHandler.IDFromAuth(authCode);
            else return 0;
        }

        public List<string> EmployeeInfo(string authCode)
        {
            if (authCode != null) return employeeHandler.EmployeeInfo(authCode);
            else return new List<string>();
        }

        public int IDFromMail(string mail) => employeeHandler.UserIDFromMail(mail);
    
    }
}
