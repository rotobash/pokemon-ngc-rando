using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.Contracts;
using XDCommon.PokemonDefinitions;

namespace XDCommon.Shufflers
{
    public static class TeamShuffler
    {
        public static int[] ShuffleTeams(ShuffleSettings shuffleSettings)
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

                        if (settings.RandomizePokemon)
                        {

                            IEnumerable<Pokemon> pokeFilter = extractedGame.ValidPokemon;
                            if (settings.RandomizeLegendaryIntoLegendary && ExtractorConstants.Legendaries.Contains(pokemon.Pokemon))
                            {
                                pokeFilter = ExtractorConstants.Legendaries.Select(i => extractedGame.PokemonList[i]);
                            }

                            if (settings.NoDuplicateShadows && pokemon.IsShadow)
                            {
                                pokeFilter = pokeFilter.Where(p => !pickedShadowPokemon.Contains(p.Index));
                            }

                            Helpers.RandomizePokemon(shuffleSettings, pokemon, pokeFilter);
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

                        if (settings.RandomizeHeldItems && pokemon.Item != 0)
                        {
                            var item = potentialItems[random.Next(0, potentialItems.Length)];
                            pokemon.Item = item.OriginalIndex;
                            Logger.Log($"Holding a(n) {item.Name}\n");
                        }

                        if (pokemon.IsShadow)
                        {
                            AdjustCatchRates(settings, pokemon as IShadowPokemon, originalPoke, extractedGame);
                        }

                        Logger.Log($"\n");
                    }
                    Logger.Log($"\n");
                }
                Logger.Log($"\n");
            }

            return pickedShadowPokemon.ToArray();
        }


        private static void AdjustPokemonLevels(AbstractRNG random, TeamShufflerSettings settings, IPokemonInstance pokemon, ExtractedGame extractedGame)
        {
            var shouldforceFullyEvolved = pokemon.Level >= settings.ForceFullyEvolvedLevel;

            if (settings.ForceFullyEvolved && shouldforceFullyEvolved)
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

                if (pokemon is IShadowPokemon shadowPokemon && extractedGame.Game == Game.XD)
                {
                    level = shadowPokemon.ShadowLevel;
                    levelIncrease = (byte)Math.Round(Math.Clamp(level + level * settings.BoostTrainerLevelPercent, 1, 100), MidpointRounding.AwayFromZero);
                    Logger.Log($"Boosting shadow level from {shadowPokemon.ShadowLevel} to {levelIncrease}\n");
                    shadowPokemon.ShadowLevel = levelIncrease;
                }
            }
        }

        private static void AdjustCatchRates(TeamShufflerSettings settings, IShadowPokemon pokemon, ushort oldPokemon, ExtractedGame extractedGame)
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
