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

        public void RandomizeTMs(ItemShufflerSettings settings)
        {
        }
        
        public void RandomizeOverworldItems(ItemShufflerSettings settings)
        {
            ItemShuffler.ShuffleTMs(random, settings, items, moves);
            var overworldItems = gameExtractor.ExtractOverworldItems();
            ItemShuffler.ShuffleOverworldItems(random, settings, overworldItems, items);

            var marts = gameExtractor.ExtractPokemarts();
            ItemShuffler.UpdatePokemarts(settings, marts, items);
        }

        public void RandomizeStatics(StaticPokemonShufflerSettings settings)
        {
            gameExtractor.RandomizeStatics(settings, random, pokemon, moves);
        }

        public void RandomizeBattleBingo()
        {
            if (gameExtractor is XDExtractor xd)
            {
                var bCards = xd.ExtractBattleBingoCards();
                var bs = bCards.SelectMany(c => c.Panels).Where(p => p.BingoPokemon != null && p.BingoPokemon.Ability > 1);
                var t = 0;
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
