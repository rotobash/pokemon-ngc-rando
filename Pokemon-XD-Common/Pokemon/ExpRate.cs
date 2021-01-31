using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public static partial class Pokemon
    {
        public enum ExpRate
        {
            Standard,
            VeryFast,
            Slowest,
            Slow,
            Fast,
            VerySlow
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

        private static int ExpForLevelVeryFast(int level)
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
                var modifier = Math.Floor((double)(1911 - (10* level)) / 3);
                return (int)(Math.Pow(level, 3) * modifier / 500);
            }
            else
            {
                return (int)(Math.Pow(level, 3) * (160 - level) / 100);
            }
        }
        
        private static int ExpForLevelFast(int level)
        {
            return (int)(4 * Math.Pow(level, 3) / 5);
        }
        
        private static int ExpForLevelVerySlow(int level)
        {
            return (int)(5 * Math.Pow(level, 3) / 4);
        }
        
        private static int ExpForLevelSlow(int level)
        {
            var exp = (6 * Math.Pow(level, 3) / 5) - (15 * Math.Pow(level, 2)) + (100 * level) - 140;
            return (int)exp;
        }
        
        private static int ExpForLevelSlowest(int level)
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
        
        private static int ExpForLevelStandard(int level)
        {
            return (int)Math.Pow(level, 3);
        }
    }
}
