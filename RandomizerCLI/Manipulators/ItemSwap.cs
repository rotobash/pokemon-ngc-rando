using RandomizerCLI.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;

namespace RandomizerCLI.Manipulators
{

    public class SwapExternalItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class SwapInternalItem
    {
        public ushort Item { get; set; }
    }


    public class SwapItemDefinition
    {
        public SwapExternalItem ExternalItem { get; set; }
        public SwapInternalItem InternalItem { get; set; }
        public byte Quantity { get; set; }
        public SlotTypes Slot { get; set; }

        public bool IsExternal()
        {
            return ExternalItem != null;
        }
    }

    public class ReplaceItemDefinition
    {
        public int Index { get; set; }
        public ushort Item { get; set; }
        public string Name { get; set; }
        public byte Quantity { get; set; }
        public SlotTypes Slot { get; set; }
    }

    public class SwapItem
    {
        public ReplaceItemDefinition ReplaceItemDefinition { get; set; }
        public SwapItemDefinition SwapItemDefinition { get; set; }
    }

    public class SwapFile
    {
        public IEnumerable<SwapExternalItem> Add { get; set; } = Array.Empty<SwapExternalItem>();
        public IEnumerable<SwapItem> Swap { get; set; } = Array.Empty<SwapItem>();
    }

    public class ItemSwap : GameManipulator
    {

        SwapFile SwapFile { get; }
        public ItemSwap(SwapOptions options) : base(options)
        {
            if (!File.Exists(options.SwapFile))
            {
                throw new ArgumentException("Cannot open swap file with the path provided.");
            }

            var swapFileContents = File.ReadAllText(options.SwapFile);
            SwapFile = JsonSerializer.Deserialize<SwapFile>(swapFileContents);

            if (SwapFile == null)
            {
                throw new ArgumentException("Cannot read swap file.");
            }
        }

        public void Swap()
        {
            var newItemsIndexDictionary = new Dictionary<string, int>();
            if (SwapFile.Add?.Any() == true)
            {
                var numberOfItems = ISO.CommonRel.GetValueAtPointer(Constants.XDNumberOfItems);
                var firstEmptyItemIndex = 350;
                foreach (var addItem in SwapFile.Add)
                {
                    var item = new Items(firstEmptyItemIndex, ISO);
                    item.BagSlot = BagSlots.KeyItems;
                    item.Name = XDCommon.Utility.UnicodeString.FromString(addItem.Name);
                    item.Description = XDCommon.Utility.UnicodeString.FromString(addItem.Description);

                    newItemsIndexDictionary[addItem.Name] = firstEmptyItemIndex;
                    firstEmptyItemIndex++;
                }
            }

            var overWorldItems = GameExtractor.ExtractOverworldItems().Where(i => i.Item != 0).ToDictionary(k => k.index, v => v);

            foreach (var swapItem in SwapFile.Swap) 
            { 
                var replaceItem = swapItem.ReplaceItemDefinition;
                var swapItemDef = swapItem.SwapItemDefinition;

                if (swapItemDef.IsExternal())
                {

                }
                else
                {
                    var item = overWorldItems[replaceItem.Index];
                    item.Item = swapItemDef.InternalItem.Item;
                    item.Quantity = swapItemDef.Quantity;
                }
            }

            SaveChanges();
        }
    }
}
