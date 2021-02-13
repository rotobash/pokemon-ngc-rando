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
        public static void RandomizeXDStatics(Random random, StaticPokemonShufflerSettings settings, XDStarterPokemon starter, IGiftPokemon[] trades, ExtractedGame extractedGame)
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
            if (settings.RandomizeMovesets && !settings.UseLevelUpMoves)
            {
                moves = MoveShuffler.GetRandomMoveset(random, settings.BanShadowMoves, settings.MovePreferType, settings.ForceGoodDamagingMovesCount, starter.Pokemon, extractedGame);
            }
            else
            {
                moves = MoveShuffler.GetLevelUpMoveset(random, starter.Pokemon, starter.Level, settings.ForceFourMoves, settings.BanShadowMoves, extractedGame);
            }

            for (int i = 0; i < moves.Length; i++)
                starter.SetMove(i, moves[i]);

            switch (settings.Trade)
            {
                case TradeRandomSetting.Given:
                case TradeRandomSetting.Both:
                    for (int i = 0; i < trades.Length; i++)
                    {
                        // do stuff
                    }
                    break;
                default:
                case TradeRandomSetting.Unchanged:
                    break;
            }
        }

        public static void RandomizeColoStarters(Random random, StaticPokemonShufflerSettings settings, IGiftPokemon[] starters, Pokemon[] pokemon)
        {
        }
    }
}
