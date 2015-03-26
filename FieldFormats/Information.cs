using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FedwireMessage.FieldFormats
{
    public class Information
    {
        public string Value { get; set; }

        private string[] _values;

        public Information(string value)
        {
            Value = value;
            _values = value.Split('*');
        }

        public string IdCode
        {
            get
            {
                if (Value.Length >= 1)
                    return Value.Substring(0, 1);
                else
                    return "";
            }
        }

        public string Identifier
        {
            get
            {
                if (_values.Length > 0 && _values[0].Length >= 1)
                    return _values[0].Substring(1);
                else
                    return "";
            }
        }

        public string Name
        {
            get
            {
                if (_values.Length >= 2)
                    return _values[1];
                else
                    return "";
            }
        }

        public string[] Address
        {
            get
            {
                List<string> addresses = new List<string>();
                for (int i = 2; i < _values.Length; i++)
                    addresses.Add(_values[i]);

                return addresses.ToArray();
            }
        }
    }
}
