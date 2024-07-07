using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.Contracts;
using XDCommon.PokemonDefinitions;

namespace Randomizer.Shufflers
{
    public static class TeamShuffler
    {
        public static void ShuffleTeams(ShuffleSettings shuffleSettings)
        {
            var settings = shuffleSettings.RandomizerSettings.TeamShufflerSettings;
            var extractedGame = shuffleSettings.ExtractedGame;
            var random = shuffleSettings.RNG;

            HashSet<int> randomizedPokemon = new HashSet<int>();
            HashSet<int> pickedShadowPokemon = new HashSet<int>();

            var potentialItems = settings.BanBadItems ? extractedGame.GoodHeldItems : extractedGame.ValidHeldItems;
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
                    if (!trainer.IsSet)
                        continue;

                    Logger.Log($"Trainer {trainer.Name}\nWith:\n");
                    foreach (var pokemon in trainer.Pokemon)
                    {
                        if (pokemon.Pokemon == 0)
                            continue;

                        var originalPoke = pokemon.Pokemon;

                        if (settings.RandomizeLegendaryIntoLegendary && ExtractorConstants.Legendaries.Contains(pokemon.Pokemon))
                        {
                            var potentialLegendaries = ExtractorConstants.Legendaries;

                            if (pokemon.IsShadow && settings.NoDuplicateShadows)
                            {
                                // try to pick a non duplicate shadow if that setting is enabled
                                potentialLegendaries = ExtractorConstants.Legendaries.Where(poke => !pickedShadowPokemon.Contains(poke)).ToArray();
                                // in case we've picked them all, just pick another at random
                                if (!potentialLegendaries.Any())
                                    potentialLegendaries = ExtractorConstants.Legendaries;
                            }

                            // pick random legendary
                            var index = random.Next(potentialLegendaries.Length);
                            pokemon.Pokemon = (ushort)potentialLegendaries[index];
                        }
                        else
                        {
                            RandomizePokemon(random, settings, extractedGame, pokemon);

                            // keep randomizing until we've picked a non duplicate, this check won't run unless the setting is enabled
                            // also add a breakout counter in case the potential pool of pokemon is empty

                            if (settings.NoDuplicateShadows && pokemon.IsShadow)
                            {
                                int breakoutCounter = 0;
                                while (pickedShadowPokemon.Contains(pokemon.Pokemon) && breakoutCounter++ < 10)
                                {
                                    RandomizePokemon(random, settings, extractedGame, pokemon);
                                }
                            }
                        }

                        if (settings.NoDuplicateShadows && pokemon.IsShadow)
                        {
                            pickedShadowPokemon.Add(pokemon.Pokemon);
                        }

                        // for some reason in Colosseum, there are duplicate trainers that point to the same pokemon which causes randomization actions to happen multiple times.
                        // Trainer data and Trainer Pokemon data are stored separately and linked by Ids, so its possible for unused trainers and placeholders to have pokemon set
                        // to the same pokemon that in-game trainers use. for example, Willie (first trainer) has two level 24 Zigzagoons that are randomized, the randomizer continues on
                        // and finds a placeholder trainer that uses those same two Zigzagoons then applies randomization again to those pokemon. The trainers are marked as in use and some even have names,
                        // but there is no way to encounter them in-game normally.
                        // The problem is that it will keep boosting their levels until they hit 100, so keep track of that and skip if we've boosted levels already
                        if (!randomizedPokemon.Contains(pokemon.Index))
                        {
                            randomizedPokemon.Add(pokemon.Index);
                            AdjustPokemonLevels(random, settings, pokemon, extractedGame);
                        }

                        Logger.Log($"{extractedGame.PokemonList[pokemon.Pokemon].Name}\n");
                        Logger.Log($"Is a shadow Pokemon: {pokemon.IsShadow}\n");

                        if (settings.RandomizeHeldItems)
                        {
                            var item = potentialItems[random.Next(0, potentialItems.Length)];
                            pokemon.Item = item.OriginalIndex;
                            Logger.Log($"Holding a(n) {item.Name}\n");
                        }

                        if (pokemon.IsShadow)
                        {
                            AdjustCatchRates(settings, pokemon, originalPoke, extractedGame);
                        }

                        RandomizeMoveSet(random, settings, pokemon, extractedGame);
                        Logger.Log($"\n");
                    }
                    Logger.Log($"\n");
                }
                Logger.Log($"\n");
            }
        }

        public static void RandomizePokemon(AbstractRNG random, TeamShufflerSettings settings, ExtractedGame extractedGame, ITrainerPokemon pokemon)
        {
            var pokeFilter = extractedGame.ValidPokemon;
            if (settings.UseSimilarBSTs)
            {
                pokeFilter = Helpers.GetSimilarBsts(pokemon.Pokemon, pokeFilter, extractedGame.PokemonList).ToArray();
            }

            if (settings.RandomizePokemon)
            {
                var pokemonIndex = 0;
                while (pokemonIndex == 0 || (settings.DontUseLegendaries && ExtractorConstants.Legendaries.Contains(pokemonIndex)))
                {
                    var index = random.Next(pokeFilter.Length);
                    pokemonIndex = pokeFilter[index].Index;
                }
                pokemon.Pokemon = (ushort)pokemonIndex;
            }
        }

        public static void RandomizeMoveSet(AbstractRNG random, TeamShufflerSettings settings, ITrainerPokemon pokemon, ExtractedGame extractedGame)
        {
            ushort[] moveSet = null;

            if (settings.MoveSetOptions.MetronomeOnly)
            {
                moveSet = Enumerable.Repeat(ExtractorConstants.MetronomeIndex, Constants.NumberOfPokemonMoves).ToArray();
            }
            else if (settings.MoveSetOptions.RandomizeMovesets || settings.RandomizePokemon)
            {
                moveSet = Helpers.GetNewMoveset(random, settings.MoveSetOptions, pokemon.Pokemon, pokemon.Level, extractedGame);
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

        public static void AdjustPokemonLevels(AbstractRNG random, TeamShufflerSettings settings, ITrainerPokemon pokemon, ExtractedGame extractedGame)
        {
            var forceFullyEvolved = pokemon.IsShadow
                ? pokemon.ShadowLevel >= settings.ForceFullyEvolvedLevel
                : pokemon.Level >= settings.ForceFullyEvolvedLevel;

            if (settings.ForceFullyEvolved && forceFullyEvolved)
            {
                var currPoke = extractedGame.PokemonList[pokemon.Pokemon];
                while (currPoke.Evolutions.Any(e => e.EvolutionMethod != EvolutionMethods.None))
                {
                    if (Helpers.CheckForSplitOrEndEvolution(currPoke, out var count) && count > 0)
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
                    currPoke = extractedGame.PokemonList[pokemon.Pokemon];
                }
            }

            if (settings.BoostTrainerLevel)
            {
                var level = pokemon.Level;
                var levelIncrease = (byte)Math.Round(Math.Clamp(level + level * settings.BoostTrainerLevelPercent, 1, 100), MidpointRounding.AwayFromZero);
                Logger.Log($"Boosting level from {pokemon.Level} to {levelIncrease}\n");
                pokemon.Level = levelIncrease;

                if (pokemon.IsShadow && extractedGame.Game == Game.XD)
                {
                    level = pokemon.ShadowLevel;
                    levelIncrease = (byte)Math.Round(Math.Clamp(level + level * settings.BoostTrainerLevelPercent, 1, 100), MidpointRounding.AwayFromZero);
                    Logger.Log($"Boosting shadow level from {pokemon.ShadowLevel} to {levelIncrease}\n");
                    pokemon.ShadowLevel = levelIncrease;
                }
            }
        }

        public static void AdjustCatchRates(TeamShufflerSettings settings, ITrainerPokemon pokemon, ushort oldPokemon, ExtractedGame extractedGame)
        {
            int newCatchRate;
            switch (settings.CatchRateAdjustment)
            {
                // Use the new pokemons catch rate to be the shadow's catch rate
                case CatchRateAdjustmentType.True:
                    newCatchRate = extractedGame.PokemonList[pokemon.Pokemon].CatchRate;
                    break;
                
                // Use the old shadow catch rate to calculate the adjusted factor
                case CatchRateAdjustmentType.Adjusted:
                {
                    var currentShadowCatchRate = pokemon.ShadowCatchRate;
                    var actualCatchRate = extractedGame.PokemonList[oldPokemon].CatchRate;
                    var newBaseCatchRate = extractedGame.PokemonList[pokemon.Pokemon].CatchRate;

                    newCatchRate = (int)(newBaseCatchRate * (float)(currentShadowCatchRate / actualCatchRate));
                }
                break;
                case CatchRateAdjustmentType.Minimum:
                {
                    newCatchRate = Math.Max(pokemon.ShadowCatchRate, settings.ShadowCatchRateMinimum);
                }
                break;
                default:
                    return;
            }

            var catchRateIncrease = (byte)Math.Clamp(newCatchRate, 0, byte.MaxValue);
            Logger.Log($"Setting catch rate to {catchRateIncrease}\n");
            pokemon.ShadowCatchRate = catchRateIncrease;
        }
    }
}
