using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FedwireMessage.FieldFormats
{
    /// <summary>
    /// Returns an empty string rather than an error if the dictionary key is not found.
    /// </summary>
    public class Tags : Dictionary<string, string>
    {
        public new string this[string key]
        {
            get
            {
                if (base.ContainsKey(key))
                {
                    return base[key];
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
