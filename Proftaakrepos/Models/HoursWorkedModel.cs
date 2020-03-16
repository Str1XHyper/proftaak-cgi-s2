using System;
using System.Globalization;

namespace Proftaakrepos.Models
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class HoursWorkedModel
    {
        public int ProjectId { get; set; }

        //@TODO: insert logic here.
        public int EmployeeName { get; set; }

        public DateTime Day { get => DateTime.Now; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public static int GetWeekOfYear(DateTime dt)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(dt);

            //Als het maandag, dinsdag of woensdag is, geef hetzelfde week # aan donderdag, vr, za, zo. 
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                dt = dt.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        public float TotalTime { get; set; }
        public float WorkedHours { get; set; }
        public float Overtime { get; set; }

    }
}
