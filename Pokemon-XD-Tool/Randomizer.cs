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
        ExtractedGame extractedGameStructures;

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

            extractedGameStructures = new ExtractedGame(extractor);
            gameExtractor = extractor;
        }

        public void RandomizeMoves(MoveShufflerSettings settings)
        {
            MoveShuffler.RandomizeMoves(random, settings, extractedGameStructures);
        }

        public void RandomizePokemonTraits(PokemonTraitShufflerSettings settings)
        {
            PokemonTraitShuffler.RandomizePokemonTraits(random, settings, extractedGameStructures);
        }

        public void RandomizeTrainers(TeamShufflerSettings settings)
        {
            TeamShuffler.ShuffleTeams(random, settings, extractedGameStructures);
        }

        public void RandomizeItems(ItemShufflerSettings settings)
        {
            ItemShuffler.ShuffleTMs(random, settings, extractedGameStructures);
            ItemShuffler.ShuffleOverworldItems(random, settings, extractedGameStructures);
            ItemShuffler.UpdatePokemarts(random, settings, extractedGameStructures);

            if (gameExtractor is XDExtractor xd)
            {
                var tutorMoves = xd.ExtractTutorMoves();
                ItemShuffler.ShuffleTutorMoves(random, settings, tutorMoves, extractedGameStructures);
            }
        }

        public void RandomizeStatics(StaticPokemonShufflerSettings settings)
        {
            if (gameExtractor is XDExtractor xd)
            {
                var starter = xd.GetStarter();
                StaticPokemonShuffler.RandomizeXDStatics(random, settings, starter, Array.Empty<IGiftPokemon>(), extractedGameStructures);
            }
            else
            {
                StaticPokemonShuffler.RandomizeColoStarters(random, settings, null, extractedGameStructures.PokemonList);
            }
        }

        public void RandomizeBattleBingo(BingoCardShufflerSettings settings)
        {
            if (gameExtractor is XDExtractor xd)
            {
                var bCards = xd.ExtractBattleBingoCards();
                BingoCardShuffler.ShuffleCards(random, settings, bCards, extractedGameStructures);
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
