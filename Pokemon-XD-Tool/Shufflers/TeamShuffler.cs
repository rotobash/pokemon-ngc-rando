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
            if (settings.MoveSetOptions.RandomizeMovesets && settings.MoveSetOptions.ForceGoodMoves)
            {
                potentialMoves = potentialMoves.Where(m => m.BasePower >= Configuration.GoodDamagingMovePower).ToArray();
            }
            Logger.Log("=============================== Trainers ===============================\n\n");

            // yikes
            foreach (var pool in extractedGame.TrainerPools)
            {
                if (pool.TeamType == TrainerPoolType.DarkPokemon)
                    continue;

                foreach (var trainer in pool.AllTrainers)
                {
                    if (!trainer.IsSet || trainer.Name == string.Empty)
                        continue;

                    Logger.Log($"Trainer {trainer.Name}\nWith:\n");
                    foreach (var pokemon in trainer.Pokemon)
                    {
                        if (pokemon.Pokemon == 0)
                            continue;

                        RandomizePokemon(random, settings, pokemon, extractedGame.PokemonList);
                        Logger.Log($"{extractedGame.PokemonList[pokemon.Pokemon].Name}\n");
                        Logger.Log($"Is a shadow Pokemon: {pokemon.IsShadow}\n");

                        if (settings.RandomizeHeldItems)
                        {
                            var item = potentialItems[random.Next(0, potentialItems.Length)];
                            pokemon.Item = (ushort)item.Index;
                            Logger.Log($"Holding a(n) {item.Name}\n");
                        }

                        if (settings.SetMinimumShadowCatchRate && pokemon.IsShadow)
                        {
                            if (pokemon.ShadowCatchRate == 0)
                            {
                                pokemon.ShadowCatchRate = extractedGame.PokemonList[pokemon.Pokemon].CatchRate;
                            }
                            var catchRate = Math.Max(pokemon.ShadowCatchRate, settings.ShadowCatchRateMinimum);
                            var catchRateIncrease = (byte)Math.Clamp(catchRate, 0, byte.MaxValue);

                            Logger.Log($"Setting catch rate to {catchRateIncrease}\n");
                            pokemon.ShadowCatchRate = catchRateIncrease;
                        }
                        if (settings.BoostTrainerLevel)
                        {
                            var level = pokemon.Level;
                            var levelIncrease = (byte)Math.Clamp(level + level * settings.BoostTrainerLevelPercent, 1, 100);
                            Logger.Log($"Boosting level from {pokemon.Level} to {levelIncrease}\n");
                            pokemon.Level = levelIncrease;
                        }

                        RandomizeMoveSet(random, settings, pokemon, extractedGame);
                        Logger.Log($"\n");
                    }
                    Logger.Log($"\n");
                }
                Logger.Log($"\n");
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
            ushort[] moveSet = null;

            if (settings.MoveSetOptions.MetronomeOnly)
            {
                moveSet = Enumerable.Repeat(RandomizerConstants.MetronomeIndex, Constants.NumberOfPokemonMoves).ToArray();
            }
            else if (settings.MoveSetOptions.RandomizeMovesets || settings.RandomizePokemon)
            {
                moveSet = MoveShuffler.GetNewMoveset(random, settings.MoveSetOptions, pokemon.Pokemon, pokemon.Level, extractedGame);
            }

            if (moveSet != null)
            {
                Logger.Log($"It knows:\n");
                for (int i = 0; i < moveSet.Length; i++)
                {
                    var move = moveSet[i];
                    Logger.Log($"{extractedGame.MoveList[move].Name}\n");
                    pokemon.SetMove(i, move);
                }
            }
        }
    }
}
