using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public static class PokemonExtensions
	{
		public static int TrueValue(this StatStages stages)
		{
			var value = (int)stages;
			if (value < 0x90)
			{
				return value / 0x10;
			}
			else
			{
				return -((value - 0x80) / 0x10);
			}
		}

		public static int Mask(this StatTypes stats)
		{
			return (int)Math.Pow(2, (double)stats);
		}

		public static int MaskForStats(IEnumerable<StatTypes> stats)
		{
			var mask = 0;
			foreach (var stat in stats)
			{
				mask |= stat.Mask();
			}
			return mask;
        }

        public static string GetCode(this Maps map)
        {
            return map switch
            {
                Maps.Demo => "B1",
                Maps.ShadowLab => "D1",
                Maps.MtBattle => "D2",
                Maps.SSLibra => "D3",
                Maps.RealgamTower => "D4",
                Maps.CipherKeyLair => "D5",
                Maps.CitadarkIsle => "D6",
                Maps.OrreColosseum => "D7",
                Maps.PhenacCity => "M1",
                Maps.PyriteTown => "M2",
                Maps.AgateVillage => "M3",
                Maps.TheUnder => "M4",
                Maps.PokemonHQ => "M5",
                Maps.GateonPort => "M6",
                Maps.OutskirtStand => "S1",
                Maps.SnagemHideout => "S2",
                Maps.KaminkosHouse => "S3",
                Maps.AncientColo => "T1",
                Maps.Pokespot => "es",
                _ => "Unknown",
            };
        }

        public static int MaxExp(this ExpRate expRate)
        {
            return expRate switch
            {
                ExpRate.Standard => 1000000,
                ExpRate.VeryFast => 600000,
                ExpRate.Slowest => 1640000,
                ExpRate.Slow => 1059860,
                ExpRate.Fast => 8000000,
                ExpRate.VerySlow => 1250000,
                _ => 1000000,
            };
        }

        public static int ExpForLevel(this ExpRate expRate, int level)
        {
            return expRate switch
            {
                ExpRate.Standard => 1000000,
                ExpRate.VeryFast => 600000,
                ExpRate.Slowest => 1640000,
                ExpRate.Slow => 1059860,
                ExpRate.Fast => 8000000,
                ExpRate.VerySlow => 1250000,
                _ => 1000000,
            };
        }

        public static int ExpForLevelVeryFast(int level)
        {
            if (level <= 50)
            {
                return (int)(Math.Pow(level, 3) * (100 - level) / 50);
            }
            else if (level <= 68)
            {
                return (int)(Math.Pow(level, 3) * (150 - level) / 100);
            }
            else if (level <= 98)
            {
                var modifier = Math.Floor((double)(1911 - (10 * level)) / 3);
                return (int)(Math.Pow(level, 3) * modifier / 500);
            }
            else
            {
                return (int)(Math.Pow(level, 3) * (160 - level) / 100);
            }
        }

        public static int ExpForLevelFast(int level)
        {
            return (int)(4 * Math.Pow(level, 3) / 5);
        }

        public static int ExpForLevelVerySlow(int level)
        {
            return (int)(5 * Math.Pow(level, 3) / 4);
        }

        public static int ExpForLevelSlow(int level)
        {
            var exp = (6 * Math.Pow(level, 3) / 5) - (15 * Math.Pow(level, 2)) + (100 * level) - 140;
            return (int)exp;
        }

        public static int ExpForLevelSlowest(int level)
        {
            if (level <= 15)
            {
                var modifier = Math.Floor((double)(level + 1) / 3);
                return (int)(Math.Pow(level, 3) * (modifier + 24) / 50);
            }
            else if (level <= 36)
            {
                return (int)(Math.Pow(level, 3) * (level + 14) / 50);
            }
            else
            {
                var modifier = Math.Floor((double)level / 2);
                return (int)(Math.Pow(level, 3) * (modifier + 32) / 50);
            }
        }

        public static int ExpForLevelStandard(int level)
        {
            return (int)Math.Pow(level, 3);
        }

        public static EvolutionConditionType ConditionType(this EvolutionMethods evolutionMethod)
        {
            switch (evolutionMethod)
            {
                case EvolutionMethods.LevelUp:
                case EvolutionMethods.MoreAttack:
                case EvolutionMethods.MoreDefense:
                case EvolutionMethods.EqualAttack:
                case EvolutionMethods.Silcoon:
                case EvolutionMethods.Cascoon:
                case EvolutionMethods.Ninjask:
                case EvolutionMethods.Shedinja:
                    return EvolutionConditionType.Level;
                case EvolutionMethods.TradeWithItem:
                case EvolutionMethods.EvolutionStone:
                case EvolutionMethods.LevelUpWithKeyItem:
                    return EvolutionConditionType.Item;
                default:
                    return EvolutionConditionType.None;
            };
        }
    }
}
