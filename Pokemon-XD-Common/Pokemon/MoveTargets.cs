using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Pokemon
{
    public static partial class Pokemon
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
}
