using DAL.API;
using DAL;
using DAL.Employees;
using Models;
using Models.Incidenten;
using System;
using System.Collections.Generic;
using DAL.Notificatie;
using DAL.Agenda;

namespace Logic
{
    public class NotificationManager
    {
        private readonly EmployeesHandler employeesHandler;
        private readonly NotificatieHandler notificatieHandler;
        private readonly AgendaHandler agendaHandler;
        public NotificationManager()
        {
            employeesHandler = new EmployeesHandler();
            notificatieHandler = new NotificatieHandler();
            agendaHandler = new AgendaHandler();
        }
        public bool NotifyStandBy(AddIncidentModel model)
        {
            List<string[]> users = employeesHandler.GetStandByEmployees();
            if (users.Count > 0)
            {
                foreach (string[] user in users)
                {
                    string email = user[0];
                    //SendMail.SendEmployeeIncident("bartdgp@outlook.com", "Bart Vermeulen", model.IncidentOmschrijving, model.IncidentNaam);
                    //SendMail.SendKlantIncident("bartdgp@outlook.com", "Bart Vermeulen", model.IncidentOmschrijving, model.IncidentNaam);
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public void NotifySolved(IncidentMailModel model)
        {
            //SendMail.SendEmployeeIncidentSolved("BartDGP@outlook.com", "Bart Vermeulen", model.Beschrijving, model.Naam);
            //SendMail.SendKlantIncidentSolved("BartDGP@outlook.com", "Bart Vermeulen", model.Beschrijving, model.Naam);
        }

        public void PasInstellingenAan(string TypeOfAge, int ValueOfNoti, string userID)
        {
            switch (Convert.ToInt32(TypeOfAge))
            {
                case 0: //Direct
                    SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `Hoeveelheid`='0', `Type`='0' WHERE `UserId`='{userID}'");
                    break;
                case 1: //Dagen
                    SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `Hoeveelheid`='{ValueOfNoti}', `Type`='1' WHERE `UserId`='{userID}'");
                    break;
                case 2: //Weken
                    SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `Hoeveelheid`='{ValueOfNoti}', `Type`='2' WHERE `UserId`='{userID}'");
                    break;
                case 3: //Maanden
                    SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `Hoeveelheid`='{ValueOfNoti}', `Type`='3' WHERE `UserId`='{userID}'");
                    break;
                default: //Error in TypeOfAge
                    throw new ArgumentException("TypeOfAge does not have a usable value", "TypeOfAge");
            }
        }

        public void SendInplanning(string userID, string eventID)
        {
            List<int[]> data = new List<int[]>();
            data.Add(notificatieHandler.GetAgeOfNotification(userID, "ReceiveMail"));
            data.Add(notificatieHandler.GetAgeOfNotification(userID, "ReceiveSMS"));
            data.Add(notificatieHandler.GetAgeOfNotification(userID, "ReceiveWhatsApp"));
            SetDate(data, userID, eventID);
        }

        public void SetDate(List<int[]> data, string userID, string eventID)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (!(data[i].Length > 0)) return;
            }
            DateTime datum = DateTime.Now;
            DateTime eventDate = agendaHandler.GetEventDate(eventID);
            bool isntDirect = true;
            for (int i = 0; i < data.Count; i++)
            {
                switch (data[i][1])
                {
                    case 0:
                        notificatieHandler.VerstuurAfspraakNotificatie(userID, eventID, i + 1);
                        isntDirect = false;
                        break;
                    case 1:
                        datum = eventDate.AddDays(-data[i][0]);
                        break;
                    case 2:
                        datum = eventDate.AddDays(-data[i][0] * 7);
                        break;
                    case 3:
                        datum = eventDate.AddDays(-data[i][0] * 7 * 4);
                        break;
                    default:
                        datum = eventDate.AddDays(-data[i][0]);
                        break;
                }
                if (isntDirect) notificatieHandler.PlanAfspraakNotificatie(userID, eventID, datum, i + 1);
            }
                if (!isntDirect) SendNotificationsDirect(userID, eventID);
        }


        public void SendRuilverzoek(string userID)
        {
            List<string[]> usersThatWantEmail = employeesHandler.EmployeesInfoWithEmailSetting(userID);
            foreach (string[] info in usersThatWantEmail)
            {
                SendMail.SendNieuwRuilverzoek(info[0], info[1]);
            }
        }

        public void SendNotifications()
        {
            string vandaag = DateTime.Now.ToString("yyyy-MM-dd");
            List<string[]> todo = notificatieHandler.TeVersturenNotificaties(vandaag);
            if (todo.Count > 0)
            {
                foreach (string[] array in todo)
                {
                    List<string> eventGegevens = SQLConnection.ExecuteSearchQuery($"SELECT Start, End, IsFullDay, Subject, ThemeColor, Description FROM Rooster WHERE EventId='{array[2]}'");
                    List<string> werknemersGegevens = SQLConnection.ExecuteSearchQuery($"SELECT Voornaam, Email, Telefoonnummer FROM Werknemers WHERE UserId='{array[1]}'");
                    switch (Convert.ToInt32(array[0]))
                    {
                        case 1:
                            //Send Email
                            notificatieHandler.VerstuurAfspraakNotificatie(array[1], array[2], 0);
                            break;
                        case 2:
                            //Send SMS

                            Twilio.SendSMS($"Subject: {eventGegevens[3]}\nDescription: {eventGegevens[5]}", "+31648539715");
                            break;
                        case 3:
                            //Send Whatsapp
                            Twilio.SendWhatsapp($"Subject: {eventGegevens[3]}\nDescription: {eventGegevens[5]}", "+31648539715");
                            break;
                        default:
                            break;
                    }
                    notificatieHandler.DeleteTask(array[3]);
                }
            }
        }


        public void SendNotificationsDirect(string userID, string eventID)
        {
            List<string> eventGegevens = SQLConnection.ExecuteSearchQuery($"SELECT Start, End, IsFullDay, Subject, ThemeColor, Description FROM Rooster WHERE EventId='{eventID}'");
            List<string> werknemersGegevens = SQLConnection.ExecuteSearchQuery($"SELECT Voornaam, Email, Telefoonnummer FROM Werknemers WHERE UserId='{userID}'");
            List<string> userSettings = SQLConnection.ExecuteSearchQuery($"SELECT ReceiveMail, ReceiveSMS, ReceiveWhatsApp FROM Settings WHERE UserId='{userID}'");
            //Send Mail
            if (userSettings[0] == "True")
                notificatieHandler.VerstuurAfspraakNotificatie(userID, eventID, 0);
            //Send SMS
            if (userSettings[1] == "True")
                Twilio.SendSMS($"Subject: {eventGegevens[3]}\nDescription: {eventGegevens[5]}", "+31612549032");
            //Send Whatsapp
            if (userSettings[2] == "True")
                Twilio.SendWhatsapp($"Subject: {eventGegevens[3]}\nDescription: {eventGegevens[5]}", "+31612549032");
        }
    }
}
