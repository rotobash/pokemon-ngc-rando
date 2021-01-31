using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public static partial class Pokemon
    {
        public enum GenderRatios
        {
            MaleOnly = 0,
            Male87 = 0x1F,
            Male75 = 0x3F,
            Male50 = 0x7F,
            FemaleOnly = 0xFE,
            Female87 = 0xDF,
            Female75 = 0xBF,
            Genderless = 0xFF
        }
    }
}
