using DAL.Employees;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Employees
{
    public class EmployeesManager
    {
        private EmployeesHandler employeesHandler;
        public EmployeesManager()
        {
            employeesHandler = new EmployeesHandler();
        }
        public string[] GetEmployeeNotificationsSettings(string userID) => employeesHandler.GetEmployeeNotificationsSettings(userID);
        public void SetPasswordSettings(List<string> values) => employeesHandler.SetPasswordSettings(values);
        public int SetSettingsAndReturnUserID(string email, string emailSetting, string smssSetting, string whatsAppSetting) => employeesHandler.SetSettingsAndReturnUserID(email, (Convert.ToBoolean(emailSetting) ? 1 : 0), (Convert.ToBoolean(smssSetting) ? 1 : 0), (Convert.ToBoolean(whatsAppSetting) ? 1 : 0));
    }
}
