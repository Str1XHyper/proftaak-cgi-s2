using DAL.HoursWorked;
using Models;
using Models.HoursWorked;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Logic.HoursWorked
{
    public class OverviewTableManager
    {
        public int GetIso8601WeekOfYear(DateTime time)
        {
            if (time.Year == 0001) time = DateTime.Now;
            var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        public List<EventModel> AssembleEventModel(List<string[]> timesheetData, string userID)
        {
            List<EventModel> eventList = new List<EventModel>();
            foreach (string[] row in timesheetData)
            {
                if (row[1] == userID)
                {
                    EventModel newEvent = new EventModel()
                    {
                        userId = row[1],
                        startDate = Convert.ToDateTime(row[2]),
                        endDate = Convert.ToDateTime(row[3]),
                        themeColor = row[5],
                    };
                    eventList.Add(newEvent);
                }
            }
            return eventList;
        }
        public Week AssembleWeekModel(List<EventModel> eventlist)
        {
            Week week = new Week();
            foreach (EventModel eventmodel in eventlist)
            {
                if (eventmodel.themeColor != "Pauze")
                {
                    if (eventmodel.startDate.DayOfWeek == eventmodel.endDate.DayOfWeek)
                    {
                        week = AddHours(eventmodel.startDate.DayOfWeek, week, eventmodel.endDate.Hour - eventmodel.startDate.Hour, eventmodel.themeColor);
                    }
                    else
                    {
                        week = AddHours(eventmodel.startDate.DayOfWeek, week, 24 - eventmodel.startDate.Hour, eventmodel.themeColor);
                        week = AddHours(eventmodel.endDate.DayOfWeek, week, eventmodel.endDate.Hour, eventmodel.themeColor);
                    }
                }
            }
            return week;
        }
        private Week AddHours(DayOfWeek day, Week week, int hours, string type)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    week = SeperateHours(type, week, hours, 0);
                    break;
                case DayOfWeek.Tuesday:
                    week = SeperateHours(type, week, hours, 1);
                    break;
                case DayOfWeek.Wednesday:
                    week = SeperateHours(type, week, hours, 2);
                    break;
                case DayOfWeek.Thursday:
                    week = SeperateHours(type, week, hours, 3);
                    break;
                case DayOfWeek.Friday:
                    week = SeperateHours(type, week, hours, 4);
                    break;
                case DayOfWeek.Saturday:
                    week = SeperateHours(type, week, hours, 5);
                    break;
                case DayOfWeek.Sunday:
                    week = SeperateHours(type, week, hours, 6);
                    break;
            }
            return week;
        }
        private Week SeperateHours(string type, Week week, int hours, int day)
        {
            if (type == "Incidenten")
            {
                week.ActiveTime[day] += hours;
                week.Total[day] += hours;
            }
            else if (type == "Stand-by")
            {
                week.StandByTime[day] += hours;
                week.Total[day] += hours;
            }
            else if (type == "Verlof")
                week.FurloughTime[day] += hours;
            return week;
        }
        public List<string[]> GetTimesheetEventsByWeek(int week) => new OverviewTableHandler().GetTimeSheetEventsByWeek(week);
    }
}
