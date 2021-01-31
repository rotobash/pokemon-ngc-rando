using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public static partial class Pokemon
    {
        public enum Stats
        {
			Attack = 1,
			Defense = 2,
			Speed = 3,
			SpecialAttack = 4,
			SpecialDefense = 5,
			Accuracy = 6,
			Evasion = 7
        }

		public static int Mask(this Stats stats)
        {
            return (int)Math.Pow(2, (double)stats);
        }

        public static int MaskForStats(IEnumerable<Stats> stats) {
			var mask = 0;
			foreach (var stat in stats) {
				mask |= stat.Mask();
			}
			return mask;
		}
    }
}
