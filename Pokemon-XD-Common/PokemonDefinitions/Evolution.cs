using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public class Evolution
    {
        public EvolutionMethods EvolutionMethod { get; }
        public EvolutionConditionType EvolutionCondition { get; }
        public int EvolvesInto { get; }

        public Evolution(int method, int condition, int evolvesInto)
        {
            EvolutionMethod = (EvolutionMethods)method;
            EvolutionCondition = (EvolutionConditionType)condition;
            EvolvesInto = evolvesInto;
        }
    }
}
