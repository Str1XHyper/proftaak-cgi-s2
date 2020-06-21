using System;
using System.Collections.Generic;
using System.Text;

namespace Models.HoursWorked
{
    public class ParsedTimeSheetRow
    {
        public DateTime Start { get; set; }
        public DateTime Eind { get; set; }
        public TimeSpan Overuren { get; set; }
    }
}
