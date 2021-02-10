using Randomizer.Shufflers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;

namespace Randomizer
{
    public interface IGameExtractor
    {
        ITrainerPool[] ExtractPools(Pokemon[] pokemon, Move[] moves);
        Items[] ExtractItems();
        OverworldItem[] ExtractOverworldItems();
        Pokemarts[] ExtractPokemarts();
        Move[] ExtractMoves();
        Pokemon[] ExtractPokemon();
        void RandomizeStatics(StaticPokemonShufflerSettings settings, Random random, Pokemon[] pokemon, Move[] moves);
    }
}
