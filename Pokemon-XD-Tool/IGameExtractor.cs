using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace Randomizer
{
    public interface IGameExtractor
    {
        TrainerPool[] ExtractPools(Pokemon[] pokemon);
        Move[] ExtractMoves();
        Pokemon[] ExtractPokemon();
    }
}
