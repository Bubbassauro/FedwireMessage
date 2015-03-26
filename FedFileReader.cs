using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FedwireMessage.FieldFormats;

namespace FedwireMessage
{
    public class FedFileReader
    {
        public Tags BusinessFunctions { get; set; }
        public Tags SubTypeCodes { get; set; }
        public Tags TypeCodes { get; set; }
        public Tags IdCodes { get; set; }
        public List<Wire> Wires { get; set; }

        public FedFileReader()
        {
            // Load Frb code lists
            BusinessFunctions = JsonConvert.DeserializeObject<Tags>(FrbCodes.Resources.BusinessFunctionCode);
            TypeCodes = JsonConvert.DeserializeObject<Tags>(FrbCodes.Resources.TypeCode);
            SubTypeCodes = JsonConvert.DeserializeObject<Tags>(FrbCodes.Resources.SubTypeCode);
            IdCodes = JsonConvert.DeserializeObject<Tags>(FrbCodes.Resources.IdCodes);
        }

        public void ReadFile(Stream fileStream)
        {
            Wires = new List<Wire>();

            StreamReader sr = new StreamReader(fileStream);

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                Wires.Add(ReadWire(line));
            }
        }

        public Wire ReadWire(string wireLine)
        {
            var wire = new Wire(wireLine);
            return wire;
        }
    }
}
