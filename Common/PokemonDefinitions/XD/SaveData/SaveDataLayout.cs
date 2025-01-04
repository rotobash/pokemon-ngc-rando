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
    public abstract class SaveDataLayout
    {
        // offset after save data pointer
        const ushort FirstPokemonOffset = 0x170;
        protected const byte BufferRow = 0x10;
        const ushort MaxInventorySize = InventoryItem.SizeOfData * 75;
        const ushort MaxPokeballInventorySize = InventoryItem.SizeOfData * 12;
        const ushort MaxTMInventorySize = InventoryItem.SizeOfData * 64;
        const ushort MaxBerryInventorySize = InventoryItem.SizeOfData * 44;
        const ushort MaxCologneInventorySize = InventoryItem.SizeOfData * 4;

        protected uint NextSectionOffset
        {
            get;
            private set;
        }


        public XDStoryFlags StoryFlag
        {
            get;
        }

        public PokemonInstance[] Party
        {
            get;
        } = new PokemonInstance[6];

        public InventoryItem[] BattleItems
        {
            get;
            private set;
        }

        public InventoryItem[] Pokeballs
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
        public InventoryItem[] Berries
        {
            get;
            private set;
        }
        public InventoryItem[] CologneCase
        {
            get;
            private set;
        } = new InventoryItem[3];

        public InventoryItem[] MysteryInventory
        {
            get;
            private set;
        }

        public virtual void LoadFromMemory(Dolphin dolphin)
        {
            var saveDataPointer = PointerLocations.GetR13RelativePointer(dolphin, PointerLocations.SaveDataR13Offset) + FirstPokemonOffset;
            if (saveDataPointer <= 0)
            {
                return;
            }

            for (int i = 0; i < Party.Length; i++) 
            {
                var pokemonData = dolphin.ReadData(saveDataPointer + (PokemonInstance.SizeOfData * i), PokemonInstance.SizeOfData);
                Party[i] = new PokemonInstance(pokemonData);
            }

            var firstItemOffset = (PokemonInstance.SizeOfData * Party.Length);
            BattleItems = ReadItemData(dolphin, saveDataPointer + firstItemOffset);

            var keyItemOffset = firstItemOffset + (BattleItems.Length * 4) + BufferRow;
            KeyItemInventory = ReadItemData(dolphin, saveDataPointer + keyItemOffset);

            var pokeballItemOffset = firstItemOffset + MaxInventorySize;
            Pokeballs = ReadItemData(dolphin, saveDataPointer + pokeballItemOffset);

            var tmItemOffset = pokeballItemOffset + MaxPokeballInventorySize + BufferRow;
            TMItemInventory = ReadItemData(dolphin, saveDataPointer + tmItemOffset);

            var berriesOffset = tmItemOffset + MaxTMInventorySize;
            Berries = ReadItemData(dolphin, saveDataPointer + berriesOffset);

            var cologneCaseOffset = berriesOffset + MaxBerryInventorySize;
            if (KeyItemInventory.Any(i => i.Index == 0x200))
            {
                var cologne = ReadItemData(dolphin, saveDataPointer + cologneCaseOffset);
            }

            var mysteryInventory = cologneCaseOffset + MaxCologneInventorySize;
            MysteryInventory = ReadItemData(dolphin, saveDataPointer + mysteryInventory);
            NextSectionOffset = (uint)(saveDataPointer + mysteryInventory + (MysteryInventory.Length * 4) + 0x98);
        }

        private InventoryItem[] ReadItemData(Dolphin dolphin, long offset)
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
