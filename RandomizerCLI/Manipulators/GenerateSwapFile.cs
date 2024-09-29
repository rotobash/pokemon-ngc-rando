using RandomizerCLI.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;
using XDCommon.Shufflers;

namespace RandomizerCLI.Manipulators
{
    public class GenerateSwapFile : GameManipulator
    {
        string OutputDirectory { get; }
        IEnumerable<SlotTypes> Slots { get; }

        public GenerateSwapFile(TestSwapOptions options): base(options)
        {
            OutputDirectory = options.OutputPath;
            Slots = options.Slots;
        }

        public void Generate()
        {
            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new UnicodeStringJsonConverter()
                }
            };

            var rooms = GameExtractor.ExtractRooms();
            var pokemon = GameExtractor.ExtractPokemon();
            var moves = GameExtractor.ExtractMoves();
            var items = GameExtractor.ExtractItems().Where(i => i.BagSlot != BagSlots.None).Select(i =>
                new
                {
                    Index = i.OriginalIndex,
                    i.Name,
                    i.Description,
                    BagSlot = i.BagSlot.ToString(),
                    i.InBattleUseID,
                    i.Price,
                    i.CouponPrice,
                });

            XDExtractor xd = GameExtractor as XDExtractor;

            var checkList = new List<ReplaceItemDefinition>();

            foreach (var slot in Slots)
            {
                switch (slot)
                {
                    case SlotTypes.Overworld:
                        var overworldItems = new List<object>();
                        foreach (var i in GameExtractor.ExtractOverworldItems().Where(it => it.Item != 0))
                        {
                            checkList.Add(new ReplaceItemDefinition
                            {
                                Index = i.index,
                                Item = i.Item,
                                Name = items.FirstOrDefault(it => it.Index == i.Item).Name.ToString(),
                                Slot = slot,
                                Quantity = i.Quantity,
                            });
                        }

                        break;
                }
            }
            var rng = new Xoroshiro128StarStar();

            var pickedItems = new HashSet<int>();
            var swapItems = new List<SwapItem>();

            foreach (var item in checkList)
            {
                var toSwapItem = rng.NextElement(checkList.Where(c => !pickedItems.Contains(c.Index)));
                swapItems.Add(new SwapItem
                {
                    ReplaceItemDefinition = item,
                    SwapItemDefinition = new SwapItemDefinition
                    {
                        Quantity = toSwapItem.Quantity,
                        Slot = toSwapItem.Slot,
                        InternalItem = new SwapInternalItem
                        {
                            Item = toSwapItem.Item,
                        }
                    }
                });

                pickedItems.Add(toSwapItem.Index);
            }

            var swapFile = new SwapFile
            {
                Swap = swapItems
            };

            File.WriteAllBytes($"{OutputDirectory}/swap_file.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(swapFile, serializeOptions)));
        }
    }
}
