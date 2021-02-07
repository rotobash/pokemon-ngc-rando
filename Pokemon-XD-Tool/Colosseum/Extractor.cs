using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace Randomizer.Colosseum
{
    public class ColoExtractor : IGameExtractor
    {
        public Move[] ExtractMoves()
        {
            throw new NotImplementedException();
        }

        public Pokemon[] ExtractPokemon()
        {
            throw new NotImplementedException();
        }

        public TrainerPool[] ExtractPools(Pokemon[] pokemon, Move[] moves)
        {
            throw new NotImplementedException();
        }
    }
}
