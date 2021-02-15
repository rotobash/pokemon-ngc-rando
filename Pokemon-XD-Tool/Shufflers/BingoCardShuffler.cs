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
            var potentialPokes = extractedGame.PokemonList;
            if (settings.ForceStrongPokemon)
            {
                potentialPokes = potentialPokes.Where(p => p.BST >= Configuration.StrongPokemonBST).ToArray();
            }

            var potentialMoves = extractedGame.MoveList.Where(m => m.BasePower > 0);
            if (settings.ForceGoodDamagingMove)
            {
                potentialMoves = potentialMoves.Where(m => m.BasePower >= Configuration.GoodDamagingMovePower);
            }

            if (settings.BanShadowMoves)
            {
                potentialMoves = potentialMoves.Where(m => !m.IsShadowMove);
            }

            foreach (var card in bingoCards)
            {
                for (int i = 0; i <= card.Panels.Length; i++)
                {
                    BattleBingoPokemon battleBingoPokemon;
                    if (i == card.Panels.Length)
                    {
                        // randomize starter
                        battleBingoPokemon = card.StartingPokemon;
                    }
                    else
                    {
                        var panel = card.Panels[i];
                        if (panel.PanelType != BattleBingoPanelType.Pokemon) continue;
                        battleBingoPokemon = panel.BingoPokemon;
                    }

                    var newPoke = potentialPokes[random.Next(0, potentialPokes.Length)];
                    battleBingoPokemon.Pokemon = (ushort)newPoke.Index;

                    // filter again by stab moves
                    var movePool = potentialMoves;
                    if (settings.ForceSTABMove)
                    {
                        var stabMoves = potentialMoves.Where(m => m.Type == newPoke.Type1 || m.Type == newPoke.Type2);
                        // don't use stab moves if there aren't any that match
                        if (stabMoves.Any())
                            movePool = stabMoves;
                    }
                    // pick good moves
                    battleBingoPokemon.Move = (ushort)movePool.ElementAt(random.Next(0, movePool.Count())).MoveIndex;
                }
                // if you shuffle the panels do you have to write back the offset?
            }
        }
    }
}
