using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public interface ITrainerPokemon
    {
        Pokemon Pokemon { get; }
        bool IsShadow { get; }
        byte ShadowCatchRate { get; set; }
        byte Level { get; set; }
        void SetPokemon(ushort dexNum);
        void SetMove(int index, ushort moveNum);
        void SetShadowMove(int index, ushort moveNum);
    }
}
