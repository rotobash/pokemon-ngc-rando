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
            252, 253, 254, 255, 256, 257, 258, 259, 260, 261, 262,
            263, 264, 265, 266, 267, 268, 269, 270, 271, 272, 273,
            274, 275, 276, 412
        };

        public static readonly List<int> Legendaries = new List<int>
        {
            144, 145, 146, 150, 151, 243, 244, 245, 249, 250,
            251, 377, 378, 379, 380, 381, 382, 383, 384, 385, 386
        };

        public static readonly List<int> InvalidItemList = new List<int>
        {
            0, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 72, 82,
            87, 88, 89, 90, 91, 92, 99, 100, 101, 102, 105, 112, 113, 
            114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 125, 126,
            127, 128, 129, 130, 131, 132, 175, 176, 177, 178, 226, 227,
            228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239,
            240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251,
            252, 253, 259, 260, 261, 262, 263, 264, 265, 266, 267, 268, 
            269, 270, 271, 272, 273, 274, 275, 276, 277, 278, 279, 280,
            281, 282, 283, 284, 285, 286, 287, 288, 339, 340, 341, 342,
            343, 344, 345, 346, 347, 348, 349, 370, 371, 372, 
        };

        public static readonly List<int> KeyItems = new List<int>
        {
            // essential story items
            352, 353, 354, 355, 356, 357, 358, 359, 360, 362, 368,
            369, 383
        };

        public static readonly List<int> NonEssentialKeyItems = new List<int>
        {
            // Krane Memos, Voice Case
            373, 374, 375, 376, 377, 378, 379, 380, 381, 382
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
        };

        public const int MetronomeIndex = 118;
        public const int WonderGuardIndex = 118;
    }
}
