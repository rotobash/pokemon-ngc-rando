using Randomizer.Shufflers;
using Randomizer.XD;
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
        Items[] items;
        TutorMove[] tutorMoves;

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
            items = gameExtractor.ExtractItems();
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

        public void RandomizeItems(ItemShufflerSettings settings)
        {
            ItemShuffler.ShuffleTMs(random, settings, items, moves);

            var overworldItems = gameExtractor.ExtractOverworldItems();
            ItemShuffler.ShuffleOverworldItems(random, settings, overworldItems, items);

            var marts = gameExtractor.ExtractPokemarts();
            ItemShuffler.UpdatePokemarts(settings, marts, items);

            if (gameExtractor is XDExtractor xd)
            {
                var tutorMoves = xd.ExtractTutorMoves();
                ItemShuffler.ShuffleTutorMoves(random, settings, tutorMoves, moves);
            }
        }

        public void RandomizeStatics(StaticPokemonShufflerSettings settings)
        {
            gameExtractor.RandomizeStatics(settings, random, pokemon, moves);
        }

        public void RandomizeBattleBingo(BingoCardShufflerSettings settings)
        {
            if (gameExtractor is XDExtractor xd)
            {
                var bCards = xd.ExtractBattleBingoCards();
                var bingoShuffler = new BingoCardShuffler(random, bCards, pokemon, moves);
                bingoShuffler.ShuffleCards(settings);
            }
        }

        public void RandomizePokeSpots()
        {
            if (gameExtractor is XDExtractor xd)
            {
                var pokespots = xd.ExtractPokeSpotPokemon();
                var t = 0;
            }
        }


    }
}
