using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace Randomizer.Shufflers
{
    public static class StaticPokemonShuffler
    {
        public static void RandomizeXDStatics(ShuffleSettings shuffleSettings, XDStarterPokemon starter, ISO iso, int[] pickedShadows)
        {
            var settings = shuffleSettings.RandomizerSettings.StaticPokemonShufflerSettings;
            var extractedGame = shuffleSettings.ExtractedGame;
            var random = shuffleSettings.RNG;

            RandomizeStarters(random, settings, extractedGame, starter);

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

                    Logger.Log($"Requested Pokemon: {string.Join(", ", newRequestedPokemon?.Select(p => p.Name) ?? ["None"])}\n");
                    Logger.Log($"Given Pokemon: {string.Join(", ", newGivenPokemon?.Select(p => p.Name) ?? ["None"])}\n");

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

        public static void RandomizeColoStatics(ShuffleSettings shuffleSettings, IGiftPokemon[] starters)
        {
            var settings = shuffleSettings.RandomizerSettings.StaticPokemonShufflerSettings;
            var extractedGame = shuffleSettings.ExtractedGame;
            var random = shuffleSettings.RNG;

            if (starters.Length != 2)
                return;

            RandomizeStarters(random, settings, extractedGame, starters[0], starters[1]);

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

        private static void RandomizeStarters(AbstractRNG random, StaticPokemonShufflerSettings settings, ExtractedGame extractedGame, IGiftPokemon starter1, IGiftPokemon starter2 = null)
        {
            Logger.Log("=============================== Starters ===============================\n\n");

            if (settings.Starter == StarterRandomSetting.Custom)
            {
                starter1.Pokemon = (ushort?)extractedGame.PokemonList.FirstOrDefault(p => p.Name.ToLower() == settings.Starter1.ToLower())?.Index ?? starter1.Pokemon;
                if (starter2 != null)
                    starter2.Pokemon = (ushort?)extractedGame.PokemonList.FirstOrDefault(p => p.Name.ToLower() == settings.Starter2.ToLower())?.Index ?? starter2.Pokemon;
            }
            else
            {
                RandomizeStarter(random, settings, extractedGame, starter1);
                RandomizeStarter(random, settings, extractedGame, starter2);
            }

            Logger.Log($"Your new starter is {extractedGame.PokemonList[starter1.Pokemon].Name}\n");
            UpdateStarterMoveset(random, settings, extractedGame, starter1);

            if (starter2 != null)
            {
                Logger.Log($"Your new starter is {extractedGame.PokemonList[starter2.Pokemon].Name}\n");
                UpdateStarterMoveset(random, settings, extractedGame, starter2);
            }

        }

        private static void RandomizeStarter(AbstractRNG random, StaticPokemonShufflerSettings settings, ExtractedGame extractedGame, IGiftPokemon starter)
        {
            if (starter == null)
                return;

            int index = 0;
            Evolution secondStage;
            bool condition = false;
            switch (settings.Starter)
            {
                case StarterRandomSetting.Random:
                {
                    index = extractedGame.ValidPokemon[random.Next(0, extractedGame.ValidPokemon.Length)].Index;
                }
                break;
                case StarterRandomSetting.RandomThreeStage:
                    while (!condition)
                    {
                        var newStarter = extractedGame.ValidPokemon[random.Next(0, extractedGame.ValidPokemon.Length)];
                        index = newStarter.Index;
                        secondStage = newStarter.Evolutions[0];
                        condition = !Helpers.CheckForSplitOrEndEvolution(newStarter, out int _)
                            && !Helpers.CheckForSplitOrEndEvolution(extractedGame.PokemonList[secondStage.EvolvesInto], out int _)
                            // check if any pokemon evolve into this one, less likely for three stages but probably good to check anyway
                            && !extractedGame.ValidPokemon.Any(p =>
                            {
                                for (int i = 0; i < p.Evolutions.Length; i++)
                                {
                                    if (p.Evolutions[i].EvolvesInto == index)
                                        return true;
                                }
                                return false;
                            });
                    }
                    break;
                case StarterRandomSetting.RandomTwoStage:
                    while (!condition)
                    {
                        var newStarter = extractedGame.ValidPokemon[random.Next(0, extractedGame.ValidPokemon.Length)];
                        index = newStarter.Index;
                        secondStage = newStarter.Evolutions[0];
                        condition = !Helpers.CheckForSplitOrEndEvolution(newStarter, out int _)
                            && Helpers.CheckForSplitOrEndEvolution(extractedGame.PokemonList[secondStage.EvolvesInto], out int count)
                            && count == 0
                            // check if any pokemon evolve into this one
                            && !extractedGame.ValidPokemon.Any(p =>
                            {
                                for (int i = 0; i < p.Evolutions.Length; i++)
                                {
                                    if (p.Evolutions[i].EvolvesInto == index)
                                        return true;
                                }
                                return false;
                            });
                    }
                    break;
                case StarterRandomSetting.RandomSingleStage:
                    while (!condition)
                    {
                        var newStarter = extractedGame.ValidPokemon[random.Next(0, extractedGame.ValidPokemon.Length)];
                        index = newStarter.Index;
                        condition = Helpers.CheckForSplitOrEndEvolution(newStarter, out int count)
                            && count == 0
                            // check if any pokemon evolve into this one
                            && !extractedGame.ValidPokemon.Any(p =>
                            {
                                for (int i = 0; i < p.Evolutions.Length; i++)
                                {
                                    if (p.Evolutions[i].EvolvesInto == index)
                                        return true;
                                }
                                return false;
                            });
                    }
                    break;
                default:
                case StarterRandomSetting.Unchanged:
                    index = starter.Pokemon;
                    break;
            }
            starter.Pokemon = (ushort)index;
        }

        private static void UpdateStarterMoveset(AbstractRNG random, StaticPokemonShufflerSettings settings, ExtractedGame extractedGame, IGiftPokemon starter)
        {
            // instance pokemon have separate movesets than the pool
            // i.e. if you don't update the moveset than your starter will have Eevee's move set
            ushort[] moves;
            if (settings.MoveSetOptions.RandomizeMovesets || settings.Starter != StarterRandomSetting.Unchanged)
            {
                Logger.Log($"It knows:\n");
                moves = Helpers.GetNewMoveset(random, settings.MoveSetOptions, starter.Pokemon, starter.Level, extractedGame);
                if (moves == null) return;

                for (int j = 0; j < moves.Length; j++)
                {
                    var move = moves[j];
                    Logger.Log($"{extractedGame.MoveList[move].Name}\n");
                    starter.SetMove(j, move);
                }
            }
        }
    }
}
