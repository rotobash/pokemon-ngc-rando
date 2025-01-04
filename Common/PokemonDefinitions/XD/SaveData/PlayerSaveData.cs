using DolphinMemoryAccess;
using Reloaded.Memory.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions.XD.SaveData
{
    public class PlayerSaveData : SaveDataLayout
    {
        // offset after inventory
        static byte PCBoxOffset = 0x14;
        const ushort PCPokemonBoxSize = 0x170C;
        const ushort PCBoxFooterSize = 0x400;

        const ushort SaveDataSize = 0x978;

        public PCBox[] PC
        {
            get;
        } = new PCBox[8];

        public InventoryItem[] PCInventory
        {
            get;
            private set;
        }

        public override void LoadFromMemory(Dolphin dolphin)
        {
            base.LoadFromMemory(dolphin);
            var pcBoxPtr = NextSectionOffset + NextSectionOffset.GetAlignBytesCount(16) + BufferRow;

            for (int i = 0; i < PC.Length; i++)
            {
                var boxData = dolphin.ReadData(pcBoxPtr + (PCPokemonBoxSize  * i), PCPokemonBoxSize);
                PC[i] = new PCBox(boxData);
            }
        }
    }
}
