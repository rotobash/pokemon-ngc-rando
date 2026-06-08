using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomizerCLI.Options
{
    public abstract class BaseOptions
    {

        [Option('g', "game", Required = true, HelpText = "The path to the game we're swapping.")]
        public string ISOPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "Where to store the output of this command. Either JSON or a patched ISO.")]
        public string OutputPath { get; set; }
    }
}
