using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomizerCLI.Options
{

    [Verb("extract", HelpText = "Set output to verbose messages.")]
    public class ExtractOptions : BaseOptions
    {
        [Option('s', "slots", Required = true, HelpText = "The item types to extract.")]
        public IEnumerable<string> Items { get; set; }

        public IEnumerable<SlotTypes> Slots
        {
            get => Items.Select(i => Enum.Parse<SlotTypes>(i, true));
        }
    }
}
