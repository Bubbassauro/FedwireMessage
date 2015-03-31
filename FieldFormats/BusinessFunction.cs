using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FedwireMessage.FieldFormats
{
    public class BusinessFunction
    {
        public BusinessFunction(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public string BusinessFunctionCode
        {
            get
            {
                if (Value.Length >= 3)
                    return Value.Substring(0, 3);
                else
                    return "";
            }
        }

        public string BusinessFunctionDescription { get; set; }

        public string TransactionTypeCode
        {
            get
            {
                if (Value.Length >= 3)
                    return Value.Substring(0, 3);
                else
                    return "";
            }
        }
    }
}
