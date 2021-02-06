using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public class Evolution
    {
        public EvolutionMethods EvolutionMethod { get; }
        public EvolutionConditionType EvolutionCondition { get; }
        public ushort EvolvesInto { get; }

        public Evolution(byte method, ushort condition, ushort evolvesInto)
        {
            EvolutionMethod = (EvolutionMethods)method;
            EvolutionCondition = (EvolutionConditionType)condition;
            EvolvesInto = evolvesInto;
        }
    }
}
