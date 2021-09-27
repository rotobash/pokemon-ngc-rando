using Randomizer.Colosseum;
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
    public enum PRNGChoice { Net, Cryptographic, Xoroshiro128 }
    public class Randomizer : IDisposable
    {
        AbstractRNG random;
        IGameExtractor gameExtractor;
        ExtractedGame extractedGameStructures;
        private bool disposedValue;

        public Randomizer(IGameExtractor extractor, PRNGChoice prng, int seed = -1)
        {
            switch (prng)
            {
                case PRNGChoice.Net:
                    random = new NetRandom(seed);
                    break;
                case PRNGChoice.Xoroshiro128:
                    random = new Xoroshiro128StarStar(seed > 0 ? (ulong)seed : 0);
                    break;
                case PRNGChoice.Cryptographic:
                    random = new Cryptographic();
                    break;
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
                StaticPokemonShuffler.RandomizeXDStatics(random, settings, starter, gameExtractor.ISO, extractedGameStructures);
            }
            else if (gameExtractor is ColoExtractor colo)
            {
                var starters = colo.GetStarters();
                StaticPokemonShuffler.RandomizeColoStatics(random, settings, starters, extractedGameStructures);
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

        public void RandomizePokeSpots(PokeSpotShufflerSettings settings)
        {
            if (gameExtractor is XDExtractor xd)
            {
                var pokespots = xd.ExtractPokeSpotPokemon();
                PokeSpotShuffler.ShufflePokeSpots(random, settings, pokespots, extractedGameStructures);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    extractedGameStructures = null;
                    gameExtractor = null;
                }

                if (random is Cryptographic c)
                {
                    c.Dispose();
                }
                random = null;
                disposedValue = true;
            }
        }

        ~Randomizer()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
