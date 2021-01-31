using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public static partial class Pokemon
    {
        public enum CMColosseumRounds
        {
            None = 0,
            Special = 0x95,
            First = 0xD4,
            Second = 0xD5,
            Semifinal = 0xD7,
            Final = 0xD6,
        }

        public enum XDColosseumRounds
        {
            None = 0,
            First = 0x95,
            Second = 0x96,
            Semifinal = 0x9B,
            Final = 0x9C,
        }
    }
}
