using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public static partial class Pokemon
    {
        public enum StatStages
        {
			// legit
			Plus1 = 0x10,
			Plus2 = 0x20,
	
			// must be added through hax
			Plus3 = 0x30,
			Plus4 = 0x40,
			Plus5 = 0x50,
			Plus6 = 0x60,
	
			// legit
			Minus1 = 0x90,
			Minus2 = 0xa0,
	
			// must be added through hax
			Minus3 = 0xb0,
			Minus4 = 0xc0,
			Minus5 = 0xd0,
			Minus6 = 0xe0,
        }

		public static int TrueValue(this StatStages stages)
        {
			var value = (int)stages;
			if (value < 0x90) {
				return value / 0x10;
			} else {
				return -((value - 0x80) / 0x10);
			}
        }
    }
}
