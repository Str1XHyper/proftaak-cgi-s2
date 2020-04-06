﻿using System;
using System.Globalization;

namespace Proftaakrepos.Models
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class HoursWorkedModel
    {
        public int ProjectId { get; set; }
        public static string EmployeeName { get; set; }
        public static DateTime Day
        {
            get => DateTime.Now;
            set => Day = value;
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public static string GetWeekOfYear
        {
            //@TODO: Find a way to work without static in views.
            get
            {
                CultureInfo cul = CultureInfo.CurrentCulture;

                int firstDayWeek = cul.Calendar.GetWeekOfYear(Day,
                    CalendarWeekRule.FirstDay,
                    DayOfWeek.Monday);

                int weekNum = cul.Calendar.GetWeekOfYear(Day,
                    CalendarWeekRule.FirstDay,
                    DayOfWeek.Monday);

                return String.Format("Week: {0}", weekNum);
            }
        }

        public float TotalTime { get; set; }
        public float WorkedHours { get; set; }
        public float Overtime { get; set; }

    }
}
