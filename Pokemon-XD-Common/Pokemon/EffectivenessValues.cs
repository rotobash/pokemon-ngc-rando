using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public static partial class Pokemon
    {
        public enum EffectivenessValues
        {
            Ineffective = 0x43,
            NotVEryEffective = 0x42,
            SuperEffective = 0x41,
            Missed = 0x40,
            Neutral = 0x3F
        }
    }
}
