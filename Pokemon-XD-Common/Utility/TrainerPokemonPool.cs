using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Utility
{
    public enum PokemonType
    {
        DPKM,
        DDPK
    }
    public class TrainerPokemonPool
    {
        public PokemonType PokeType;

        public TrainerPokemonPool(int index)
        {
            PokeType = PokemonType.DPKM;
        }

        public TrainerPokemonPool(int index, TrainerPool team)
        {
            PokeType = PokemonType.DDPK;
        }
    }
}
