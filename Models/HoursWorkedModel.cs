using System;
using System.Globalization;

namespace Models
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class HoursWorkedModel
    {
        public static int ProjectId { get; set; }

        public int UserId { get; set; }

        public int WeekNumber { get; set; }
        public static string EmployeeName { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public float TotalTime { get; set; }
        public float WorkedHours { get; set; }
        public float Overtime { get; set; }

    }
}
