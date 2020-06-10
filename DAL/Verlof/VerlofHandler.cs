using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;

namespace DAL.Verlof
{
    public class VerlofHandler
    {
        public void Afwijzen(string verlofID)
        {
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Verlofaanvragen` SET `Geaccepteerd`='0' WHERE `VerlofID` = '{verlofID}'");
            List<string> response = SQLConnection.ExecuteSearchQuery($"SELECT `EventID` FROM `Verlofaanvragen` WHERE `VerlofID`='{verlofID}'");
            List<string> info = SQLConnection.ExecuteSearchQuery($"SELECT Rooster.EventId, Rooster.Subject FROM `Rooster` INNER JOIN `Verlofaanvragen` ON Rooster.EventId = Verlofaanvragen.EventID WHERE Verlofaanvragen.VerlofID = '{verlofID}'");
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Rooster` SET `Subject` = '{info[1]} - Afgewezen' WHERE `EventId` = '{info[0]}'");
        }
        
        public void Goedkeuren(string verlofID)
        {
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Verlofaanvragen` SET `Geaccepteerd` = '1' WHERE `VerlofID` = '{verlofID}'");
            List<string> info = SQLConnection.ExecuteSearchQuery($"SELECT Rooster.EventId, Rooster.Subject FROM `Rooster` INNER JOIN `Verlofaanvragen` ON Rooster.EventId = Verlofaanvragen.EventID WHERE Verlofaanvragen.VerlofID = '{verlofID}'");
            SQLConnection.ExecuteNonSearchQuery($"UPDATE `Rooster` SET `Subject` = '{info[1]} - Geaccepteerd' WHERE `EventId` = '{info[0]}'");
        }

        public List<List<string[]>> GegevensOphalen()
        {
            List<string[]> requests = SQLConnection.ExecuteSearchQueryWithArrayReturn($"Select * FROM `Verlofaanvragen` WHERE `Geaccepteerd`='-1'");
            List<string[]> names = SQLConnection.ExecuteSearchQueryWithArrayReturn($"Select `UserId`, `Voornaam`, `Tussenvoegsel`, `Achternaam` FROM `Werknemers`");
            List<string[]> info = SQLConnection.ExecuteSearchQueryWithArrayReturn($"SELECT `EventID`, `Description`, `Start`, `End`, `IsFullDay` FROM `Rooster` WHERE `ThemeColor`='Verlof'");
            List<List<string[]>> data = new List<List<string[]>>()
            {
                requests,
                names,
                info
            };
            return data;
        }
    }
}
