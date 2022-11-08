using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public interface IGiftPokemon
    {
        byte Index { get; }
        ushort Exp { get; }
        ushort[] Moves { get; }
        string GiftType { get; }

        byte Level { get; set; }
        ushort Pokemon { get; set; }
        void SetMove(int i, ushort move);
    }
}
