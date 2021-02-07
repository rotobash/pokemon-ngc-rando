using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public interface IGiftPokemon
    {
        byte Index { get; }

        byte Level { get; }
        ushort Exp { get; }
        ushort Pokemon { get; }
        ushort Move1 { get; }
        ushort Move2 { get; }
        ushort Move3 { get; }
        ushort Move4 { get; }
        string GiftType { get; }
        bool UseLevelUpMoves { get; }
    }
}
