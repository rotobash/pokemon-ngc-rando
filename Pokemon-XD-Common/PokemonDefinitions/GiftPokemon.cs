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
        ushort[] Moves { get; }
        string GiftType { get; }
    }
}
