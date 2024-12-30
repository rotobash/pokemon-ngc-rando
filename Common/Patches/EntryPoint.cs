using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;

namespace XDCommon.Patches
{

    public class EntryPoint
    {
        public uint Pointer { get; set; }

        /// <summary>
        /// Source register that contains data to be manipulated. 
        /// </summary>
        public uint Source { get; set; }
        /// <summary>
        /// Destination register that will contain the manipulated data
        /// </summary>
        public uint Destination { get; set; }
        /// <summary>
        /// return address
        /// </summary>
        public uint? Next { get; set; }
    }

    public class RegionEntryPoints : Dictionary<Region, EntryPoint[]>
    {
    }
}
