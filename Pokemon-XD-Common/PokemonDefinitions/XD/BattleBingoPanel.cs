using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public enum BattleBingoPanelType
    {
        Mystery,
        Pokemon
    }

    public enum BattleBingoItemType
    {
        None,
        Masterball,
        EP1,
        EP2,
    }

    public class BattleBingoPanel
    {
        public BattleBingoPanelType PanelType { get; }
        public BattleBingoPokemon BingoPokemon { get; }
        public BattleBingoItemType BingoItem { get; }

        public BattleBingoPanel(BattleBingoPokemon bingoPokemon)
        {
            PanelType = BattleBingoPanelType.Pokemon;
            BingoPokemon = bingoPokemon;
        }

        public BattleBingoPanel(BattleBingoItemType bingoItem)
        {
            PanelType = BattleBingoPanelType.Mystery;
            BingoItem = bingoItem;
        }
    }
}
