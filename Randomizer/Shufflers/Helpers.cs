using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.Contracts;
using XDCommon.PokemonDefinitions;
using static System.Windows.Forms.Design.AxImporter;

namespace Randomizer.Shufflers
{
    internal static class Helpers
    {
        public static IEnumerable<Pokemon> GetSimilarBsts(int initialPokeIndex, IEnumerable<Pokemon> currentPokeFilter, Pokemon[] pokemonList)
        {
            var count = 1;
            var pokemonDefinition = pokemonList[initialPokeIndex];
            IEnumerable<Pokemon> similarStrengths = Array.Empty<Pokemon>();
            while (!similarStrengths.Any())
            {
                // anybody? hello?
                var bstRangeSize = count * Configuration.BSTRange;
                similarStrengths = currentPokeFilter.Where(p => p.BST >= pokemonDefinition.BST - bstRangeSize && p.BST <= pokemonDefinition.BST + bstRangeSize);
                count++;
            }
            return similarStrengths;
        }

        public static bool CheckForSplitOrEndEvolution(Pokemon currentPoke, out int count)
        {
            bool endOrSplitEvolution = false;
            count = 0;
            for (int i = 0; i < currentPoke.Evolutions.Length; i++)
            {
                // if more than one definition found or the first evolution is none
                if (i == 0 && currentPoke.Evolutions[i].EvolutionMethod == EvolutionMethods.None
                    || currentPoke.Evolutions[i].EvolutionMethod != EvolutionMethods.None && i > 0)
                    endOrSplitEvolution = true;

                // keep count for split evos
                if (currentPoke.Evolutions[i].EvolutionMethod != EvolutionMethods.None)
                    count++;
            }

            return endOrSplitEvolution;
        }

        public static ushort[] GetLevelUpMoveset(AbstractRNG random, RandomMoveSetOptions options, IPokemonInstance pokemon, ushort level, ExtractedGame extractedGame)
        {
            var moveSet = new HashSet<ushort>();
            var levelUpMoves = extractedGame.PokemonList[pokemon.Pokemon].CurrentLevelMoves(level);
            var moveSetCount = options.ForceFourMoves ? 4 : pokemon.Moves.Where(m => m > 0).Count();
            if (moveSetCount == 0) 
                moveSetCount = 1;

            // increase the level until there's enough moves for them
            while (levelUpMoves.Count() < moveSetCount && level <= 100)
                levelUpMoves = extractedGame.PokemonList[pokemon.Pokemon].CurrentLevelMoves(++level);

            // still nothing, add a random move
            if (!levelUpMoves.Any())
            {
                var newMove = random.NextElement(extractedGame.ValidMoves);
                moveSet.Add((ushort)newMove.MoveIndex);
            }
            else
            {
                foreach (var levelUpMove in levelUpMoves)
                {
                    moveSet.Add(levelUpMove.Move);
                }
            }
            return moveSet.ToArray();
        }

        public static Pokemon RandomizePokemon(ShuffleSettings shuffleSettings, IPokemonInstance pokemon = null, IEnumerable<Pokemon> pokeFilter = null)
        {
            AbstractRNG random = shuffleSettings.RNG;
            TeamShufflerSettings settings = shuffleSettings.RandomizerSettings.TeamShufflerSettings;
            ExtractedGame extractedGame = shuffleSettings.ExtractedGame;
            if (pokeFilter == null || pokeFilter.Count() == 0)
                pokeFilter = extractedGame.ValidPokemon;

            if (settings.UseSimilarBSTs)
            {
                pokeFilter = GetSimilarBsts(pokemon.Pokemon, pokeFilter, extractedGame.PokemonList).ToArray();
            }

            Pokemon newPoke = null;
            while (newPoke == null || (settings.DontUseLegendaries && ExtractorConstants.Legendaries.Contains(newPoke.Index)))
            {
                newPoke = random.NextElement(pokeFilter);
            }

            if (pokemon != null)
            {
                pokemon.Pokemon = (ushort)newPoke.Index;
                Logger.Log($"{newPoke.Name}\n");

                RandomizeMoveSet(random, settings, pokemon, extractedGame);
            }

            return newPoke;
        }

