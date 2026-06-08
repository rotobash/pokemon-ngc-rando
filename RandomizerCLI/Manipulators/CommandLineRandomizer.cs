using RandomizerCLI.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;
using XDCommon.Shufflers;
using XDCommon;
using XDCommon.Utility;
using System.IO;

namespace RandomizerCLI.Manipulators
{
    internal class CommandLineRandomizer : GameManipulator
    {
        protected Settings Settings { get; }
        protected Randomizer Randomizer { get; }

        public CommandLineRandomizer(RandomizeOptions randomizeOptions) : base(randomizeOptions)
        { 
            if (!File.Exists(randomizeOptions.SettingsPath))
            {
                throw new ArgumentException("Cannot open settings file with the path provided.");
            }


            var settingsFileContents = File.ReadAllText(randomizeOptions.SettingsPath);
            Settings = JsonSerializer.Deserialize<Settings>(settingsFileContents);

            if (Settings == null)
            {
                throw new ArgumentException("Settings file could not be read properly. Please fix the file and try again.");
            }

            Randomizer = new Randomizer(GameExtractor, Settings, randomizeOptions.RNGChoice, randomizeOptions.Seed);
            Logger.CreateNewLogFile(randomizeOptions.OutputPath, Settings);
        }

        public void Randomize()
        {

            Console.WriteLine("Randomizing Moves...");
            Randomizer.RandomizeMoves();

            Console.WriteLine("Randomizing Items...");
            Randomizer.RandomizeItems();

            Console.WriteLine("Randomizing Pokemon Traits...");
            Randomizer.RandomizePokemonTraits();

            Console.WriteLine("Randomizing Trainers...");
            var pickedshadows = Randomizer.RandomizeTrainers();

            Console.WriteLine("Randomizing PokeSpots...");
            Randomizer.RandomizePokeSpots();

            Console.WriteLine("Randomizing Statics...");
            Randomizer.RandomizeStatics(pickedshadows);

            // will only do something if game is XD
            Console.WriteLine("Randomizing Bingo...");
            Randomizer.RandomizeBattleBingo();

            Logger.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Randomizer.Dispose();
        }
    }
}
