using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinArchipelagoClient.Memory
{
    public class APData
    {
        byte[] data;

        public ushort Settings { get; set; }
        public byte QueueDeathLink { get; set; }

        public ushort itemCounter { get; set; }
        public APItem[] items { get; }
        public ushort PokemonCounter { get; set; }
        public APPokemon[] pokemon { get; }


        public APData()
        {

        }
    }
}
