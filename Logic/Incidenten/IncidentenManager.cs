using DAL.Incidenten;
using Models;
using Models.Incidenten;
using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Incidenten
{
    public class IncidentenManager
    {
        NotificationManager notificationManager = new NotificationManager();
        IncidentenHelper incidentenHelper = new IncidentenHelper();
        public bool StartIncident(int IncidentID)
        {
            bool suceeded = false;
            suceeded = incidentenHelper.UpdateIncidentStatus(1, IncidentID);
            suceeded = incidentenHelper.InsertStatusUpdate(IncidentID,0,"Begonnen met werken aan het incident", DateTime.Now, DateTime.Now, "Begonnen");
            return suceeded;
        }

        public bool FinishIncident(int IncidentID, IncidentMailModel model)
        {
            int StatusCount = incidentenHelper.GetAmountStatusUpdates(IncidentID);
            bool suceeded = incidentenHelper.InsertStatusUpdate(IncidentID, StatusCount, "Incident is afgehandled", DateTime.Now, DateTime.Now, "Afgehandeld");
            suceeded = incidentenHelper.UpdateIncidentStatus(2, IncidentID);
            notificationManager.NotifySolved(model);
            return suceeded;
        }

        public bool AddStatusUpdate(AddStatusUpdateModel model) => incidentenHelper.InsertStatusUpdate(model.IncidentID, model.StatusIdIncident, model.StatusOmschrijving, DateTime.Parse(model.Start), DateTime.Parse(model.End), model.StatusNaam) ;
        public bool DeleteStatusUpdate(int UpdateID) => incidentenHelper.DeleteStatusUpdate(UpdateID);
        public List<string[]> GetIncidents() => incidentenHelper.GetIncidentsFromDatabase();
        public List<string[]> GetIncidentUpdates() => incidentenHelper.GetIncidentUpdatesFromDatabase();
    }
}
