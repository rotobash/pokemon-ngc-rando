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

        public void RandomizePokemonTraits(PokemonTraitShufflerSettings settings)
        {
            PokemonTraitShuffler.RandomizePokemonTraits(random, pokemon, moves, settings);
        }

        public void RandomizeTrainers(TeamShufflerSettings settings)
        {
            var decks = gameExtractor.ExtractPools(pokemon, moves);
            TeamShuffler.ShuffleTeams(random, settings, decks, pokemon, moves);
        }

        public void RandomizeTMs()
        {

        }

        public void RandomizeStatics(StaticPokemonShufflerSettings settings)
        {
            gameExtractor.RandomizeStatics(settings, random, pokemon, moves);
        }

        public void RandomizeBattleBingo()
        {

        }


    }
}
