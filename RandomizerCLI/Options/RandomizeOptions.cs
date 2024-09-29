using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;

namespace RandomizerCLI.Options
{

    [Verb("randomize", HelpText = "Set output to verbose messages.")]
    public class RandomizeOptions : BaseOptions
    {
        [Option('s', "settings", Required = true, HelpText = "The path to the randomization settings we're using.")]
        public string SettingsPath { get; set; }

        [Option('r', "random", Default = PRNGChoice.Xoroshiro128, Required = true, HelpText = "The random number generator to use.")]
        public PRNGChoice RNGChoice { get; set; }

        [Option('d', "seed", Default = -1, Required = false, HelpText = "The seed for the RNG to use, default is -1 for a random seed. Not applicable to Cryptographic.")]
        public int Seed { get; set; }

    }
}
