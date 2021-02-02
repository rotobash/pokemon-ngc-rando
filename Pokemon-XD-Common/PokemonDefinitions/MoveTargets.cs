using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public enum MoveTargets
    {
        SelectedTarget,
        DependsOnMove,
        AllPokemon,
        Random,
        BothFoes,
        User,
        BothFoesAndAlly,
        OpponentField
    }
}
