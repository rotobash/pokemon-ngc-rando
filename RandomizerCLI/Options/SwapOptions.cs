using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomizerCLI.Options
{

    [Verb("swap", HelpText = "Set output to verbose messages.")]
    public class SwapOptions : BaseOptions
    {
        [Option('s', "swapfile", Required = true, HelpText = "The JSON file that defines which items to swap.")]
        public string SwapFile { get; set; }
    }
}
