using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions.XD.SaveData
{
    public class InventoryItem
    {
        public const int SizeOfData = 0x4;

        public ushort Index { get; set; }
        public ushort Quantity { get; set; }
    }

}
