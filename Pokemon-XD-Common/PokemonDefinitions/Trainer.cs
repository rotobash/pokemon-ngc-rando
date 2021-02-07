using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public interface ITrainer
    {
        ITrainerPokemon[] Pokemon { get; }
        bool IsSet { get; }
    }
}
