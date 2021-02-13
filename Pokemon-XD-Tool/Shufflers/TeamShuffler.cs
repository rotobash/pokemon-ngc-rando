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
                        if (pokemon.Pokemon.Index == 0)
                            continue;

                        RandomizePokemon(random, settings, pokemon, extractedGame.PokemonList);

                        if (settings.RandomizeHeldItems)
                        {
                            pokemon.Item = (ushort)potentialItems[random.Next(0, potentialItems.Length)].Index;
                        }

                        if (settings.SetMinimumShadowCatchRate)
                        {
                            SetMinimumCatchRate(settings.ShadowCatchRateMinimum, pokemon);
                        }
                        if (settings.BoostTrainerLevel)
                        {
                            BoostLevel(settings.BoostTrainerLevelPercent, pokemon);
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
                pokemon.SetPokemon((ushort)index);
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
        }

        public static void RandomizeMoveSet(Random random, TeamShufflerSettings settings, ITrainerPokemon pokemon, ExtractedGame extractedGame)
        {
            var moveSet = new HashSet<ushort>();
            if (settings.RandomizeMovesets)
            {
                if (settings.ForceGoodDamagingMoves)
                {
                    var goodDamagingMoves = extractedGame.GoodDamagingMoves;
                    // find all moves that meet our criteria and sample from there
                    while (moveSet.Count < settings.ForceGoodDamagingMovesCount)
                    {
                        var potentialMove = goodDamagingMoves[(ushort)random.Next(0, goodDamagingMoves.Length)];
                        moveSet.Add((ushort)potentialMove.MoveIndex);
                    }
                }

                // fill the rest of the move set
                while (moveSet.Count < Constants.NumberOfPokemonMoves)
                    moveSet.Add((ushort)random.Next(1, extractedGame.MoveList.Length));

                for (int i = 0; i < Constants.NumberOfPokemonMoves; i++)
                {
                    pokemon.SetMove(i, moveSet.ElementAt(i));
                }
            }
            else if (settings.MetronomeOnly)
            {
                for (int i = 0; i < Constants.NumberOfPokemonMoves; i++)
                    pokemon.SetMove(i, RandomizerConstants.MetronomeIndex);
            }
            else
            {
                var learnableMoves = pokemon.Pokemon.CurrentLevelMoves(pokemon.Level).Select(m => extractedGame.MoveList[m.Move]).ToArray();

                // well fuck
                if (learnableMoves.Length == 0)
                {
                    // pick one at random I guess?
                    pokemon.SetMove(0, (ushort)random.Next(1, extractedGame.MoveList.Length));
                    return;
                }

                for (int i = 0; i < Constants.NumberOfPokemonMoves; i++)
                {
                    // the pokemon is too low level/doesn't learn enough moves
                    // if forcing 4 moves, randomly pick some to fill the gaps
                    if (i > learnableMoves.Length - 1 && settings.ForceFourMoves)
                        pokemon.SetMove(i, (ushort)random.Next(1, extractedGame.MoveList.Length));
                    else if (i < learnableMoves.Length)
                        pokemon.SetMove(i, (ushort)learnableMoves.ElementAt(i).MoveIndex);
                }
            }
        }

        public static void SetMinimumCatchRate(float minimumCatchRate, ITrainerPokemon poke)
        {
            if (poke.ShadowCatchRate == 0)
            {
                poke.ShadowCatchRate = poke.Pokemon.CatchRate;
            }

            var catchRate = Math.Min(poke.ShadowCatchRate, minimumCatchRate);
            var catchRateIncrease = (byte)Math.Clamp(catchRate, 0, byte.MaxValue);
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
