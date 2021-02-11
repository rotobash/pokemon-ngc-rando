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
        public bool ForceFourMoves;
    }

    public static class StaticPokemonShuffler
    {
        public static void RandomizeXDStatics(Random random, StaticPokemonShufflerSettings settings, XDStarterPokemon starter, IGiftPokemon[] trades, Pokemon[] pokemon, Move[] moves)
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
                    index = pokemon.FirstOrDefault(p => p.Name.ToLower() == settings.Starter1.ToLower())?.Index ?? starter.Pokemon;
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
                        condition = (index != 0 || TeamShuffler.SpecialPokemon.Contains(index))
                            && (secondStage = pokemon[index].Evolutions[0]).EvolutionMethod != EvolutionMethods.None
                            && pokemon[secondStage.EvolvesInto].Evolutions[0].EvolutionMethod != EvolutionMethods.None
                            && !PokemonTraitShuffler.CheckForSplitOrEndEvolution(pokemon[index], out int _);
                    }
                    break;
                case StarterRandomSetting.RandomTwoStage:
                    while (!condition)
                    {
                        index = random.Next(1, pokemon.Length);
                        condition = (index != 0 || TeamShuffler.SpecialPokemon.Contains(index))
                            && (secondStage = pokemon[index].Evolutions[0]).EvolutionMethod != EvolutionMethods.None
                            && pokemon[secondStage.EvolvesInto].Evolutions[0].EvolutionMethod == EvolutionMethods.None;
                    }
                    break;
                case StarterRandomSetting.RandomSingleStage:
                    while (!condition)
                    {
                        index = random.Next(1, pokemon.Length);
                        condition = (index != 0 || TeamShuffler.SpecialPokemon.Contains(index))
                            && pokemon[index].Evolutions[0].EvolutionMethod == EvolutionMethods.None;
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
            var moveSet = new HashSet<ushort>();
            if (settings.RandomizeMovesets)
            {
                while (moveSet.Count < Constants.NumberOfPokemonMoves)
                    moveSet.Add((ushort)random.Next(0, moves.Length));

                for (int i = 0; i < Constants.NumberOfPokemonMoves; i++)
                {
                    starter.SetMove(i, moveSet.ElementAt(i));
                }
            }
            else
            {
                // not randomizing moves? pick level up moves then
                foreach (var levelUpMove in pokemon[starter.Pokemon].CurrentLevelMoves(starter.Level))
                {
                    moveSet.Add(levelUpMove.Move);
                }

                if (settings.ForceFourMoves && moveSet.Count < Constants.NumberOfPokemonMoves)
                {
                    var total = moveSet.Count;
                    while (moveSet.Count < Constants.NumberOfPokemonMoves)
                        moveSet.Add((ushort)random.Next(0, moves.Length));
                }

                for (int i = 0; i < moveSet.Count; i++)
                {
                    starter.SetMove(i, moveSet.ElementAt(i));
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
