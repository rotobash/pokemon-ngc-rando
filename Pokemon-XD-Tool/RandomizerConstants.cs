using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer
{
    public static class RandomizerConstants
    {
        // invalid pokemon
        public static readonly List<int> SpecialPokemon = new List<int>
        {
            0, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262,
            263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273,
            274, 275, 276, 412
        };

        public static readonly List<int> Legendaries = new List<int>
        {
            144, 145, 146, 150, 151, 243, 244, 245, 249, 250,
            251, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386
        };

        public static readonly List<int> BadItemList = new List<int>
        {
            // anything to do with wild pokemon (i.e. repels, smoke ball, pokedoll, etc.),
            // contest effect items (i.e. red scarf, etc.), shards (except moon and sun), 
            // and berries
            40, 42, 46, 47, 48, 49, 50, 51, 80, 81, 83, 84, 85, 86, 194,
            133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144,
            145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156,
            157, 158, 159, 160, 161, 162, 153, 164, 165, 166, 167, 169,
            170, 171, 172, 173, 174, 254, 255, 256, 257, 258
        };

        public static readonly List<int> BadAbilityList = new List<int>
        {
            // Stench, Illuminate, Run Away, Truant, Cacophony
            1, 35, 50, 54, 76
        };

        public const int MetronomeIndex = 118;
        public const int WonderGuardIndex = 25;
        public const int ShedinjaIndex = 303;
        public const int BonslyIndex = 413;
    }
}
