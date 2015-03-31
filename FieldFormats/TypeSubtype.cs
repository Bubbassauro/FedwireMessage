using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FedwireMessage.FieldFormats
{
    /// <summary>
    /// Type Code (2 characters)
    /// Subtype Code (2 characters)
    /// </summary>
    public class TypeSubtype
    {
        public TypeSubtype(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public string TypeCode
        {
            get
            {
                if (Value.Length >= 2)
                    return Value.Substring(0, 2);
                else
                    return "";
            }
        }

        public string SubtypeCode
        {
            get
            {
                if (Value.Length >= 4)
                    return Value.Substring(2, 2);
                else
                    return "";
            }
        }

        public string TypeDescription { get; set; }

        public string SubtypeDescription { get; set; }
    }
}
