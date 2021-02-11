using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;

namespace Randomizer.Shufflers
{
    public struct TeamShufflerSettings
    {
        public bool RandomizePokemon;
        public bool AllowSpecialPokemon;
        public bool DontUseLegendaries;

        public bool BoostShadowCatchRate;
        public float BoostShadowCatchRatePercent;
        public bool BoostTrainerLevel;
        public float BoostTrainerLevelPercent;
        public bool ForceFullyEvolved;
        public int ForceFullyEvolvedLevel;

        public bool RandomizeHeldItems;
        public bool BanBadItems;
        public bool RandomizeMovesets;
        public bool MetronomeOnly;
        public bool UseLevelUpMoves;
        public bool ForceFourMoves;
    }

    public static class TeamShuffler
    {
        // invalid pokemon
        public static readonly List<int> SpecialPokemon = new List<int>
        {
            252,
            253,
            254,
            255,
            256,
            257,
            258,
            259,
            260,
            261,
            262,
            263,
            264,
            265,
            266,
            267,
            268,
            269,
            270,
            271,
            272,
            273,
            274,
            275,
            276,
            412
        };

        public static readonly List<int> Legendaries = new List<int>
        {
        };

        public static readonly List<int> BadItemList = new List<int>
        {
        };


        public static void ShuffleTeams(Random random, TeamShufflerSettings settings, ITrainerPool[] trainerPools, Pokemon[] pokemonList, Move[] moves)
        {
            // yikes
            foreach (var pool in trainerPools)
            {
                if (pool.TeamType == TrainerPoolType.DarkPokemon)
                    continue;

                foreach (var trainer in pool.AllTrainers)
                {
                    if (!trainer.IsSet)
                        continue;

                    foreach (var pokemon in trainer.Pokemon)
                    {
                        if (pokemon.Pokemon.Index == 0)
                            continue;

                        if (settings.RandomizePokemon)
                        {
                            var index = 0;
                            while (index == 0 || (!settings.AllowSpecialPokemon && SpecialPokemon.Contains(index)) || (settings.DontUseLegendaries && Legendaries.Contains(index)))
                            {
                                index = random.Next(1, pokemonList.Length);
                            }
                            pokemon.SetPokemon((ushort)index);
                        }

                        if (settings.BoostShadowCatchRate)
                        {
                            BoostCatchRate(settings.BoostShadowCatchRatePercent, pokemon);
                        }

                        if (settings.BoostTrainerLevel)
                        {
                            BoostLevel(settings.BoostTrainerLevelPercent, pokemon);
                        }

                        if (settings.RandomizeHeldItems)
                        {
                            var newItemInd = (ushort)random.Next(0, Constants.TotalNumberOfItems);
                            if (settings.BanBadItems)
                            {
                                while (BadItemList.Contains(newItemInd))
                                {
                                    newItemInd = (ushort)random.Next(0, Constants.TotalNumberOfItems);
                                }
                            }
                            pokemon.Item = newItemInd;
                        }

                        if (settings.ForceFullyEvolved && pokemon.Level >= settings.ForceFullyEvolvedLevel)
                        {
                            if (PokemonTraitShuffler.CheckForSplitOrEndEvolution(pokemon.Pokemon, out var count) && count > 0)
                            {
                                // randomly pick from the split
                                var evoInd = random.Next(0, count);
                                pokemon.SetPokemon(pokemon.Pokemon.Evolutions[evoInd].EvolvesInto);
                            } 
                            else if (count == 1)
                            {
                                // it wasn't split or the end but still evolved
                                pokemon.SetPokemon(pokemon.Pokemon.Evolutions[0].EvolvesInto);
                            }
                        }

                        if (settings.RandomizeMovesets)
                        {
                            if (settings.MetronomeOnly)
                            {
                                var metronomeMove = moves.Single(m => m.Name.ToLower() == "metronome");
                                for (int i = 0; i < Constants.NumberOfPokemonMoves; i++)
                                    pokemon.SetMove(i, (ushort)metronomeMove.MoveIndex);
                            }
                            else if (settings.UseLevelUpMoves)
                            {
                                var learnableMoves = pokemon.Pokemon.CurrentLevelMoves(pokemon.Level).ToArray();
                                for (int i = 0; i < Constants.NumberOfPokemonMoves; i++)
                                {
                                    // the pokemon is too low level/doesn't learn enough moves
                                    // if forcing 4 moves, randomly pick some to fill the gaps
                                    if (i > learnableMoves.Length - 1 && settings.ForceFourMoves)
                                        pokemon.SetMove(i, (ushort)random.Next(1, moves.Length));
                                    else if (i < learnableMoves.Length)
                                        pokemon.SetMove(i, learnableMoves.ElementAt(i).Move);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < Constants.NumberOfPokemonMoves; i++)
                                {
                                    pokemon.SetMove(i, (ushort)random.Next(0, moves.Length));
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void BoostCatchRate(float boostPercent, ITrainerPokemon poke)
        {
            if (poke.ShadowCatchRate == 0)
            {
                poke.ShadowCatchRate = poke.Pokemon.CatchRate;
            }

            var catchRate = poke.ShadowCatchRate;
            var catchRateIncrease = (byte)Math.Clamp(catchRate + catchRate * boostPercent, 0, byte.MaxValue);
            poke.ShadowCatchRate = catchRateIncrease;
        }

        public static void BoostLevel(float boostPercent, ITrainerPokemon poke)
        {
            var level = poke.Level;
            var levelIncrease = (byte)Math.Clamp(level + level * boostPercent, 1, 100);
            poke.Level = levelIncrease;
        }
    }
}
