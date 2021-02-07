using Randomizer.Shufflers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;

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

        public ITrainerPool[] ExtractPools(Pokemon[] pokemon, Move[] moves)
        {
            throw new NotImplementedException();
        }

        public void RandomizeStatics(StaticPokemonShufflerSettings settings, Random random, Pokemon[] pokemon, Move[] moves)
        {
            throw new NotImplementedException();
        }
    }
}
