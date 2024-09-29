using CommandLine;
using RandomizerCLI.Manipulators;
using RandomizerCLI.Options;
using System.ComponentModel;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Text.Json.Serialization;
using XDCommon;
using XDCommon.PokemonDefinitions;
using XDCommon.Shufflers;
using XDCommon.Utility;

namespace RandomizerCLI
{
    internal class Program
    {
        static int Main(string[] args)
        {
            try
            {
                return Parser.Default.ParseArguments<RandomizeOptions, ExtractOptions, SwapOptions, TestSwapOptions>(args)
                    .MapResult(
                        (RandomizeOptions randOpts) => Randomize(randOpts),
                        (ExtractOptions exOpts) => ExtractItems(exOpts),
                        (SwapOptions swapOpts) => SwapItems(swapOpts),
                        (TestSwapOptions swapOpts) => GenerateSwapFile(swapOpts),
                        errs => 1
                    );
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
        }


        static int SwapItems(SwapOptions swapOptions)
        {
            using var itemSwap = new ItemSwap(swapOptions);
            itemSwap.Swap();
            return 0;
        }
        static int GenerateSwapFile(TestSwapOptions swapOptions)
        {
            using var generateSwpFile = new GenerateSwapFile(swapOptions);
            generateSwpFile.Generate();
            return 0;
        }

        static int Randomize(RandomizeOptions randomizeOptions)
        {
            var cliRandomizer = new CommandLineRandomizer(randomizeOptions);
            cliRandomizer.Randomize();
            return 0;
        }

        static int ExtractItems(ExtractOptions extractOptions)
        {
            using var jsonExtractor = new JsonExtractor(extractOptions);
            jsonExtractor.Extract();
            return 0;
        }
    }
}
