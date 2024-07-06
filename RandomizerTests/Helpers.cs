using Randomizer.Shufflers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomizerTests
{
    public static class Helpers
    {
        public static int RerunTimes = 100;
        public static Settings CreateBlankSettings()
        {
            return new Settings
            {
                ItemShufflerSettings = new ItemShufflerSettings(),
                BingoCardShufflerSettings = new BingoCardShufflerSettings(),
                MoveShufflerSettings = new MoveShufflerSettings(),
                PokemonTraitShufflerSettings = new PokemonTraitShufflerSettings(),
                PokeSpotShufflerSettings = new PokeSpotShufflerSettings(),
                StaticPokemonShufflerSettings = new StaticPokemonShufflerSettings(),
                TeamShufflerSettings = new TeamShufflerSettings()
            };
        }
    }
}
