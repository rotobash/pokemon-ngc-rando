using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchipelagoClient.Controls
{
    public class Credential
    {
        public required string Url { get; set; }
        public required string Slotname { get; set; }
        public string? Password { get; set; } = null;
    }
}
