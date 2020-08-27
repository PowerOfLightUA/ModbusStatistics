using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace F28027TempTest.Model
{
    class SerialLogEntity
    {
        private List<byte> _bytes;

        public DateTime Time { get; set; }

        public List<byte> Bytes
        {
            get
            {
                return _bytes;
            }
            set
            {
                _bytes = value;
            }
        }

        public string Type { get; set; }

        public string Text
        {
            get
            {
                return Encoding.ASCII.GetString(_bytes.ToArray());
            }
        }

        public string Hex
        {
            get
            {
                return BitConverter.ToString(_bytes.ToArray());
            }
        }

        public SerialLogEntity(DateTime time, string type, List<byte> bytes)
        {
            Bytes = bytes;
            Type = type;
            Time = time;
        }
    }
}
