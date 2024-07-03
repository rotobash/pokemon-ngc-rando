using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.PokemonDefinitions;

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

        public static ushort[] GetNewMoveset(AbstractRNG random, RandomMoveSetOptions options, ushort pokemon, ushort level, ExtractedGame extractedGame)
        {
            var poke = extractedGame.PokemonList[pokemon];
            var moveSet = new HashSet<ushort>();
            var moveFilter = options.BanShadowMoves ? extractedGame.ValidMoves.Where(m => !m.IsShadowMove) : extractedGame.ValidMoves;
            if (options.BanEarlyDragonRage)
            {
                moveFilter = moveFilter.Where(m => !(m.MoveIndex == ExtractorConstants.DragonRageIndex && level < ExtractorConstants.BanDragonRageUnderLevel));
            }

            var typeFilter = moveFilter;
            if (options.PreferType)
            {
                typeFilter = typeFilter.Where(m => m.Type == poke.Type1 || m.Type == poke.Type2);
                if (!typeFilter.Any())
                    typeFilter = moveFilter;
            }

            var potentialMoves = typeFilter.ToArray();

            if (options.LegalMovesOnly)
            {
                // change our move filter to only include learnable moves for this pokemon
                var legalMoves = new HashSet<ushort>();
                for (int i = 0; i < poke.LearnableTMs.Length; i++)
                {
                    var canLearn = poke.LearnableTMs[i];
                    if (canLearn)
                    {
                        legalMoves.Add(extractedGame.TMs[i].Move);
                    }
                }

                for (int i = 0; i < poke.TutorMoves.Length; i++)
                {
                    var canLearn = poke.TutorMoves[i];
                    if (canLearn)
                    {
                        legalMoves.Add(extractedGame.TutorMoves[i].Move);
                    }
                }

                for (int i = 0; i < poke.LevelUpMoves.Length; i++)
                {
                    var levelUpMove = poke.LevelUpMoves[i];
                    legalMoves.Add(levelUpMove.Move);
                }

                moveFilter = moveFilter.Where(m => legalMoves.Contains((ushort)m.MoveIndex));
            }
            else if (options.UseLevelUpMoves || !options.RandomizeMovesets)
            {
                // not randomizing moves? pick level up moves then
                var levelUpMoves = extractedGame.PokemonList[pokemon].CurrentLevelMoves(level++);

                // this *could* happen so increase the level until there's at least one move for them
                while (!levelUpMoves.Any() && level <= 100)
                    levelUpMoves = extractedGame.PokemonList[pokemon].CurrentLevelMoves(level++);

                // still nothing, add a random move
                if (!levelUpMoves.Any())
                {
                    var newMove = potentialMoves[random.Next(0, potentialMoves.Length)];
                    moveSet.Add((ushort)newMove.MoveIndex);
                }
                else
                {
                    foreach (var levelUpMove in levelUpMoves)
                    {
                        moveSet.Add(levelUpMove.Move);
                    }
                }
            }
            else if (options.MinimumGoodMoves > 0)
            {
                var goodMoves = potentialMoves.Where(m => m.BasePower >= Configuration.GoodDamagingMovePower).ToArray();
                while (moveSet.Count < options.MinimumGoodMoves)
                {
                    var newMove = goodMoves[random.Next(0, goodMoves.Length)];
                    moveSet.Add((ushort)newMove.MoveIndex);
                }
            }

            if (options.RandomizeMovesets || options.ForceFourMoves)
            {
                while (moveSet.Count < XDCommon.PokemonDefinitions.Constants.NumberOfPokemonMoves)
                {
                    Move newMove;
                    if (options.PreferType && random.Next(0, 10) >= 8)
                        // allow 20% chance for move to not be same type
                        newMove = moveFilter.ElementAt(random.Next(0, moveFilter.Count()));
                    else
                        newMove = potentialMoves[random.Next(0, potentialMoves.Length)];

                    moveSet.Add((ushort)newMove.MoveIndex);
                }
            }

            return moveSet.ToArray();
        }
    }
}
