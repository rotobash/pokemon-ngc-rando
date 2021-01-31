using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;

namespace XDCommon.PokemonDefinitions
{
	public static partial class Pokemon 
	{
		public enum Natures
		{
			Hardy = 0x00,
			Lonely = 0x01,
			Brave = 0x02,
			Adamant = 0x03,
			Naughty = 0x04,

			Bold = 0x05,
			Docile = 0x06,
			Relaxed = 0x07,
			Impish = 0x08,
			Lax = 0x09,

			Timid = 0x0A,
			Hasty = 0x0B,
			Serious = 0x0C,
			Jolly = 0x0D,
			Naive = 0x0E,

			Modest = 0x0F,
			Mild = 0x10,
			Quiet = 0x11,
			Bashful = 0x12,
			Rash = 0x13,

			Calm = 0x14,
			Gentle = 0x15,
			Sassy = 0x16,
			Careful = 0x17,
			Quirky = 0x18,

			Random = 0xFF
		}
	}
}
