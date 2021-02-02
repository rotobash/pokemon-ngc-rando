using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Utility
{
    public enum PokemonFileType
    {
        DPKM,
        DDPK
    }
    public class TrainerPokemonPool
    {
        public PokemonFileType PokeType;

        public TrainerPokemonPool(int index)
        {
            PokeType = PokemonFileType.DPKM;
        }

        public TrainerPokemonPool(int index, TrainerPool team)
        {
            PokeType = PokemonFileType.DDPK;
        }
    }
}
