using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public class LevelUpMove
    {
        public byte Level { get; }
        public ushort Move { get; }

        public LevelUpMove(byte level, ushort move)
        {
            Level = level;
            Move = move;
        }
    }
}
