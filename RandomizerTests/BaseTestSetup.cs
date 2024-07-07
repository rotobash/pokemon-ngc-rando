using Randomizer.Shufflers;
using Randomizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace RandomizerTests
{
    public class BaseTestSetup
    {
        protected IGameExtractor gameExtractor { get; set; }
        protected ShuffleSettings shuffleSettings { get; set; } = new ShuffleSettings
        {
            RNG = new Cryptographic()
        };
        protected Pokemon[] pokemon;
        protected Move[] moves;

        [OneTimeSetUp]
        public void Setup()
        {
            TestConfiguration.TestRomPath = "H:\\ISO\\Gamecube\\Pokemon XD - Gale of Darkness.iso";

            var isoExtractor = new ISOExtractor(TestConfiguration.TestRomPath);
            var iso = isoExtractor.ExtractISO();
            if (iso.Game == XDCommon.Contracts.Game.XD)
            {
                gameExtractor = new XDExtractor(iso);
            }
            else
            {
                gameExtractor = new ColoExtractor(iso);
            }
            shuffleSettings.ExtractedGame = new ExtractedGame(gameExtractor);

            pokemon = gameExtractor.ExtractPokemon();
            moves = gameExtractor.ExtractMoves();
        }

        [SetUp]
        public void SettingsSetup()
        {
            shuffleSettings.RandomizerSettings = Helpers.CreateBlankSettings();
        }
    }
}
