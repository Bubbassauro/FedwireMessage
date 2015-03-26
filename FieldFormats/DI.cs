using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FedwireMessage.FieldFormats
{
    /// <summary>
    /// ABA Number (9 characters)
    /// Short Name (18 characters; if omitted, it will not be inserted by the Fedwire Funds Service)
    /// </summary>
    public class DI
    {
        public DI(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public string AbaNumber
        {
            get
            {
                if (Value.Length >= 9)
                    return Value.Substring(0, 9);
                else
                    return "";
            }
        }

        public string ShortName
        {
            get
            {
                if (Value.Length > 9)
                    return Value.Substring(9).Replace("*", "");
                else
                    return "";
            }
        }
    }
}
