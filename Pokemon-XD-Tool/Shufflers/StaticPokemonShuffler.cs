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
        public static void RandomizeXDStatics(Random random, StaticPokemonShufflerSettings settings, XDStarterPokemon starter, ISO iso, ExtractedGame extractedGame)
        {
            int index = 0;
            Evolution secondStage;
            bool condition = false;
            // pick starters
            // basically set a condition based on the setting, keep looping till you meet it
            switch (settings.Starter)
            {
                case StarterRandomSetting.Custom:
                {
                    index = extractedGame.PokemonList.FirstOrDefault(p => p.Name.ToLower() == settings.Starter1.ToLower())?.Index ?? starter.Pokemon;
                }
                break;
                case StarterRandomSetting.Random:
                {
                    while (index == 0 || RandomizerConstants.SpecialPokemon.Contains(index))
                    {
                        index = random.Next(1, extractedGame.PokemonList.Length);
                    }
                }
                break;
                case StarterRandomSetting.RandomThreeStage:
                    while (!condition)
                    {
                        var newStarter = extractedGame.PokemonList[random.Next(0, extractedGame.ValidPokemon.Length)];
                        index = newStarter.Index;
                        secondStage = extractedGame.PokemonList[index].Evolutions[0];
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
                        var newStarter = extractedGame.PokemonList[random.Next(0, extractedGame.ValidPokemon.Length)];
                        index = newStarter.Index;
                        secondStage = extractedGame.PokemonList[index].Evolutions[0];
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
                        var newStarter = extractedGame.PokemonList[random.Next(0, extractedGame.ValidPokemon.Length)];
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

            // instance pokemon have separate movesets than the pool
            // i.e. if you don't update the moveset than your starter will have Eevee's move set
            ushort[] moves;
            if (settings.MoveSetOptions.RandomizeMovesets || settings.Starter != StarterRandomSetting.Unchanged)
            {
                moves = MoveShuffler.GetNewMoveset(random, settings.MoveSetOptions, starter.Pokemon, starter.Level, extractedGame);
                for (int i = 0; i < moves.Length; i++)
                    starter.SetMove(i, moves[i]);
            }

            List<Pokemon> newRequestedPokemon = new List<Pokemon>();
            List<Pokemon> newGivenPokemon = new List<Pokemon>();
            var pokemon = extractedGame.ValidPokemon;
            // set given
            switch (settings.Trade)
            {
                default:
                case TradeRandomSetting.Unchanged:
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
            switch (settings.Trade)
            {
                case TradeRandomSetting.Requested:
                case TradeRandomSetting.Both:
                    for (int i = 0; i < 3; i++)
                    {
                        var newRequestedPoke = settings.UsePokeSpotPokemonInTrade 
                            ? pokemon[random.Next(0, pokemon.Length)]
                            : pokemon[new PokeSpotPokemon(2, (PokeSpotType)index, iso).Pokemon];
                        newRequestedPokemon.Add(newRequestedPoke);
                    }
                    break;
                default:
                    break;
            }

            XDTradePokemon.UpdateTrades(iso, newRequestedPokemon.ToArray(), newGivenPokemon.ToArray());
        }

        public static void RandomizeColoStatics(Random random, StaticPokemonShufflerSettings settings, IGiftPokemon[] starters, ExtractedGame extractedGame)
        {
        }
    }
}
