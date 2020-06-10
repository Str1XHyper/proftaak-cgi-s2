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

        public void NotifyPlanned(string userID, string eventID)
        {
            List<string> response = SQLConnection.ExecuteSearchQuery($"SELECT ReceiveMail, ReceiveSMS, ReceiveWhatsApp, Type, Hoeveelheid FROM Settings WHERE UserId='{userID}'");
            if (response.Count > 0)
            {
                List<string> eventGegevens = SQLConnection.ExecuteSearchQuery($"SELECT Start, End, IsFullDay, Subject, ThemeColor, Description FROM Rooster WHERE EventId='{eventID}'");
                List<string> werknemersGegevens = SQLConnection.ExecuteSearchQuery($"SELECT Voornaam, Email, Telefoonnummer FROM Werknemers WHERE UserId='{userID}'");
                if (response[0].ToLower() == "true")
                {
                    //wilt email ontvangen

                    SendMail.SendNotification(werknemersGegevens[1], werknemersGegevens[0], werknemersGegevens[0], eventGegevens[3], eventGegevens[5], eventGegevens[4]);
                }
                if(response[2].ToLower() == "true")
                {
                    Twilio.SendWhatsapp($"Subject: {eventGegevens[3]}\nDescription: {eventGegevens[5]}","+" + werknemersGegevens[2]);
                }
                if (response[1].ToLower() == "true")
                {
                    Twilio.SendSMS($"Subject: {eventGegevens[3]}\nDescription: {eventGegevens[5]}", "+" + werknemersGegevens[2]);
                }
            }
        }

        public void PasInstellingenAan(string TypeOfAge, int ValueOfNoti, string userID)
        {
            switch (Convert.ToInt32(TypeOfAge))
            {
                case 0: //DIRECT
                    SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `Hoeveelheid`='0', `Type`='0' WHERE `UserId`='{userID}'");
                    break;
                case 1: //DAGEN
                    SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `Hoeveelheid`='{ValueOfNoti}', `Type`='1' WHERE `UserId`='{userID}'");
                    break;
                case 2: //WEEEEEEEEEEEEEEEEEEEEEEEEEEEEEKEN
                    SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `Hoeveelheid`='{ValueOfNoti}', `Type`='2' WHERE `UserId`='{userID}'");
                    break;
                case 3: //MAANDEN
                    SQLConnection.ExecuteNonSearchQuery($"UPDATE `Settings` SET `Hoeveelheid`='{ValueOfNoti}', `Type`='3' WHERE `UserId`='{userID}'");
                    break;
                default: //waduhek
                    throw new System.ArgumentException("waduhek", "bananaman");
            }
        }

        public void SendInplanning(string userID, string eventID)
        {
            int[] wiltOntvangen = notificatieHandler.GetAgeOfNotification(userID);
            DateTime eventDate = agendaHandler.GetEventDate(eventID);
            if (wiltOntvangen.Length > 0)
            {
                DateTime datum = DateTime.Now;
                bool isntDirect = true;
                switch (wiltOntvangen[1])
                {
                    case 0:
                        notificatieHandler.VerstuurAfspraakNotificatie(userID, eventID);
                        isntDirect = false;
                        break;
                    case 1:
                        datum = eventDate.AddDays(-wiltOntvangen[0]);
                        break;
                    case 2:
                        datum = eventDate.AddDays(-wiltOntvangen[0]*7);
                        break;
                    case 3:
                        datum = eventDate.AddDays(-wiltOntvangen[0]*7*4);
                        break;
                    default:
                        datum = eventDate.AddDays(-wiltOntvangen[0]);
                        break;
                }
                if(isntDirect) notificatieHandler.PlanAfspraakNotificatie(userID, eventID, datum);
            }
        }

        public void SendRuilverzoek(string userID)
        {
            List<string[]> users = employeesHandler.EmployeesInfoWithEmailSetting(userID);
            foreach (string[] info in users)
            {
                SendMail.SendNieuwRuilverzoek(info[0], info[1]);
            }
        }
    }
}
