using CommandLine;

namespace RandomizerCLI
{
    internal class Program
    {
        public enum ItemTypes { Overworld }

        [Verb("extract", HelpText = "Set output to verbose messages.")]
        public class ExtractOptions
        {
            [Option('o', "output", Required = true, HelpText = "Where to extract the JSON files too.")]
            public string OutputPath { get; set; }

            [Option('i', "items", Required = true, HelpText = "The item types to extract.")]
            public IEnumerable<string> Items { get; set; }

            public IEnumerable<ItemTypes> ItemTypes
            { 
                get => Items.Select(i => Enum.Parse<ItemTypes>(i));
            }
        }

        [Verb("randomize", HelpText = "Set output to verbose messages.")]
        public class RandomizeOptions
        {

            [Option('g', "game", Required = true, HelpText = "The path to the game we're swapping.")]
            public string ISOPath { get; set; }
            [Option('s', "settings", Required = false, HelpText = "Set output to verbose messages.")]
            public Random Settings { get; set; }

        }

        [Verb("swap", HelpText = "Set output to verbose messages.")]
        public class SwapOptions
        {
            [Option('s', "swapfile", Required = true, HelpText = "The JSON file that defines which items to swap.")]
            public string SwapFile { get; set; }

            [Option('g', "game", Required = true, HelpText = "The path to the game we're swapping.")]
            public string ISOPath { get; set; }
        }

        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<RandomizeOptions, ExtractOptions, SwapOptions>(args)
                .MapResult(
                    (RandomizeOptions randOpts) => Randomize(randOpts),
                    (ExtractOptions exOpts) => ExtractItems(exOpts),
                    (SwapOptions swapOpts) => SwapItems(swapOpts),
                    errs => 1
                );
        }


        static int SwapItems(SwapOptions swapOptions)
        {
            return 0;
        }

        static int Randomize(RandomizeOptions randomizeOptions)
        {
            return 0;
        }

        static int ExtractItems(ExtractOptions extractOptions)
        {
            return 0;
        }
    }
}
