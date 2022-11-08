using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public class Evolution
    {
        public EvolutionMethods EvolutionMethod { get; }
        public ushort EvolutionCondition { get; }
        public ushort EvolvesInto { get; }

        public Evolution(byte method, ushort condition, ushort evolvesInto)
        {
            EvolutionMethod = (EvolutionMethods)method;
            EvolutionCondition = condition;
            EvolvesInto = evolvesInto;
        }
    }
}
