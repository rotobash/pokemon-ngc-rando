using Randomizer.Shufflers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.Utility;

namespace Randomizer
{
    public class Randomizer
    {
        Random random;
        IGameExtractor gameExtractor;

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
        }

        public void RandomizeMoves(MoveShufflerSettings settings)
        {
            var moves = gameExtractor.ExtractMoves();
            MoveShuffler.RandomizeMoves(random, moves, settings);
        }

        public void RandomizePokemon(PokemonTraitShufflerSettings settings)
        {
            var pokemon = gameExtractor.ExtractPokemon();
            PokemonTraitShuffler.RandomizePokemonTraits(random, pokemon, settings);
        }

        public void RandomizeTrainers()
        {
            var decks = gameExtractor.ExtractPools();
            var t = 0;

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
