using Randomizer.Shufflers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace Randomizer
{
    public class Randomizer
    {
        Random random;
        IGameExtractor gameExtractor;
        Move[] moves;
        Pokemon[] pokemon;

        public Randomizer(IGameExtractor extractor, int seed = -1)
        {
            if (seed < 0)
            {
                random = new Random();
            }
            else
            {
                random = new Random(seed);
            }
            gameExtractor = extractor;
            moves = gameExtractor.ExtractMoves();
            pokemon = gameExtractor.ExtractPokemon();
        }

        public void RandomizeMoves(MoveShufflerSettings settings)
        {
            MoveShuffler.RandomizeMoves(random, moves, settings);
        }

        public void RandomizePokemon(PokemonTraitShufflerSettings settings)
        {
            PokemonTraitShuffler.RandomizePokemonTraits(random, pokemon, settings);
        }

        public void RandomizeTrainers(TeamShufflerSettings settings)
        {
            var decks = gameExtractor.ExtractPools(pokemon);
            TeamShuffler.ShuffleTeams(random, settings, decks, pokemon);
        }

        public void RandomizeTMs()
        {

        }

        public void RandomizeTraits()
        {

        }

        public void RandomizeBattleBingo()
        {

        }


    }
}
