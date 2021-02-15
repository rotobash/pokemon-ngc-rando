using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public interface ITrainerPokemon
    {
        ushort Pokemon { get; set; }
        bool IsShadow { get; }
        byte ShadowCatchRate { get; set; }
        byte Level { get; set; }
        ushort Item { get; set; }
        ushort[] Moves { get; }
        void SetMove(int index, ushort moveNum);
        void SetShadowMove(int index, ushort moveNum);
    }
}
