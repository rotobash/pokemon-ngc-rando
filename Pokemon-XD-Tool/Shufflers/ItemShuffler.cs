using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;

namespace Randomizer.Shufflers
{
    public struct ItemShufflerSettings
    {
        public bool RandomizeItems;
    }

    public static class ItemShuffler
    {
        public static void ShuffleItems(Random random, ItemShufflerSettings settings, Items[] items)
        {
            if (settings.RandomizeItems)
            {

            }
        }

        public static void ShuffleTMs(Random random, ItemShufflerSettings settings, Items[] items, Move[] moveList)
        {
            var tms = items.Where(x => x is TM).Select(x => x as TM);
            foreach (var tm in tms)
            {
                tm.Move = (ushort)random.Next(0, moveList.Length);
            }
        }

        public static void ShuffleOverworldItems(Random random, ItemShufflerSettings settings, OverworldItem[] overworldItems, Items[] items)
        {
            foreach (var item in overworldItems)
            {
                item.Item = (ushort)random.Next(0, items.Length);
            }
        }

        public static void UpdatePokemarts(ItemShufflerSettings settings, Pokemarts[] marts, Items[] items)
        {
            foreach (var mart in marts)
            {
                
            }
        }
    }
}
