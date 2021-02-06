using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public class LevelUpMove
    {
        public int Level { get; }
        public int Move { get; }

        public LevelUpMove(int level, int move)
        {
            Level = level;
            Move = move;
        }
    }
}
