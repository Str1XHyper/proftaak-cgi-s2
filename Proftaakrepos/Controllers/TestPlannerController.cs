using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ClassLibrary.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Agenda;
using Ubiety.Dns.Core.Records.NotUsed;

namespace Proftaakrepos.Controllers
{
    public class TestPlannerController : Controller
    {
        public List<ParseableEventModel> FetchAllEvents(string userIds, string type)
        {
            string[] uids = SterilizeInput(userIds);
            type = HtmlEncoder.Default.Encode(type);

            // Create sql query
            string sqlQuery = $"Select * from `Rooster` WHERE `UserId` = '{uids[0]}'";
            if (type != "all")
            {
                sqlQuery += $"AND `ThemeColor` = '{type}'";
            }
            if (uids.Length > 1)
            {
                for (int i = 1; i < uids.Length; i++)
                {
                    if (!string.IsNullOrEmpty(uids[i]))
                    {
                        sqlQuery += $"OR `UserId` = '{uids[i]}' ";
                        if (type != "all")
                        {
                            sqlQuery += $"AND `ThemeColor` = '{type}'";
                        }
                    }
                }
            }

            // Get names
            List<string[]> names = SQLConnection.ExecuteSearchQueryWithArrayReturn("SELECT `Voornaam`, `Tussenvoegsel`, `Achternaam`, `UserId` FROM `Werknemers`");

            // Get events
            List<string[]> result = SQLConnection.ExecuteSearchQueryWithArrayReturn(sqlQuery);

            // Get colors
            List<string> colors = SQLConnection.ExecuteSearchQuery("SELECT * FROM `ColorScheme`");
            if (colors.Count < 1)
            {
                colors = new List<string>();
                // Standby
                colors.Add("#3B5A6F");
                // Incidenten
                colors.Add("#353B45");
                // Pauze
                colors.Add("#828A87");
                // Verlof
                colors.Add("#830101");
            }

            // Put result into models
            String[] c = { "Stand-by", "Incidenten", "Pauze", "Verlof" };
            List<ParseableEventModel> returnList = new List<ParseableEventModel>();
            foreach (string[] row in result)
            {
                int index = Array.IndexOf(c, row[6]);
                string title = row[2];
                if (HttpContext.Session.GetString("Rol").ToLower() == "roostermaker")
                {
                    foreach(string[] name in names)
                    {
                        if(row[1] == name[3])
                            title = name[0] + " - " + title;
                    }
                }
                ParseableEventModel em = new ParseableEventModel
                {
                    id = Convert.ToInt32(row[0]),
                    title = title,
                    start = DateTime.Parse(row[4]),
                    end = DateTime.Parse(row[5]),
                    backgroundColor = colors[index],
                    allDay = Convert.ToBoolean(Convert.ToInt32(row[7])),
                    description = row[3],
                    borderColor = "#010203",
                    soort = row[6],
                    userId = row[1],
                };
                returnList.Add(em);
            }

            // return parsable model for Full Calendar
            return returnList;
        }

        public void UpdateEvent(DateTime start, DateTime end, string eventid, bool allday)
        {
            // Encode to prevent SQL Injection
            string startTime = HtmlEncoder.Default.Encode(start.ToString("yyyy/MM/dd HH:mm"));
            string endTime = HtmlEncoder.Default.Encode(end.ToString("yyyy/MM/dd HH:mm"));
            eventid = HtmlEncoder.Default.Encode(eventid);
            string alldaystring = HtmlEncoder.Default.Encode(allday.ToString());
            allday = Boolean.Parse(alldaystring);

            // Update event
            SQLConnection.ExecuteNonSearchQuery($"Update Rooster Set Start = '{startTime}',End = '{endTime}',IsFullDay = '{Convert.ToInt32(allday)}' Where EventId = '{eventid}'");
        }

        [HttpPost]
        public void CreateEvent(EventModel newmodel)
        {
            if (!string.IsNullOrEmpty(newmodel.userId))
            {
                string[] uids = SterilizeInput(newmodel.userId);
                string sqlquery = $"INSERT INTO Rooster(UserId, Subject, Description, Start, End, ThemeColor, IsFullDay, IsPending) VALUES ";
                for (int i = 0; i < uids.Length; i++)
                {
                    if (i > 0)
                    {
                        sqlquery += ",";
                    }
                    sqlquery += $"('{uids[i]}', '{newmodel.title}', '{newmodel.description}', '{newmodel.startDate.ToString("yyyy/MM/dd HH:mm:ss")}', '{newmodel.endDate.ToString("yyyy/MM/dd HH:mm:ss")}', '{newmodel.themeColor}', '{(newmodel.isFullDay)}', '{(newmodel.isPending ? 1 : 0)}')";
                }
                SQLConnection.ExecuteNonSearchQuery(sqlquery);
            }
        }

        public string[] GetUsers()
        {
            List<string[]> names = SQLConnection.ExecuteSearchQueryWithArrayReturn("SELECT `UserId`, `Voornaam`, `Tussenvoegsel`, `Achternaam` FROM `Werknemers` ORDER BY `UserId` ASC");
            string[] parsedNames = new string[names.Count];
            for (int i = 0; i < names.Count; i++)
            {
                string completeName = "";
                foreach (string namePiece in names[i])
                {
                    completeName += namePiece + " ";
                }
                completeName.Trim();
                parsedNames[i] = completeName;
            }
            return parsedNames;
        }

        ///<summary>
        ///Sterilize string input seperated with ','
        ///return string[] of valid and clean data
        ///</summary>
        private string[] SterilizeInput(string input)
        {
            // Check if input is not empty
            if (input == "" || input == null)
            {
                throw new ArgumentNullException();
            }

            // Encode to prevent SQL Injection
            string[] cleanData = new string[input.Split(',').Length];
            for (int i = 0; i < input.Split(",").Length; i++)
            {
                cleanData[i] = HtmlEncoder.Default.Encode(input.Split(',')[i]);
            }

            return cleanData;
        }
    }
}