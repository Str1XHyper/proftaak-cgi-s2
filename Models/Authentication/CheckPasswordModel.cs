using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Authentication
{
    public class CheckPasswordModel
    {
        public CheckPasswordModel()
        {

        }

        public CheckPasswordModel(bool numberReq, bool specCharReq, bool upperReq, bool lowerReq, int minLen)
        {
            NumberRequired = numberReq;
            SpecialCharRequired = specCharReq;
            UpperRequired = upperReq;
            LowerRequired = lowerReq;
            MinLength = minLen;

        }

        public bool NumberRequired { get; set; }
        public bool SpecialCharRequired { get; set; }
        public bool UpperRequired { get; set; }
        public bool LowerRequired { get; set; }
        public int MinLength { get; set; }
        public bool Number { get; set; }
        public bool SpecialChar { get; set; }
        public bool Upper { get; set; }
        public bool Lower { get; set; }
        public bool Length { get; set; }
    }
}
