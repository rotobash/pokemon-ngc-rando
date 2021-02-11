using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.PokemonDefinitions;

namespace Randomizer.Shufflers
{
    public struct ItemShufflerSettings
    {
        public bool RandomizeItems;
        public bool RandomizeTMs;
        public bool RandomizeTutorMoves;

        public bool TMForceGoodDamagingMove;
        public float TMGoodDamagingMovePercent;
        public bool TutorForceGoodDamagingMove;
        public float TutorGoodDamagingMovePercent;
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
            if (settings.RandomizeTMs) 
            {
                var tms = items.Where(x => x is TM).Select(x => x as TM).ToArray();
                // use set to avoid dupes
                var newTMSet = new HashSet<ushort>();

                if (settings.TMForceGoodDamagingMove)
                {
                    // determine what percent of TMs should be good
                    var count = (int)Math.Round(settings.TMGoodDamagingMovePercent * tms.Length);
                    // filter the move list by moves that are deemed good
                    var goodMoves = moveList.Where(m => m.BasePower >= Configuration.GoodDamagingMovePower).ToList();
                    for (int i = 0; i < count; i++)
                    {
                        // keep picking until it's not a dupe
                        // this could probably be done smarterly
                        while (!newTMSet.Add((ushort)random.Next(0, moveList.Length)));
                    }
                }

                // keep picking while we haven't picked enough TMs
                while (newTMSet.Count < tms.Length)
                    newTMSet.Add((ushort)random.Next(0, moveList.Length));

                // set them to the actual TM item
                for (int i = 0; i < tms.Length; i++)
                {
                    tms[i].Move = newTMSet.ElementAt(i);
                }
            }
        }

        public static void ShuffleTutorMoves(Random random, ItemShufflerSettings settings, TutorMove[] tutorMoves, Move[] moves)
        {

            if (settings.RandomizeTutorMoves)
            {
                var newTutorMoveSet = new HashSet<ushort>();

                if (settings.TutorForceGoodDamagingMove)
                {
                    // determine what percent of TMs should be good
                    var count = (int)Math.Round(settings.TutorGoodDamagingMovePercent * tutorMoves.Length);
                    // filter the move list by moves that are deemed good
                    var goodMoves = moves.Where(m => m.BasePower >= Configuration.GoodDamagingMovePower).ToList();
                    for (int i = 0; i < count; i++)
                    {
                        // keep picking until it's not a dupe
                        // this could probably be done smarterly
                        while (!newTutorMoveSet.Add((ushort)random.Next(0, moves.Length))) ;
                    }
                }

                // keep picking while we haven't picked enough TMs or we picked a dupe
                while (newTutorMoveSet.Count < tutorMoves.Length)
                    newTutorMoveSet.Add((ushort)random.Next(0, moves.Length));

                // set them to the actual TM item
                for (int i = 0; i < tutorMoves.Length; i++)
                {
                    tutorMoves[i].Move = newTutorMoveSet.ElementAt(i);
                }
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
