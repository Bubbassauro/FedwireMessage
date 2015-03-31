using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FedwireMessage.FieldFormats
{
    /// <summary>
    /// Input Cycle Date (CCYYMMDD)
    /// Input Source (8 characters)
    /// Input Sequence Number (6 characters)
    /// </summary>
    public class IMAD
    {
        public IMAD(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public string InputCycle
        {
            get
            {
                if (Value.Length > 8)
                    return Value.Substring(0, 8);
                else
                    return "";
            }
        }

        public DateTime? InputCycleDate
        {
            get
            {
                DateTime inputCycleDate;
                if (Value.Length >= 8)
                {
                    if (DateTime.TryParseExact(Value.Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out inputCycleDate))
                        return inputCycleDate;
                }
                return null;
            }
        }

        public string InputSource
        {
            get
            {
                if (Value.Length >= 16)
                {
                    return Value.Substring(8, 8);
                }
                else
                {
                    return "";
                }
            }
        }

        public string InputSequenceNumber
        {
            get
            {
                if (Value.Length >= 22)
                {
                    return Value.Substring(16, 6);
                }
                else
                {
                    return "";
                }
            }
        }

        public string SplitValue
        {
            get
            {
                return String.Format("{0} {1} {2}", InputCycle, InputSource, InputSequenceNumber);
            }
        }
    }
}
