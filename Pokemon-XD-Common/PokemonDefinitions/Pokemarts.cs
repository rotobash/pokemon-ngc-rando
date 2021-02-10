using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class Pokemarts
    {
        int index;
        REL pocket;

        public int FirstItemIndex => (int)pocket.GetPointer(Constants.MartStartIndexes) + (index * 4) + 2;
        public int StartOffset => (int)pocket.GetPointer(Constants.MartItems) + (FirstItemIndex * 2);
        public List<ushort> Items
        {
            get;
        }

        public Pokemarts(int index, ISO iso)
        {
            this.index = index;
            pocket = iso.GetFSysFile("pocket_menu.fsys").ExtractEntryByFileName("pocket_menu.rel") as REL;

            Items = new List<ushort>();
            ushort nextItem;
            var nextItemOffset = StartOffset;
            while ((nextItem = pocket.ExtractedFile.GetUShortAtOffset(nextItemOffset)) != 0) 
            {
                Items.Add(nextItem);
                nextItemOffset += 2;
            }
        }

        public void SaveItems()
        {
            var nextItemOffset = StartOffset;
            foreach (var item in Items)
            {
                pocket.ExtractedFile.WriteBytesAtOffset(nextItemOffset, item.GetBytes());
                nextItemOffset += 2;
            }
            pocket.ExtractedFile.WriteByte(0);
            pocket.ExtractedFile.WriteByte(0);
        }
    }
}
