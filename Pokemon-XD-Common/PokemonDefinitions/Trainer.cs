using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public interface ITrainer
    {
        string Name { get; }
        ITrainerPokemon[] Pokemon { get; }
        bool IsSet { get; }
    }
}
