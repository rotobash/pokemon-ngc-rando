using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.PokemonDefinitions;

namespace Randomizer.Shufflers
{
    public struct BingoCardShufflerSettings
    {
        public bool ForceStrongPokemon;
        public bool ForceGoodDamagingMove;
        public bool ForceSTABMove;
        public bool AllowTrolls;
    }

    public class BingoCardShuffler
    {
        BattleBingoCard[] cards;
        Random random;
        Move[] moves;
        Pokemon[] pokemon;
        public BingoCardShuffler(Random random, BattleBingoCard[] bingoCards, Pokemon[] pokemon, Move[] moves)
        {
            this.random = random;
            cards = bingoCards;
            this.pokemon = pokemon;
            this.moves = moves;
        }

        public void ShuffleCards(BingoCardShufflerSettings settings)
        {
            foreach (var card in cards)
            {
                foreach (var panel in card.Panels)
                {
                    if (panel.PanelType == BattleBingoPanelType.Pokemon)
                    {
                        IEnumerable<Pokemon> potentialPokes = pokemon;
                        if (settings.ForceStrongPokemon)
                        {
                            potentialPokes = potentialPokes.Where(p => p.BST >= Configuration.StrongPokemonBST);
                        }
                        var newPoke = potentialPokes.ElementAt(random.Next(0, potentialPokes.Count()));
                        panel.BingoPokemon.Pokemon = (ushort)newPoke.Index;
                        // write type to card?

                        // pick good moves
                        var potentialMoves = moves.Where(m => m.BasePower > 0);
                        if (settings.ForceGoodDamagingMove)
                        {
                            potentialMoves = potentialMoves.Where(m => m.BasePower >= Configuration.GoodDamagingMovePower);
                        }

                        // filter again by stab moves
                        if (settings.ForceSTABMove)
                        {
                            var stabMoves = potentialMoves.Where(m => m.Type == newPoke.Type1 || m.Type == newPoke.Type2);
                            // don't use stab moves if there aren't any that match
                            if (stabMoves.Any())
                                potentialMoves = stabMoves;
                        }
                        panel.BingoPokemon.Move = (ushort)potentialMoves.ElementAt(random.Next(0, potentialMoves.Count())).MoveIndex;
                    }
                    // if you shuffle the panels do you have to write back the offset?
                }
            }
        }
    }
}
