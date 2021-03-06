﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
	public enum StatusEffectTypes
	{
		None = 0,

		// non volatile status
		No_status = 1,
		Brn_psn_or_par = 2,
		Poison = 3,
		BadPoison = 4,
		Paralysis = 5,
		Burn = 6,
		Freeze = 7,
		Sleep = 8,

		// volatile status
		Confusion = 9,
		Attract = 10,
		Bound = 14,
		Focus_energy = 15,
		Flinched = 17,
		Must_recharge = 18,
		Rage = 19,
		Substitute = 20,
		Destiny_bond = 21,
		Trapped = 22,
		Nightmare = 23,
		Cursed = 24,
		Foresight = 25,
		Tormented = 27,
		Leech_seeded = 28,
		Locked_on_to = 29,
		Perish_song = 30,
		Fly = 31,
		Dig = 32,
		Dive = 33,
		Charge = 36,
		Ingrain = 37,
		Disabled = 41,
		Encored = 42,
		Protected = 43,
		Endure = 44,
		Pressure = 46,
		Bide = 47,
		Taunted = 48,
		Helping_hand = 50,
		Future_sight = 52,
		Choice_locked = 54,
		Magic_coat = 55,

		// field
		Mudsport = 56,
		Watersport = 57,

		// abilities
		Flash_fire = 58,
		Intimidated = 59,
		Traced_ability = 60,

		No_held_item = 61,

		// move effectiveness
		Reverse_mode = 62,
		Neutral = 63,
		Missed = 64,
		Super_effective = 65,
		Ineffective = 66,
		No_effect = 67,
		OHKO = 68,
		Failed = 69,
		Endured = 70,
		Hung_on = 71, // focus band

		// field effects
		Reflect = 72,
		Light_screen = 73,
		Spikes = 74,
		Safeguard = 75,
		Mist = 76,
		Follow_me = 77,

		// weather
		No_weather = 78,
		Permanent_sun = 79,
		Permanent_rain = 80,
		Permanent_sand = 81,
		Shadow_sky = 82,
		Hail = 83,
		Sun = 84,
		Rain = 85,
		Sandstorm = 86,
	}

	public class StatusEffects
    {
		private StatusEffectTypes effect;

		public StatusEffects(StatusEffectTypes effectType)
        {
			effect = effectType;
        }

		public int StartOffset()
		{
			return (int)(Constants.FirstStatusEffectOffset + ((int)effect * Constants.SizeOfStatusEffect));
		}

		public int NameID(ISOExtractor file)
		{
			return file.ISOStream.GetUShortAtOffset(StartOffset() + Constants.StatusEffectNameIDOffset);
		}

		public int Duration(ISOExtractor file)
		{
			return file.ISOStream.GetByteAtOffset(StartOffset() + Constants.StatusEffectDurationOffset);
		}

		public void SetDuration(ISOExtractor file, int turns)
		{
			var newBytes = BitConverter.GetBytes(turns);
			file.ISOStream.Seek(StartOffset() + Constants.StatusEffectDurationOffset, SeekOrigin.Begin);
			file.ISOStream.Write(newBytes);
		}
	}
}
