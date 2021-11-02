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
        public static void RandomizeXDStatics(AbstractRNG random, StaticPokemonShufflerSettings settings, XDStarterPokemon starter, ISO iso, ExtractedGame extractedGame)
        {
            RandomizeStarters(random, settings, extractedGame, starter);

            List<Pokemon> newRequestedPokemon = new List<Pokemon>();
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
                case TradeRandomSetting.Both:
                    for (int i = 0; i < extractedGame.GiftPokemonList.Length; i++)
                    {
                        var giftPoke = extractedGame.GiftPokemonList[i];
                        var newPoke = pokemon[random.Next(0, pokemon.Length)];
                        giftPoke.Pokemon = (ushort)newPoke.Index;

                        if (giftPoke.GiftType.Contains("Duking"))
                        {
                            newGivenPokemon.Add(newPoke);
                        }

                        ushort[] newMoveSet = MoveShuffler.GetNewMoveset(random, settings.MoveSetOptions, giftPoke.Pokemon, extractedGame.GiftPokemonList[i].Level, extractedGame);
                        for (int j = 0; j < newMoveSet.Length; j++)
                        {
                            extractedGame.GiftPokemonList[i].SetMove(j, newMoveSet[j]);
                        }
                    }
                    break;
                case TradeRandomSetting.Requested:
                    for (int i = 0; i < 3; i++)
                    {
                        newGivenPokemon.Add(extractedGame.PokemonList[new XDTradePokemon((byte)(i + 2), iso).Pokemon]);
                    }
                    break;
            }

            // set requested
            for (int i = 0; i < 3; i++)
            {
                var pokeSpot = new PokeSpotPokemon(2, (PokeSpotType)i, iso);
                var newRequestedPoke = settings.UsePokeSpotPokemonInTrade || settings.Trade == TradeRandomSetting.Requested || settings.Trade == TradeRandomSetting.Both
                    ? extractedGame.PokemonList[pokeSpot.Pokemon]
                    : pokemon[random.Next(0, pokemon.Length)];
                newRequestedPokemon.Add(newRequestedPoke);
            }

            Logger.Log($"Requested Pokemon: {string.Join(", ", newRequestedPokemon.Select(p => p.Name))}\n");
            Logger.Log($"Given Pokemon: {string.Join(", ", newGivenPokemon.Select(p => p.Name))}\n");

            XDTradePokemon.UpdateTrades(iso, newRequestedPokemon.ToArray(), newGivenPokemon.ToArray());
        }

        public static void RandomizeColoStatics(AbstractRNG random, StaticPokemonShufflerSettings settings, IGiftPokemon[] starters, ExtractedGame extractedGame)
        {
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
                        var newPoke = pokemon[random.Next(0, pokemon.Length)];
                        giftPoke.Pokemon = (ushort)newPoke.Index;
                        newGivenPokemon.Add(newPoke);

                        ushort[] newMoveSet = MoveShuffler.GetNewMoveset(random, settings.MoveSetOptions, giftPoke.Pokemon, extractedGame.GiftPokemonList[i].Level, extractedGame);
                        for (int j = 0; j < newMoveSet.Length; j++)
                        {
                            extractedGame.GiftPokemonList[i].SetMove(j, newMoveSet[j]);
                        }
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
                        condition = !PokemonTraitShuffler.CheckForSplitOrEndEvolution(newStarter, out int _)
                            && !PokemonTraitShuffler.CheckForSplitOrEndEvolution(extractedGame.PokemonList[secondStage.EvolvesInto], out int _)
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
                        condition = !PokemonTraitShuffler.CheckForSplitOrEndEvolution(newStarter, out int _)
                            && PokemonTraitShuffler.CheckForSplitOrEndEvolution(extractedGame.PokemonList[secondStage.EvolvesInto], out int count)
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
                        condition = PokemonTraitShuffler.CheckForSplitOrEndEvolution(newStarter, out int count)
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
                moves = MoveShuffler.GetNewMoveset(random, settings.MoveSetOptions, starter.Pokemon, starter.Level, extractedGame);
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
