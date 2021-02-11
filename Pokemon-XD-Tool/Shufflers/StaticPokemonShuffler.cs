using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace Randomizer.Shufflers
{
    public enum StarterRandomSetting
    {
        Unchanged,
        Custom,
        Random,
        RandomThreeStage,
        RandomTwoStage,
        RandomSingleStage
    }
    public enum TradeRandomSetting
    {
        Unchanged,
        Given,
        Both,
    }

    public struct StaticPokemonShufflerSettings
    {
        public StarterRandomSetting Starter;
        public string Starter1;
        public string Starter2;

        public TradeRandomSetting Trade;

        public bool RandomizeMovesets;
        public bool UseLevelUpMoves;
    }

    public static class StaticPokemonShuffler
    {
        public static void RandomizeXDStatics(Random random, StaticPokemonShufflerSettings settings, XDStarterPokemon starter, IGiftPokemon[] trades, Pokemon[] pokemon, Move[] moves)
        {
            int index = 0;
            Evolution secondStage;
            bool condition = false;
            switch (settings.Starter)
            {
                case StarterRandomSetting.Custom:
                    {
                        index = pokemon.FirstOrDefault(p => p.Name.ToLower() == settings.Starter1)?.Index ?? starter.Pokemon;
                    }
                    break;
                case StarterRandomSetting.Random:
                    {
                        while (index == 0 || TeamShuffler.SpecialPokemon.Contains(index))
                        {
                            index = random.Next(1, pokemon.Length);
                        }
                    }
                    break;
                case StarterRandomSetting.RandomThreeStage:
                    while (!condition)
                    {
                        index = random.Next(1, pokemon.Length);
                        condition = index != 0 || TeamShuffler.SpecialPokemon.Contains(index)
                            && ((secondStage = pokemon[index].Evolutions[0]).EvolutionMethod != EvolutionMethods.None
                            && pokemon[secondStage.EvolvesInto].Evolutions[0].EvolutionMethod != EvolutionMethods.None);
                    }
                    break;
                case StarterRandomSetting.RandomTwoStage:
                    while (!condition)
                    {
                        index = random.Next(1, pokemon.Length);
                        condition = index != 0 || TeamShuffler.SpecialPokemon.Contains(index)
                            && (secondStage = pokemon[index].Evolutions[0]).EvolutionMethod != EvolutionMethods.None
                            && pokemon[secondStage.EvolvesInto].Evolutions[0].EvolutionMethod == EvolutionMethods.None;
                    }
                    break;
                case StarterRandomSetting.RandomSingleStage:
                    while (!condition)
                    {
                        index = random.Next(1, pokemon.Length);
                        condition = index != 0 || TeamShuffler.SpecialPokemon.Contains(index)
                            && pokemon[index].Evolutions[0].EvolutionMethod == EvolutionMethods.None;
                    }
                    break;
                default:
                case StarterRandomSetting.Unchanged:
                    index = starter.Pokemon;
                    break;
            }
            starter.Pokemon = (ushort)index;

            if (settings.RandomizeMovesets)
            {
                if (settings.UseLevelUpMoves)
                {
                    var learnableMoves = pokemon[starter.Pokemon].CurrentLevelMoves(starter.Level).ToArray();
                    // this is really unsafe 
                    starter.Move1 = learnableMoves[0].Move;
                    starter.Move2 = learnableMoves[1].Move;
                    starter.Move3 = learnableMoves[2].Move;
                    starter.Move4 = learnableMoves[3].Move;
                }
                else
                {
                    starter.Move1 = (ushort)random.Next(0, moves.Length);
                    starter.Move2 = (ushort)random.Next(0, moves.Length);
                    starter.Move3 = (ushort)random.Next(0, moves.Length);
                    starter.Move4 = (ushort)random.Next(0, moves.Length);
                }
            }

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

        public static void RandomizeColoStarters(Random random, StaticPokemonShufflerSettings settings, IGiftPokemon starter1, IGiftPokemon starter2, Pokemon[] pokemon)
        {
        }
    }
}
