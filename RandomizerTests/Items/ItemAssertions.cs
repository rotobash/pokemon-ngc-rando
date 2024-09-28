using Newtonsoft.Json;
using XDCommon.Shufflers;
using RandomizerTests.ReferenceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.PokemonDefinitions;

namespace RandomizerTests.Items
{
    public static class ItemAssertions
    {
        public static List<ItemReference> ValidItemReference = JsonConvert.DeserializeObject<List<ItemReference>>(File.ReadAllText("ReferenceData\\JSON\\items.json")) ?? new List<ItemReference>();
        public static string CheckTrainerHeldItems(ITrainerPool[] trainerPools, Settings settings)
        {
            var randomizedPokemon = new HashSet<int>();
            foreach (var pool in trainerPools)
            {
                foreach (var trainer in pool.AllTrainers)
                {
                    foreach (var partypoke in trainer.Pokemon)
                    {
                        if (!partypoke.IsSet) continue;
                        if (randomizedPokemon.Contains(partypoke.Index))
                            continue;
                        else
                            randomizedPokemon.Add(partypoke.Index);

                        var reference = ValidItemReference.Find(i => i.Index == partypoke.Item);
                        if (reference == null)
                        {
                            return $"Item {partypoke.Item} not found in reference list.";
                        }
                        else if (reference.Name.Equals("unknown", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return $"Item {partypoke.Item} is an unknown item, this will be junk.";
                        }
                        else if (settings.TeamShufflerSettings.BanBadItems && ExtractorConstants.BadItemList.Contains(reference.Index))
                        {
                            return $"Item {reference.Name} is marked as a bad item but was selected for randomization.";
                        }
                        else if (settings.TeamShufflerSettings.BanBattleCDs && ExtractorConstants.BattleCDList.Contains(reference.Index))
                        {
                            return $"Item {reference.Name} was selected for randomization but BanBattleCDs was selected.";
                        }
                    }
                }
            }
            return string.Empty;
        }

        public static string CheckOverworldItems(OverworldItem[] overworldItems, Settings settings)
        {
            foreach (var item in overworldItems)
            {
                var reference = ValidItemReference.Find(i => i.Index == item.Item);
                if (reference == null)
                {
                    return $"Item {item.Item} not found in reference list.";
                }
                else if (reference.Name.Equals("unknown", StringComparison.InvariantCultureIgnoreCase))
                {
                    return $"Item {item.Item} is an unknown item, this will be junk.";
                }
                else if (settings.ItemShufflerSettings.BanBadItems && ExtractorConstants.BadItemList.Contains(reference.Index))
                {
                    return $"Item {reference.Name} is marked as a bad item but was selected for randomization.";
                }
                else if (settings.ItemShufflerSettings.BanBattleCDs && ExtractorConstants.BattleCDList.Contains(reference.Index))
                {
                    return $"Item {reference.Name} is banned but was selected for randomization.";
                }
                else if (settings.ItemShufflerSettings.MakeItemsBoxes && item.Model == (byte)TreasureTypes.Sparkle)
                {
                    return $"Item {reference?.Name} was set as a sparkle.";
                }
                else if (settings.ItemShufflerSettings.MakeItemsSparkles && item.Model == (byte)TreasureTypes.Chest)
                {
                    return $"Item {reference?.Name} was set as a chest.";
                }
                else if (settings.ItemShufflerSettings.RandomizeItemQuantity)
                {
                    // todo: how to test this?
                }
            }

            return string.Empty;
        }

        public static string CheckPokemarts(Pokemarts[] marts, Settings settings)
        {
            foreach (var mart in marts)
            {
                foreach (var item in mart.Items)
                {
                    var reference = ValidItemReference.Find(i => i.Index == item);
                    if (reference == null)
                    {
                        return $"Item {item} not found in reference list.";
                    }
                    else if (reference.Name.Equals("unknown", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return $"Item {item} is an unknown item, this will be junk.";
                    }
                    else if (settings.ItemShufflerSettings.BanBadItems && ExtractorConstants.BadItemList.Contains(reference.Index))
                    {
                        return $"Item {reference.Name} is marked as a bad item but was selected for randomization.";
                    }
                    else if (settings.ItemShufflerSettings.BanBattleCDs && ExtractorConstants.BattleCDList.Contains(reference.Index))
                    {
                        return $"Item {reference.Name} is banned but was selected for randomization.";
                    }
                }

                if (ExtractorConstants.AgateVillageMartIndices.Contains(mart.Index))
                {
                    if (settings.ItemShufflerSettings.MartsSellEvoStones && !mart.Items.Any(i => ExtractorConstants.EvoStoneItemList.Contains(i)))
                    {
                        return $"Evolution stones were not added to the Agate mart.";
                    }
                    else if (settings.ItemShufflerSettings.MartsSellXItems && !mart.Items.Any(i => ExtractorConstants.XItemList.Contains(i)))
                    {
                        return $"X Items were not added to the Agate mart.";
                    }
                }
            }

            return string.Empty;
        }
    }
}
