using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public interface ITrainerPool
    {
        TrainerPoolType TeamType { get; }
        IEnumerable<ITrainer> AllTrainers { get; }
        void SetShadowPokemon(ITrainerPool shadowPool);
    }
}
