using DolphinMemoryAccess;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions.XD;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions.XD.SaveData
{
    public class SaveDataLayout
    {
        const byte FirstPokemonOffset = 0x30;
        const ushort InventoryOffset = FirstPCPokemonOffset + (6 * PokemonInstance.SizeOfData);
        const ushort TMInventoryOffset = InventoryOffset + 0x164;

        static byte[] PCByteMarker = new byte[] 
        { 
            0x20, 0x00, 0xFF, 0xA7,
            0xC0, 0x3D, 0x8E, 0x84,
            0x00, 0x00, 0x00, 0x00,
            0xC1, 0xD0, 0xEC, 0x62,
            0x00, 0x00, 0x92, 0xD4,
            0x00, 0x00, 0x00, 0x65,
            0x00, 0x00, 0x00, 0x00,
            0x16, 0xC7, 0x04, 0x00,
            0x00, 0x02, 0x00, 0x8A,
            0x02, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00 
        };

        const byte FirstPCPokemonOffset = 0x18;

        const ushort BoxPokemonSize = 0x170;

        public XDStoryFlags StoryFlag
        {
            get;
        }

        public PokemonInstance[] Party
        {
            get;
        } = new PokemonInstance[6];

        public PokemonInstance[] PC
        {
            get;
        } = new PokemonInstance[30];

        public InventoryItem[] Inventory
        {
            get;
            private set;
        }

        public InventoryItem[] KeyItemInventory
        {
            get;
            private set;
        }

        public InventoryItem[] TMItemInventory
        {
            get;
            private set;
        }

        public InventoryItem[] PCInventory
        {
            get;
            private set;
        }

        public static SaveDataLayout LoadFromMemory(Dolphin dolphin, long saveDataPointer)
        {
            var saveData = new SaveDataLayout();

            var offset = FirstPokemonOffset;
            var party = new PokemonInstance[6];
            for (int i = 0; i < 6; i++) 
            {
                var pokemonData = dolphin.ReadData(saveDataPointer + FirstPokemonOffset + (PokemonInstance.SizeOfData * i), PokemonInstance.SizeOfData);
                saveData.Party[i] = new PokemonInstance(pokemonData);
            }

            var firstItemOffset = FirstPokemonOffset + (PokemonInstance.SizeOfData * 6);
            var itemIventory = ReadItemData(dolphin, saveDataPointer + firstItemOffset);

            var keyItemOffset = firstItemOffset + (itemIventory.Length * 4) + 0x10;
            saveData.KeyItemInventory = ReadItemData(dolphin, saveDataPointer + keyItemOffset);

            var pokeballItemOffset = keyItemOffset + (saveData.KeyItemInventory.Length * 4) + 0x88;
            var pokeballInventory = ReadItemData(dolphin, saveDataPointer + pokeballItemOffset);

            var tmItemOffset = pokeballItemOffset + (pokeballInventory.Length * 4) + 0x10;
            saveData.TMItemInventory = ReadItemData(dolphin, saveDataPointer + tmItemOffset);
            saveData.Inventory = itemIventory.Concat(pokeballInventory).ToArray();

            return saveData;
        }

        private static InventoryItem[] ReadItemData(Dolphin dolphin, long offset)
        {
            var inventory = new List<InventoryItem>();

            var lastItemHit = false;
            while (!lastItemHit)
            {
                var itemIndex = BinaryPrimitives.ReadUInt16BigEndian(dolphin.ReadData(offset, 2));
                var itemCount = BinaryPrimitives.ReadUInt16BigEndian(dolphin.ReadData(offset + 2, 2));

                if (itemIndex == 0)
                {
                    lastItemHit = true;
                    break;
                }

                inventory.Add(new InventoryItem
                {
                    Index = itemIndex,
                    Quantity = itemCount
                });
                offset += 4;
            }

            return inventory.ToArray();
        }
    }
}
