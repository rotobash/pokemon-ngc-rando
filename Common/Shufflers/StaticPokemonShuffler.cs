using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace XDCommon.Shufflers
{
    public static class StaticPokemonShuffler
    {
        public static void RandomizeXDStatics(ShuffleSettings shuffleSettings, XDStarterPokemon starter, ISO iso, int[] pickedShadows)
        {
            var settings = shuffleSettings.RandomizerSettings.StaticPokemonShufflerSettings;
            var extractedGame = shuffleSettings.ExtractedGame;
            var random = shuffleSettings.RNG;

            RandomizeStarters(random, shuffleSettings, extractedGame, pickedShadows, starter);

            Logger.Log("=============================== Trades ===============================\n\n");
            // set given
            switch (settings.Trade)
            {
                default:
                case TradeRandomSetting.Unchanged:
                    Logger.Log("Unchanged\n\n");
                    return;
                case TradeRandomSetting.Given:
                case TradeRandomSetting.Requested:
                case TradeRandomSetting.Both:
                    var (newGivenPokemon, newRequestedPokemon, hordelGivenShadow, hordelTrade) = BuildTradeList(shuffleSettings, pickedShadows);
                    XDTradePokemon.UpdateDukingTrades(iso, newRequestedPokemon, newGivenPokemon);
                    XDTradePokemon.UpdateHordelTrade(iso, hordelGivenShadow, hordelTrade);

                    Logger.Log($"Requested Pokemon: {string.Join(", ", newRequestedPokemon?.Select(p => p.Name) ?? new string[] { "None" })}\n");
                    Logger.Log($"Given Pokemon: {string.Join(", ", newGivenPokemon?.Select(p => p.Name) ?? new string[] { "None" })}\n");

                    Logger.Log($"Hordel Shadow Pokemon: {string.Join(", ", hordelGivenShadow?.Name ?? "None")}\n");
                    Logger.Log($"Hordel Trade Pokemon: {string.Join(", ", hordelTrade?.Name ?? "None")}\n");
                    break;
            }

        }

        private static (Pokemon[], Pokemon[], Pokemon, Pokemon) BuildTradeList(ShuffleSettings shuffleSettings, int[] pickedShadows)
        {
            var settings = shuffleSettings.RandomizerSettings.StaticPokemonShufflerSettings;
            var extractedGame = shuffleSettings.ExtractedGame;
            var pokeFilter = pickedShadows.Select(i => extractedGame.PokemonList[i]);

            List<Pokemon> newRequestedPokemon;
            List<Pokemon> newGivenPokemon;
            Pokemon hordelGivenShadow = null;
            Pokemon hordelTradePokemon = null;

            if (settings.Trade == TradeRandomSetting.Given || settings.Trade == TradeRandomSetting.Both)
            {
                newGivenPokemon = new List<Pokemon>();
                foreach (var giftPoke in extractedGame.GiftPokemonList)
                {
                    if (giftPoke.GiftType.Contains("Duking", StringComparison.CurrentCultureIgnoreCase))
                    {
                        var newpoke = Helpers.RandomizePokemon(shuffleSettings, giftPoke);
                        newGivenPokemon.Add(newpoke);
                    }
                    else if (giftPoke is XDShadowGiftPokemon hordelGivenShadowGift)
                    {
                        hordelGivenShadow = Helpers.RandomizePokemon(shuffleSettings, hordelGivenShadowGift);
                    }
                    else if (giftPoke.GiftType.Contains("Hordel", StringComparison.CurrentCultureIgnoreCase))
                    {
                        hordelTradePokemon = Helpers.RandomizePokemon(shuffleSettings, giftPoke);
                    }
                }
            }
            else
            {
                newGivenPokemon = null;
            }



            if (settings.Trade == TradeRandomSetting.Requested || settings.Trade == TradeRandomSetting.Both)
            {
                newRequestedPokemon = new List<Pokemon>();

                if (settings.UsePokeSpotPokemonInTrade)
                {
                    PokeSpotPokemon pokeSpotPokemon = null;
                    while (newRequestedPokemon.Count < 3)
                    {
                        pokeSpotPokemon = shuffleSettings.RNG.NextElement(extractedGame.PokeSpotPokemon);

                        if (!newRequestedPokemon.Any(p => p.Index == pokeSpotPokemon?.Pokemon))
                            newRequestedPokemon.Add(extractedGame.PokemonList[pokeSpotPokemon.Pokemon]);
                    }
                }
                else
                {
                    Pokemon newPoke = null;
                    while (newRequestedPokemon.Count < 3)
                    {
                        newPoke = Helpers.RandomizePokemon(shuffleSettings);

                        if (!newRequestedPokemon.Any(p => p.Index == newPoke.Index))
                            newRequestedPokemon.Add(newPoke);
                    }
                }
            }
            else
            {
                newRequestedPokemon = null;
            }

            return (newGivenPokemon?.ToArray(), newRequestedPokemon?.ToArray(), hordelGivenShadow, hordelTradePokemon);
        }

        public static void RandomizeColoStatics(ShuffleSettings shuffleSettings, IGiftPokemon[] starters, int[] pickedShadows)
        {
            var settings = shuffleSettings.RandomizerSettings.StaticPokemonShufflerSettings;
            var extractedGame = shuffleSettings.ExtractedGame;
            var random = shuffleSettings.RNG;

            if (starters.Length != 2)
                return;

            RandomizeStarters(random, shuffleSettings, extractedGame, pickedShadows, starters[0], starters[1]);

            List<Pokemon> newGivenPokemon = new List<Pokemon>();
            var pokemon = extractedGame.ValidPokemon;
            Logger.Log("=============================== Trades ===============================\n\n");
            // set given
            switch (settings.Trade)
            {
                default:
                case TradeRandomSetting.Unchanged:
                    Logger.Log("Unchanged\n\n");
                    return;
                case TradeRandomSetting.Given:
                    for (int i = 0; i < extractedGame.GiftPokemonList.Length; i++)
                    {
                        var giftPoke = extractedGame.GiftPokemonList[i];
                        Helpers.RandomizePokemon(shuffleSettings, giftPoke);
                    }
                    break;
            }

            Logger.Log($"Given Pokemon: {string.Join(", ", newGivenPokemon.Select(p => p.Name))}\n");
        }

        private static void RandomizeStarters(AbstractRNG random, ShuffleSettings shuffleSettings, ExtractedGame extractedGame, int[] pickedShadows, IGiftPokemon starter1, IGiftPokemon starter2 = null)
        {
            var settings = shuffleSettings.RandomizerSettings.StaticPokemonShufflerSettings;
            Logger.Log("=============================== Starters ===============================\n\n");

            Logger.Log($"Your new starter is:\n");
            if (settings.Starter == StarterRandomSetting.Custom)
            {
                starter1.Pokemon = (ushort?)extractedGame.PokemonList.FirstOrDefault(p => p.Name.ToLower() == settings.Starter1.ToLower())?.Index ?? starter1.Pokemon;
                if (starter2 != null)
                    starter2.Pokemon = (ushort?)extractedGame.PokemonList.FirstOrDefault(p => p.Name.ToLower() == settings.Starter2.ToLower())?.Index ?? starter2.Pokemon;
            }
            else
            {
                RandomizeStarter(shuffleSettings, extractedGame, starter1, pickedShadows);
                RandomizeStarter(shuffleSettings, extractedGame, starter2, pickedShadows);
            }

        }

        private static void RandomizeStarter(ShuffleSettings shuffleSettings, ExtractedGame extractedGame, IGiftPokemon starter, int[] pickedShadows)
        {
            if (starter == null)
                return;

            Evolution secondStage;
            bool condition = false;
            var settings = shuffleSettings.RandomizerSettings.StaticPokemonShufflerSettings;
            IEnumerable<Pokemon> pokeFilter = extractedGame.ValidPokemon;

            if (shuffleSettings.RandomizerSettings.TeamShufflerSettings.NoDuplicateShadows)
            {
                pokeFilter = pokeFilter.Where(p => !pickedShadows.Contains(p.Index));
            }

            switch (settings.Starter)
            {
                case StarterRandomSetting.Random:
                {
                    Helpers.RandomizePokemon(shuffleSettings, starter);
                }
                break;
                case StarterRandomSetting.RandomThreeStage:
                    pokeFilter = extractedGame.ValidPokemon.Where(p => p.Evolutions.Any(e => e.EvolvesInto > 0) 
                                    && extractedGame.PokemonList
                                            .Where(evo => p.Evolutions.Any(e => e.EvolvesInto == evo.Index))
                                            .Any(evo => evo.Evolutions.Any(e => e.EvolvesInto > 0)));

                    Helpers.RandomizePokemon(shuffleSettings, starter, pokeFilter);
                    break;
                case StarterRandomSetting.RandomTwoStage:
                    pokeFilter = extractedGame.ValidPokemon
                                    .Where(p => p.Evolutions.Any(e => e.EvolvesInto > 0)
                                                && extractedGame.PokemonList.Where(evo => p.Evolutions.Any(e => e.EvolvesInto == evo.Index))
                                                                .Any(evo => Helpers.CheckForSplitOrEndEvolution(evo, out int count) && count == 0));
                    Helpers.RandomizePokemon(shuffleSettings, starter, pokeFilter);
                    break;
                case StarterRandomSetting.RandomSingleStage:
                    pokeFilter = extractedGame.ValidPokemon
                                    .Where(p => p.Evolutions.All(e => e.EvolvesInto == 0));
                    Helpers.RandomizePokemon(shuffleSettings, starter, pokeFilter);
                    break;
                default:
                case StarterRandomSetting.Unchanged:
                    break;
            }
        }
    }
}
