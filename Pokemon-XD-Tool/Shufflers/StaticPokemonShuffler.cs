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
        public static void RandomizeXDStatics(Random random, StaticPokemonShufflerSettings settings, XDStarterPokemon starter, ExtractedGame extractedGame)
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
                        index = random.Next(1, extractedGame.PokemonList.Length);
                        condition = (RandomizerConstants.SpecialPokemon.Contains(index))
                            && (secondStage = extractedGame.PokemonList[index].Evolutions[0]).EvolutionMethod != EvolutionMethods.None
                            && extractedGame.PokemonList[secondStage.EvolvesInto].Evolutions[0].EvolutionMethod != EvolutionMethods.None
                            && !PokemonTraitShuffler.CheckForSplitOrEndEvolution(extractedGame.PokemonList[index], out int _);
                    }
                    break;
                case StarterRandomSetting.RandomTwoStage:
                    while (!condition)
                    {
                        index = random.Next(1, extractedGame.PokemonList.Length);
                        condition = (RandomizerConstants.SpecialPokemon.Contains(index))
                            && (secondStage = extractedGame.PokemonList[index].Evolutions[0]).EvolutionMethod != EvolutionMethods.None
                            && extractedGame.PokemonList[secondStage.EvolvesInto].Evolutions[0].EvolutionMethod == EvolutionMethods.None;
                    }
                    break;
                case StarterRandomSetting.RandomSingleStage:
                    while (!condition)
                    {
                        index = random.Next(1, extractedGame.PokemonList.Length);
                        condition = (RandomizerConstants.SpecialPokemon.Contains(index))
                            && extractedGame.PokemonList[index].Evolutions[0].EvolutionMethod == EvolutionMethods.None;
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

            switch (settings.Trade)
            {
                case TradeRandomSetting.Given:
                case TradeRandomSetting.Both:
                    var pokemon = extractedGame.ValidPokemon;
                    for (int i = 0; i < extractedGame.GiftPokemonList.Length; i++)
                    {
                        var newPoke = (ushort)pokemon[random.Next(0, pokemon.Length)].Index;
                        extractedGame.GiftPokemonList[i].Pokemon = newPoke;

                        ushort[] newMoveSet = MoveShuffler.GetNewMoveset(random, settings.MoveSetOptions, newPoke, extractedGame.GiftPokemonList[i].Level, extractedGame);
                        for (int j = 0; j < newMoveSet.Length; j++)
                        {
                            extractedGame.GiftPokemonList[i].SetMove(j, newMoveSet[j]);
                        }
                    }
                    break;
                default:
                case TradeRandomSetting.Unchanged:
                    break;
            }
        }

        public static void RandomizeColoStatics(Random random, StaticPokemonShufflerSettings settings, IGiftPokemon[] starters, ExtractedGame extractedGame)
        {
        }
    }
}
