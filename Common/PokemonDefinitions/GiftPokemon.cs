using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public interface IGiftPokemon : IPokemonInstance
    {
        ushort Exp { get; }
        string GiftType { get; }
    }
}
