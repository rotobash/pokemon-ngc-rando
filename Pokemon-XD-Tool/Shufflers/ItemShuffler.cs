using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.PokemonDefinitions;

namespace Randomizer.Shufflers
{
    public static class ItemShuffler
    {
        public static void ShuffleTMs(Random random, ItemShufflerSettings settings, ExtractedGame extractedGame)
        {
            if (settings.RandomizeTMs) 
            {
                var tms = extractedGame.ItemList.Where(x => x is TM).Select(x => x as TM).ToArray();
                // use set to avoid dupes
                var newTMSet = new HashSet<ushort>();

                if (settings.TMForceGoodDamagingMove)
                {
                    // determine what percent of TMs should be good
                    var count = (int)Math.Round(settings.TMGoodDamagingMovePercent * tms.Length);
                    // keep picking until it's not a dupe
                    // this could probably be done smarterly
                    var goodDamagingMoves = extractedGame.GoodDamagingMoves;
                    while (newTMSet.Count < count)
                    {
                        var newMove = goodDamagingMoves[random.Next(0, goodDamagingMoves.Length)];
                        newTMSet.Add((ushort)newMove.MoveIndex);
                    }
                }

                // keep picking while we haven't picked enough TMs
                while (newTMSet.Count < tms.Length)
                    newTMSet.Add((ushort)random.Next(1, extractedGame.MoveList.Length));

                // set them to the actual TM item
                for (int i = 0; i < tms.Length; i++)
                {
                    tms[i].Move = newTMSet.ElementAt(i);
                }
            }
        }

        public static void ShuffleTutorMoves(Random random, ItemShufflerSettings settings, TutorMove[] tutorMoves, ExtractedGame extractedGame)
        {

            if (settings.RandomizeTutorMoves)
            {
                var newTutorMoveSet = new HashSet<ushort>();

                if (settings.TutorForceGoodDamagingMove)
                {
                    // determine what percent of TMs should be good
                    var count = (int)Math.Round(settings.TutorGoodDamagingMovePercent * tutorMoves.Length);
                    // filter the move list by moves that are deemed good
                    var goodDamagingMoves = extractedGame.GoodDamagingMoves;
                    while (newTutorMoveSet.Count < count)
                    {
                        var newMove = goodDamagingMoves[random.Next(0, goodDamagingMoves.Length)];
                        newTutorMoveSet.Add((ushort)newMove.MoveIndex);
                    }
                }

                // keep picking while we haven't picked enough TMs or we picked a dupe
                while (newTutorMoveSet.Count < tutorMoves.Length)
                    newTutorMoveSet.Add((ushort)random.Next(1, extractedGame.MoveList.Length));

                // set them to the actual TM item
                for (int i = 0; i < tutorMoves.Length; i++)
                {
                    tutorMoves[i].Move = newTutorMoveSet.ElementAt(i);
                }
            }
        }

        public static void ShuffleOverworldItems(Random random, ItemShufflerSettings settings, ExtractedGame extractedGame)
        {
            var potentialItems = settings.BanBadItems ? extractedGame.NonKeyItems : extractedGame.GoodItems;

            foreach (var item in extractedGame.OverworldItemList)
            {
                // i'm *assuming* the devs didn't place any invalid items on the overworld
                if (item.Item > extractedGame.ItemList.Length || extractedGame.ItemList[item.Item].BagSlot == BagSlots.KeyItems)
                    continue;

                if (settings.RandomizeItems)
                {
                    item.Item = (ushort)potentialItems[random.Next(0, potentialItems.Length)].Index;
                }
                if (settings.RandomizeItemQuantity)
                {
                    item.Quantity = (byte)random.Next(1, 6);
                }

            }
        }

        public static void UpdatePokemarts(Random random, ItemShufflerSettings settings, ExtractedGame extractedGame)
        {
            foreach (var mart in extractedGame.Pokemarts)
            {
                
            }
        }
    }
}
