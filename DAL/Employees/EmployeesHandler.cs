using DAL.Agenda;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Employees
{
    public class EmployeesHandler
    {
        private AgendaHandler agendaHandler;
        public EmployeesHandler()
        {
            agendaHandler = new AgendaHandler();
        }
        public List<string[]> GetStandByEmployees()
        {
            List<string[]> roosterData = agendaHandler.GetAllRoosterData();
            List<string[]> userData = GetAllEmployees();
            List<string[]> users = new List<string[]>();

            foreach (string[] roosterEvent in roosterData)
            {
                DateTime start = DateTime.Parse(roosterEvent[1]);
                DateTime end = DateTime.Parse(roosterEvent[2]);
                if (DateTime.Compare(start, DateTime.Now) < 0 && DateTime.Compare(end, DateTime.Now) > 0)
                {
                    bool userInList = false;
                    foreach (string[] userInfo in userData)
                    {
                        if (userInfo[0] == roosterEvent[0])
                        {
                            if (users.Count > 0)
                            {
                                foreach (string[] user in users)
                                {
                                    if (user[0] == userInfo[0])
                                    {
                                        userInList = true;
                                        break;
                                    }
                                }

                                if (userInList)
                                {
                                    break;
                                }
                                else
                                {
                                    users.Add(userInfo);
                                }
                            }
                            else
                            {
                                users.Add(userInfo);
                            }
                        }
                    }
                }
            }

            return users;
        }
        public List<string[]> GetAllEmployees()
        {
            return SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT UserId,Email,Voornaam FROM Werknemers");
        }

        public string[] GetEmployeeNotificationsSettings(string userID) => SQLConnection.ExecuteSearchQuery($"SELECT * FROM `Settings` WHERE `UserId` = {userID}").ToArray();
        public void SetPasswordSettings(List<string> values) => SQLConnection.ExecuteNonSearchQuery($"UPDATE `PasswordRequirements` SET `NumberRequired` = '{values[0]}', `SpecialCharRequired` = '{values[1]}', `UpperRequired` = '{values[2]}', `LowerRequired` = '{values[3]}', `MinimumLength` = '{values[4]}'");
        public int SetSettingsAndReturnUserID(string email, int emailSetting, int smssSetting, int whatsAppSetting)
        {
            List<string> userIDlist = SQLConnection.ExecuteSearchQuery($"SELECT `UserId` FROM `Werknemers` WHERE `Email` = '{email}'");
            int userID = Convert.ToInt32(userIDlist[0]);
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `ReceiveMail`='{emailSetting}',`ReceiveSMS`='{smssSetting}',`ReceiveWhatsApp`='{whatsAppSetting}' WHERE `UserId` = {userID}");
            return userID;
        }
    }
}
