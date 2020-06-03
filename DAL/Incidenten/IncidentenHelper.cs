using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Incidenten
{
    public class IncidentenHelper
    {
        public bool UpdateIncidentStatus(int status, int IncidentID) => SQLConnection.ExecuteNonSearchQuery($"UPDATE `Incidenten` SET `Afgehandeld`= '{status}' WHERE `IncidentID` = '{IncidentID}'");
        public bool InsertStatusUpdate(int IncidentID,int StatusIDIncident, string Beschrijving, DateTime start, DateTime end, string StatusNaam) => SQLConnection.ExecuteNonSearchQuery($"INSERT INTO `IncidentUpdates`(`IncidentID`, `StatusIDIncident`, `StatusOmschrijving`, `Start`, `End`, `StatusNaam`) VALUES ('{IncidentID}','{StatusIDIncident}','{Beschrijving}','{start.ToString("yyyy/MM/dd HH:mm")}','{end.ToString("yyyy/MM/dd HH:mm")}', '{StatusNaam}')");
        public int GetAmountStatusUpdates(int IncidentID) => Convert.ToInt32(SQLConnection.ExecuteSearchQuery($"SELECT COUNT(`StatusIDIncident`) FROM  `IncidentUpdates` WHERE `IncidentID` = '{IncidentID}'")[0]);
        public List<string[]> GetIncidentsFromDatabase() => SQLConnection.ExecuteSearchQueryWithArrayReturn("SELECT * FROM `Incidenten` WHERE `Afgehandeld` = '0' OR `Afgehandeld` = '1'");
        public List<string[]> GetIncidentUpdatesFromDatabase() => SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT IncidentID,COUNT(*) FROM IncidentUpdates GROUP BY IncidentID");
        public bool DeleteStatusUpdate(int UpdateID) => SQLConnection.ExecuteNonSearchQuery($"DELETE FROM `IncidentUpdates` WHERE `StatusId` = '{UpdateID}'");
            
    }
}
