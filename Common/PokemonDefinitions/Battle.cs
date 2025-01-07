using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class Battle
    {
        public readonly int Index = 0;
        protected ISO iso;

        public Battle(int index, ISO iso)
        { 
            Index = index;
            this.iso = iso;
        }
    }
}
