using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.PokemonDefinitions;

namespace Randomizer.Shufflers
{
    public static class BingoCardShuffler
    {
        public static void ShuffleCards(Random random, BingoCardShufflerSettings settings, BattleBingoCard[] bingoCards, ExtractedGame extractedGame)
        {
            IEnumerable<Pokemon> potentialPokes = extractedGame.PokemonList;
            if (settings.ForceStrongPokemon)
            {
                potentialPokes = potentialPokes.Where(p => p.BST >= Configuration.StrongPokemonBST);
            }
            var potentialMoves = extractedGame.MoveList.Where(m => m.BasePower > 0);
            if (settings.ForceGoodDamagingMove)
            {
                potentialMoves = potentialMoves.Where(m => m.BasePower >= Configuration.GoodDamagingMovePower);
            }

            foreach (var card in bingoCards)
            {
                // randomize starter
                ShufflePokemonAndMove(random, potentialPokes, potentialMoves, extractedGame.PokemonList, card.StartingPokemon, settings.ForceSTABMove);

                foreach (var panel in card.Panels)
                {
                    if (panel.PanelType == BattleBingoPanelType.Pokemon)
                    {
                        // randomize panel pokemon
                        ShufflePokemonAndMove(random, potentialPokes, potentialMoves, extractedGame.PokemonList, panel.BingoPokemon, settings.ForceSTABMove);
                    }
                }
                // if you shuffle the panels do you have to write back the offset?
            }
        }

        private static void ShufflePokemonAndMove(Random random, IEnumerable<Pokemon> potentialPokemon, IEnumerable<Move> potentialMoves, Pokemon[] pokemonList, BattleBingoPokemon bingoPokemon, bool forceSTABMove)
        {
            var starterPoke = pokemonList[potentialPokemon.ElementAt(random.Next(0, potentialPokemon.Count())).Index];
            bingoPokemon.Pokemon = (ushort)starterPoke.Index;

            // filter again by stab moves
            var movePool = potentialMoves;
            if (forceSTABMove)
            {
                var stabMoves = potentialMoves.Where(m => m.Type == starterPoke.Type1 || m.Type == starterPoke.Type2);
                // don't use stab moves if there aren't any that match
                if (stabMoves.Any())
                    movePool = stabMoves;
            }
            // pick good moves
            bingoPokemon.Move = (ushort)movePool.ElementAt(random.Next(0, movePool.Count())).MoveIndex;
        }
    }
}
