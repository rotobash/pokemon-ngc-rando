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
        public static void ShuffleTMs(ShuffleSettings shuffleSettings)
        {
            var settings = shuffleSettings.RandomizerSettings.ItemShufflerSettings;
            var extractedGame = shuffleSettings.ExtractedGame;
            var random = shuffleSettings.RNG;

            if (settings.RandomizeTMs)
            {
                Logger.Log("=============================== TMs ===============================\n\n");
                var tms = extractedGame.TMs;
                // use set to avoid dupes
                var newTMSet = new HashSet<ushort>();
                IEnumerable<Move> moveFilter = extractedGame.ValidMoves;

                if (settings.TMForceGoodDamagingMove)
                {
                    // determine what percent of TMs should be good
                    var count = (int)Math.Round(settings.TMGoodDamagingMovePercent * tms.Length);
                    // keep picking until it's not a dupe
                    // this could probably be done smarterly
                    var goodDamagingMoves = extractedGame.GoodDamagingMoves;
                    while (newTMSet.Count < count)
                    {
                        var newMove = random.NextElement(goodDamagingMoves);
                        newTMSet.Add((ushort)newMove.MoveIndex);
                    }
                }

                if (shuffleSettings.RandomizerSettings.TeamShufflerSettings.MoveSetOptions.BanShadowMoves)
                {
                    moveFilter = moveFilter.Where(m => !m.IsShadowMove);
                }

                // keep picking while we haven't picked enough TMs
                while (newTMSet.Count < tms.Length)
                    newTMSet.Add((ushort)random.NextElement(moveFilter).MoveIndex);

                // set them to the actual TM item
                for (int i = 0; i < tms.Length; i++)
                {
                    var tm = tms[i];
                    var newMove = extractedGame.MoveList[newTMSet.ElementAt(i)];
                    tm.Move = (ushort)newMove.MoveIndex;
                    tm.Description = newMove.Description;

                    Logger.Log($"TM{tm.TMIndex}: {extractedGame.MoveList[tm.Move].Name}\n");
                }
                Logger.Log($"\n\n");
            }
        }

        public static void ShuffleTutorMoves(ShuffleSettings shuffleSettings, TutorMove[] tutorMoves)
        {
            var settings = shuffleSettings.RandomizerSettings.ItemShufflerSettings;
            var extractedGame = shuffleSettings.ExtractedGame;
            var random = shuffleSettings.RNG;

            if (settings.RandomizeTutorMoves)
            {
                Logger.Log("=============================== Tutor Moves ===============================\n\n");
                var newTutorMoveSet = new HashSet<ushort>();
                IEnumerable<Move> moveFilter = extractedGame.ValidMoves;

                if (settings.TutorForceGoodDamagingMove)
                {
                    // determine what percent of TMs should be good
                    var count = (int)Math.Round(settings.TutorGoodDamagingMovePercent * tutorMoves.Length);
                    // filter the move list by moves that are deemed good
                    var goodDamagingMoves = extractedGame.GoodDamagingMoves;
                    while (newTutorMoveSet.Count < count)
                    {
                        var newMove = random.NextElement(goodDamagingMoves);
                        newTutorMoveSet.Add((ushort)newMove.MoveIndex);
                    }
                }

                if (shuffleSettings.RandomizerSettings.TeamShufflerSettings.MoveSetOptions.BanShadowMoves)
                {
                    moveFilter = moveFilter.Where(m => !m.IsShadowMove);
                }

                // keep picking while we haven't picked enough TMs or we picked a dupe
                while (newTutorMoveSet.Count < tutorMoves.Length)
                    newTutorMoveSet.Add((ushort)random.NextElement(moveFilter).MoveIndex);

                // set them to the actual TM item
                for (int i = 0; i < tutorMoves.Length; i++)
                {
                    var tutorMove = tutorMoves[i];
                    var newMove = extractedGame.MoveList[newTutorMoveSet.ElementAt(i)];
                    tutorMove.Move = (ushort)newMove.MoveIndex;

                    Logger.Log($"Tutor Move {i + 1}: {extractedGame.MoveList[tutorMoves[i].Move].Name}\n");
                }
                Logger.Log($"\n\n");
            }
        }

        public static void ShuffleOverworldItems(ShuffleSettings shuffleSettings)
        {
            var settings = shuffleSettings.RandomizerSettings.ItemShufflerSettings;
            var extractedGame = shuffleSettings.ExtractedGame;
            var random = shuffleSettings.RNG;

            // there are a lot of battle cds, so only add them to one location and then
            // block that same cd from being put in another location (only if they haven't banned cds entirely)
            var battleCDsUsed = new List<int>();
            var potentialItems = settings.BanBadItems ? extractedGame.GoodItems : extractedGame.NonKeyItems;
            if (settings.BanBattleCDs)
            {
                potentialItems = potentialItems.Where(i => !ExtractorConstants.BattleCDList.Contains(i.OriginalIndex)).ToArray();
            }

            Logger.Log("=============================== Overworld Items ===============================\n\n");
            foreach (var item in extractedGame.OverworldItemList)
            {
                // i'm *assuming* the devs didn't place any invalid items on the overworld
                var originalItem = extractedGame.ItemList.FirstOrDefault(i =>  i.OriginalIndex == item.Item);
                if (originalItem == null)
                    continue;

                // key items are distributed through chests and sparkles
                if (originalItem.BagSlot == BagSlots.KeyItems)
                    continue;

                if (settings.RandomizeItems)
                {
                    Items newItem = null;
                    while (newItem == null || battleCDsUsed.Contains(newItem.OriginalIndex))
                    {
                        var nextIndex = random.Next(0, potentialItems.Length);
                        newItem = potentialItems[nextIndex];
                    }
                    if (ExtractorConstants.BattleCDList.Contains(newItem.Index))
                        battleCDsUsed.Add(newItem.OriginalIndex);

                    Logger.Log($"{originalItem.Name} -> ");
                    item.Item = newItem.OriginalIndex;
                    Logger.Log($"{newItem.Name}\n\n");
                }

                if (settings.RandomizeItemQuantity)
                {
                    Logger.Log($"Quantity: {item.Quantity} -> ");
                    item.Quantity = (byte)random.Next(1, 6);
                    Logger.Log($"{item.Quantity}\n\n");
                }
            }
        }

        public static void UpdatePokemarts(ShuffleSettings shuffleSettings)
        {
            var settings = shuffleSettings.RandomizerSettings.ItemShufflerSettings;
            var extractedGame = shuffleSettings.ExtractedGame;
            var random = shuffleSettings.RNG;

            if (settings.RandomizeMarts)
            {
                Logger.Log("=============================== Mart Items ===============================\n\n");
                var potentialItems = settings.BanBadItems ? extractedGame.GoodItems : extractedGame.NonKeyItems;

                if (settings.BanBattleCDs)
                {
                    potentialItems = potentialItems.Where(i => !ExtractorConstants.BattleCDList.Contains(i.OriginalIndex)).ToArray();
                }

                foreach (var mart in extractedGame.Pokemarts)
                {
                    Logger.Log($"Mart {mart.Index}:\n");
                    for (int i = 0; i < mart.Items.Count; i++)
                    {
                        var item = mart.Items[i];
                        var martItem = extractedGame.ItemList.First(i => i.OriginalIndex == item);

                        if (martItem.BagSlot == BagSlots.KeyItems)
                        {
                            continue;
                        }

                        Logger.Log($"{martItem?.Name} -> ");

                        var nextItem = random.NextElement(potentialItems);
                        if (nextItem.Index == 1)
                            nextItem.Price = 10000;

                        mart.Items[i] = nextItem.OriginalIndex;
                        Logger.Log($"{nextItem.Name}\n");
                    }
                    mart.SaveItems();
                    Logger.Log($"\n");
                }
                Logger.Log($"\n");
            }

            if (settings.MartsSellEvoStones)
            {
                Logger.Log($"Adding Evolution Stones to Agate Village Mart.\n\n");
                foreach (var agateMartIndex in ExtractorConstants.AgateVillageMartIndices)
                {
                    var agateMart = extractedGame.Pokemarts[agateMartIndex];
                    agateMart.Items.AddRange(ExtractorConstants.EvoStoneItemList);
                    for (int i = agateMartIndex + 1; i < extractedGame.Pokemarts.Length; i++)
                    {
                        extractedGame.Pokemarts[i].FirstItemIndex += (ushort)(ExtractorConstants.EvoStoneItemList.Length);
                    }
                    agateMart.SaveItems();
                }
            }
        }
    }
}
