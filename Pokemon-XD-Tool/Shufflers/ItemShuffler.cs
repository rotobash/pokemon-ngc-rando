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
                var tms = extractedGame.TMs;
                // use set to avoid dupes
                var newTMSet = new HashSet<ushort>();
                var validMoves = extractedGame.ValidMoves;

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
                    newTMSet.Add((ushort)validMoves[random.Next(0, validMoves.Length)].MoveIndex);

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
                var validMoves = extractedGame.ValidMoves;

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
                    newTutorMoveSet.Add((ushort)validMoves[random.Next(0, validMoves.Length)].MoveIndex);

                // set them to the actual TM item
                for (int i = 0; i < tutorMoves.Length; i++)
                {
                    tutorMoves[i].Move = newTutorMoveSet.ElementAt(i);
                }
            }
        }

        public static void ShuffleOverworldItems(Random random, ItemShufflerSettings settings, ExtractedGame extractedGame)
        {
            // there are a lot of battle cds, so only add them to one location and then
            // block that same cd from being put in another location (only if they haven't banned cds entirely)
            var battleCDsUsed = new List<int>();
            var potentialItems = settings.BanBadItems ? extractedGame.NonKeyItems : extractedGame.GoodItems;
            if (settings.BanBattleCDs)
            {
                potentialItems = potentialItems.Where(i => !RandomizerConstants.BattleCDList.Contains(i.Index)).ToArray();
            }

            foreach (var item in extractedGame.OverworldItemList)
            {
                // i'm *assuming* the devs didn't place any invalid items on the overworld
                if (item.Item > extractedGame.ItemList.Length || extractedGame.ItemList[item.Item].BagSlot == BagSlots.KeyItems)
                    continue;

                if (settings.RandomizeItems)
                {
                    ushort newItem = 0;
                    while (newItem == 0 || battleCDsUsed.Contains(newItem))
                    {
                        newItem = (ushort)potentialItems[random.Next(0, potentialItems.Length)].Index;
                    }

                    if (RandomizerConstants.BattleCDList.Contains(newItem))
                        battleCDsUsed.Add(newItem);

                    item.Item = newItem;
                }

                if (settings.RandomizeItemQuantity)
                {
                    item.Quantity = (byte)random.Next(1, 6);
                }

            }
        }

        public static void UpdatePokemarts(Random random, ItemShufflerSettings settings, ExtractedGame extractedGame)
        {
            if (settings.RandomizeMarts)
            {
                var potentialItems = settings.BanBadItems ? extractedGame.NonKeyItems : extractedGame.GoodItems;
                potentialItems = potentialItems.Where(i => !RandomizerConstants.BattleCDList.Contains(i.Index)).ToArray();

                foreach (var mart in extractedGame.Pokemarts)
                {
                    for (int i = 0; i < mart.Items.Count; i++)
                    {
                        mart.Items[i] = (ushort)potentialItems[random.Next(0, potentialItems.Length)].Index;
                    }
                    mart.SaveItems();
                }
            }

            if (settings.MartsSellEvoStones)
            {
                foreach (var agateMartIndex in RandomizerConstants.AgateVillageMartIndices)
                {
                    var agateMart = extractedGame.Pokemarts[agateMartIndex];
                    agateMart.Items.AddRange(RandomizerConstants.EvoStoneItemList);
                    for (int i = agateMartIndex + 1; i < extractedGame.Pokemarts.Length; i++)
                    {
                        extractedGame.Pokemarts[i].FirstItemIndex += (ushort)(RandomizerConstants.EvoStoneItemList.Length);
                    }
                    agateMart.SaveItems();
                }
            }
        }
    }
}