        public static void RandomizeMoveSet(AbstractRNG random, TeamShufflerSettings settings, IPokemonInstance pokemon, ExtractedGame extractedGame)
        {
            ushort[] moveSet = null;

            if (settings.MoveSetOptions.MetronomeOnly)
            {
                moveSet = new ushort[] { ExtractorConstants.MetronomeIndex, 0, 0, 0 };
            }
            else if (settings.MoveSetOptions.RandomizeMovesets || settings.RandomizePokemon)
            {
                moveSet = GetNewMoveset(random, settings.MoveSetOptions, pokemon, pokemon.Level, extractedGame);
            }

            if (moveSet != null)
            {
                Logger.Log($"It knows:\n");
                for (int i = 0; i < Constants.NumberOfPokemonMoves; i++)
                {
                    ushort move = i < moveSet.Length ? moveSet[i] : (ushort)0;
                    if (move > 0) Logger.Log($"{extractedGame.MoveList[move].Name}\n");
                    pokemon.SetMove(i, move);
                }
                Logger.Log($"\n\n");
            }
        }

        public static ushort[] GetNewMoveset(AbstractRNG random, RandomMoveSetOptions options, IPokemonInstance pokemon, ushort level, ExtractedGame extractedGame)
        {
            if (options.UseLevelUpMoves)
            {
                // not randomizing moves? pick level up moves then
                return GetLevelUpMoveset(random, options, pokemon, level, extractedGame);
            }
            else if (options.RandomizeMovesets)
            {
                return GetRandomMoveset(random, options, pokemon, level, extractedGame);
            }
            else if (options.LegalMovesOnly)
            {
                // change our move filter to only include learnable moves for this pokemon
                var legalMoves = extractedGame.PokemonLegalMovePool[pokemon.Pokemon];
                var moveSetCount = options.ForceFourMoves ? 4 : pokemon.Moves.Where(m => m > 0).Count();

                if (moveSetCount == 0)
                    moveSetCount = 1;

                var moveSet = new ushort[moveSetCount];
                for (int i = 0; i < moveSetCount; i++)
                {
                    moveSet[i] = random.NextElement(legalMoves);
                }
                return moveSet;
            }
            else
            {
                return null;
            }

        }

        private static ushort[] GetRandomMoveset(AbstractRNG random, RandomMoveSetOptions options, IPokemonInstance pokemon, ushort level, ExtractedGame extractedGame)
        {
            if (options.RandomizeMovesets)
            {
                var poke = extractedGame.PokemonList[pokemon.Pokemon];
                var moveSet = new HashSet<ushort>();
                var moveFilter = options.BanShadowMoves ? extractedGame.ValidMoves.Where(m => !m.IsShadowMove) : extractedGame.ValidMoves;
                var moveSetCount = options.ForceFourMoves ? 4 : pokemon.Moves.Where(m => m > 0).Count();

                if (moveSetCount == 0)
                    moveSetCount = 1;

                if (options.BanEarlyDragonRage && level < ExtractorConstants.BanDragonRageUnderLevel)
                {
                    moveFilter = moveFilter.Where(m => m.MoveIndex != ExtractorConstants.DragonRageIndex);
                }

                var potentialMoves = moveFilter.ToArray();
                while (moveSet.Count < moveSetCount)
                {
                    Move newMove;
                    if (options.PreferType && random.Next(0, 10) >= 8)
                        // allow 20% chance for move to not be same type
                        newMove = random.NextElement(moveFilter);
                    else
                        newMove = random.NextElement(potentialMoves);

                    moveSet.Add((ushort)newMove.MoveIndex);
                }

                return moveSet.ToArray();
            }

            return null;
        }
    }
}
