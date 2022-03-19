using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class Pokemarts
    {
        public int Index { get; }
        REL pocket;
        int originalItemsCount;

        public int StartOffset => (int)pocket.GetPointer(Constants.MartItems) + (FirstItemIndex * 2);

        public ushort FirstItemIndex
        {
            get => pocket.ExtractedFile.GetUShortAtOffset(pocket.GetPointer(Constants.MartStartIndexes) + (Index * 4) + 2);
            set => pocket.ExtractedFile.WriteBytesAtOffset(pocket.GetPointer(Constants.MartStartIndexes) + (Index * 4) + 2, value.GetBytes());
        }
        public List<ushort> Items
        {
            get;
            private set;
        }

        public Pokemarts(int index, REL pocket)
        {
            Index = index;
            this.pocket = pocket;
            LoadItems();
        }

        private void LoadItems()
        {
            Items = new List<ushort>();
            ushort nextItem;
            var nextItemOffset = StartOffset;

            while ((nextItem = pocket.ExtractedFile.GetUShortAtOffset(nextItemOffset)) != 0)
            {
                Items.Add(nextItem);
                nextItemOffset += 2;
            }

            originalItemsCount = Items.Count;
        }

        public void SaveItems()
        {
            var newItemsCount = (Items.Count - originalItemsCount);

            var numberOfItemsOffset = pocket.GetPointer(Constants.NumberOfMartItems);
            var numberOfItems = pocket.ExtractedFile.GetIntAtOffset(numberOfItemsOffset);
            pocket.ExtractedFile.WriteBytesAtOffset(numberOfItemsOffset, (numberOfItems + newItemsCount).GetBytes());

            // update pointers
            int offset = 0;
            for (int i = 0; i < pocket.NumberOfPointers; i++)
            {
                offset = (int)pocket.GetPointer(i);

                if (offset > StartOffset)
                    pocket.ReplacePointer(i, offset + (newItemsCount * 2));
            }

            var itemOffset = StartOffset;
            var itemBytes = Items.SelectMany(i => i.GetBytes()).ToArray();

            pocket.AdjustPointerStart(newItemsCount * 2);
            pocket.ExtractedFile = pocket.ExtractedFile.DeleteFromStream(itemOffset, originalItemsCount * 2);
            pocket.ExtractedFile = pocket.ExtractedFile.InsertIntoStream(itemOffset, itemBytes);
            pocket.ExtractedFile.Flush();

            originalItemsCount = Items.Count;
        }
    }
}
