using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.PokemonDefinitions;

namespace Randomizer.Shufflers
{
    public static class TeamShuffler
    {
        public static void ShuffleTeams(Random random, TeamShufflerSettings settings, ExtractedGame extractedGame)
        {
            var potentialItems = settings.BanBadItems ? extractedGame.GoodItems : extractedGame.NonKeyItems;
            var potentialMoves = extractedGame.MoveList;
            if (settings.RandomizeMovesets && settings.ForceGoodDamagingMoves)
            {
                potentialMoves = potentialMoves.Where(m => m.BasePower >= Configuration.GoodDamagingMovePower).ToArray();
            }
            
            // yikes
            foreach (var pool in extractedGame.TrainerPools)
            {
                if (pool.TeamType == TrainerPoolType.DarkPokemon)
                    continue;

                foreach (var trainer in pool.AllTrainers)
                {
                    if (!trainer.IsSet)
                        continue;

                    foreach (var pokemon in trainer.Pokemon)
                    {
                        if (pokemon.Pokemon == 0)
                            continue;

                        RandomizePokemon(random, settings, pokemon, extractedGame.PokemonList);

                        if (settings.RandomizeHeldItems)
                        {
                            pokemon.Item = (ushort)potentialItems[random.Next(0, potentialItems.Length)].Index;
                        }

                        if (settings.SetMinimumShadowCatchRate)
                        {
                            if (pokemon.ShadowCatchRate == 0)
                            {
                                pokemon.ShadowCatchRate = extractedGame.PokemonList[pokemon.Pokemon].CatchRate;
                            }

                            var catchRate = Math.Max(pokemon.ShadowCatchRate, settings.ShadowCatchRateMinimum);
                            var catchRateIncrease = (byte)Math.Clamp(catchRate, 0, byte.MaxValue);
                            pokemon.ShadowCatchRate = catchRateIncrease;
                        }
                        if (settings.BoostTrainerLevel)
                        {
                            var level = pokemon.Level;
                            var levelIncrease = (byte)Math.Clamp(level + level * settings.BoostTrainerLevelPercent, 1, 100);
                            pokemon.Level = levelIncrease;
                        }

                        RandomizeMoveSet(random, settings, pokemon, extractedGame);
                    }
                }
            }
        }

        public static void RandomizePokemon(Random random, TeamShufflerSettings settings, ITrainerPokemon pokemon, Pokemon[] pokemonList)
        {
            if (settings.RandomizePokemon)
            {
                var index = 0;
                while (index == 0 || RandomizerConstants.SpecialPokemon.Contains(index) || (settings.DontUseLegendaries && RandomizerConstants.Legendaries.Contains(index)))
                {
                    index = random.Next(1, pokemonList.Length);
                }
                pokemon.Pokemon = (ushort)index;
            }

            if (settings.ForceFullyEvolved && pokemon.Level >= settings.ForceFullyEvolvedLevel)
            {
                var currPoke = pokemonList[pokemon.Pokemon];
                if (PokemonTraitShuffler.CheckForSplitOrEndEvolution(currPoke, out var count) && count > 0)
                {
                    // randomly pick from the split
                    var evoInd = random.Next(0, count);
                    pokemon.Pokemon = currPoke.Evolutions[evoInd].EvolvesInto;
                }
                else if (count == 1)
                {
                    // it wasn't split or the end but still evolved
                    pokemon.Pokemon = currPoke.Evolutions[0].EvolvesInto;
                }
            }
        }

        public static void RandomizeMoveSet(Random random, TeamShufflerSettings settings, ITrainerPokemon pokemon, ExtractedGame extractedGame)
        {
            ushort[] moveSet;
            if (settings.RandomizeMovesets && !settings.UseLevelUpMoves)
            {
                moveSet = MoveShuffler.GetRandomMoveset(random, settings.BanShadowMoves, settings.MovePreferType, settings.ForceGoodDamagingMovesCount, pokemon.Pokemon, extractedGame);
            }
            else if (settings.MetronomeOnly)
            {
                for (int i = 0; i < Constants.NumberOfPokemonMoves; i++)
                    pokemon.SetMove(i, RandomizerConstants.MetronomeIndex);
                return;
            }
            else
            {
                moveSet = MoveShuffler.GetLevelUpMoveset(random, pokemon.Pokemon, pokemon.Level, settings.ForceFourMoves, settings.BanShadowMoves, extractedGame);
            }

            for (int i = 0; i < Constants.NumberOfPokemonMoves; i++)
            {
                pokemon.SetMove(i, moveSet.ElementAt(i));
            }
        }
    }
}
