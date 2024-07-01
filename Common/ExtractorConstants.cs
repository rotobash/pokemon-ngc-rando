using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDCommon
{
    public static class ExtractorConstants
    {
        // invalid pokemon
        public static readonly int[] SpecialPokemon = new int[]
        {
            0, 252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262,
            263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273,
            274, 275, 276, 412, 414
        };

        public static readonly int[] Legendaries = new int[]
        {
            144, 145, 146, 150, 151, 243, 244, 245, 249, 250,
            251, 401, 402, 403, 404, 405, 406, 407, 408, 409, 410
        };

        public static readonly int[] BattleCDList = Enumerable.Range(534, 61).ToArray();

        public static readonly int[] InvalidItemList = new int[]
        {
            0, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 72, 82,
            87, 88, 89, 90, 91, 92, 99, 100, 101, 102, 105, 112, 113,
            114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 
            126, 127, 128, 129, 130, 131, 132, 175, 176, 177, 178, 226,
            227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 
            239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250,
            251, 252, 253, 259, 260, 261, 262, 263, 264, 265, 266, 267,
            268, 269, 270, 271, 272, 273, 274, 275, 276, 278, 279, 280,
            281, 282, 283, 284, 285, 286, 287, 339, 340, 341, 342, 343, 
            344, 345, 346, 347, 348, 349, 370, 371, 372
        };

        public static readonly int[] BadItemList = new int[]
        {
            // anything to do with wild pokemon (i.e. repels, smoke ball, pokedoll, etc.),
            // contest effect items (i.e. red scarf, etc.), shards (except moon and sun), 
            // and berries
            40, 42, 46, 47, 48, 49, 50, 51, 80, 81, 83, 84, 85, 86, 194,
            133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144,
            145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156,
            157, 158, 159, 160, 161, 162, 153, 164, 165, 166, 167, 168, 
            169, 170, 171, 172, 173, 174, 254, 255, 256, 257, 258
        };

        // Water Stone, Thunder Stone, Fire Stone, Leaf Stone, Sun Stone, Moon Stone
        public static readonly ushort[] EvoStoneItemList = Enumerable.Range(93, 6).Select(i => (ushort)i).ToArray();

        public static readonly int[] BadAbilityList = new int[]
        {
            // Stench, Illuminate, Run Away, Truant, Cacophony
            1, 35, 50, 54, 76
        };

        public const ushort DragonRageIndex = 82;
        public const ushort MetronomeIndex = 118;
        public const int WonderGuardIndex = 25;
        public const int ShedinjaIndex = 303;
        public const int BonslyIndex = 413;

        public const int BanDragonRageUnderLevel = 20;

        public static readonly int[] AgateVillageMartIndices = new[] { 9, 10, 11 };
    }
}
