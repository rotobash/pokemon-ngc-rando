﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    // for convienience
    public class Pokeballs : Items
    {
        public Pokeballs(int index, ISO iso) : base(index, iso)
        {
        }
    }

    public class Items
    {
        const byte NumberOfFriendshipEffects = 0x03;
        const byte BagSlotOffset = 0x00;
        const byte ItemCantBeHeldOffset = 0x01; // also can't be thrown away?
        const byte InBattleUseItemIDOffset = 0x04;// really more like a functional id which is compared with items that can be used rather than having passive uses
        const byte ItemPriceOffset = 0x06;
        const byte ItemCouponCostOffset = 0x08;
        const byte ItemBattleHoldItemIDOffset = 0x0B;
        const byte ItemNameIDOffset = 0x10;
        const byte ItemDescriptionIDOffset = 0x14;
        const byte ItemParameterOffset = 0x1B;
        const byte FirstFriendshipEffectOffset = 0x24; // Signed Int

        uint ColFirstItemOffset => iso.Region switch
        {
            Region.US => 0x360CE8,
            Region.Europe => 0x34D428,
            _ => 0x3ADDA0,
        };

        protected int index;
        public ushort OriginalIndex
        {
            get => (ushort)index;
        }
        public virtual int Index
        {
            get
            {
                if (iso.Game == Game.XD)
                    return (index > iso.CommonRel.GetValueAtPointer(Constants.XDNumberOfItems) && index < 0x251) ? index - 150 : index;
                else
                    return (index >= 349 && index < 0x251) ? index - 151 : index;
            }
        }

        public uint StartOffset
        {
            get
            {
                if (iso.Game == Game.XD)
                    return iso.CommonRel.GetPointer(Constants.Items) + (uint)(Index * Constants.SizeOfItemData);
                else
                    return ColFirstItemOffset + (uint)(Index * Constants.SizeOfItemData);
            }
        }

        public int NameId => dataSource.ExtractedFile.GetIntAtOffset(StartOffset + ItemNameIDOffset);
        public int DescriptionId => dataSource.ExtractedFile.GetIntAtOffset(StartOffset + ItemDescriptionIDOffset);
        
        public UnicodeString Name
        {
            get => iso.CommonRelStringTable.GetStringWithId(NameId);
            set => iso.CommonRelStringTable.ReplaceString(NameId, value);
        }
        public UnicodeString Description
        {
            get => pocketMenu.GetStringWithId(DescriptionId);
            set => pocketMenu.ReplaceString(DescriptionId, value);
        }
        

        protected ISO iso;
        protected StringTable pocketMenu;
        BaseExtractedFile dataSource;

        public Items(int index, ISO iso)
        {
            this.index = index;
            this.iso = iso;

            dataSource = iso.Game == Game.XD ? (BaseExtractedFile)iso.CommonRel : iso.DOL;

            KeyValuePair<string, string> itemTableLocation;
            if (iso.Game == Game.XD)
            {
                itemTableLocation = Constants.XDItemTables[iso.Region].First();
            }
            else
            {
                itemTableLocation = Constants.ColItemTables[iso.Region].First();
            }
            pocketMenu = iso.GetFSysFile(itemTableLocation.Key).GetEntryByFileName(itemTableLocation.Value) as StringTable;

        }

        public BagSlots BagSlot
        {
            get => (BagSlots)dataSource.ExtractedFile.GetByteAtOffset(StartOffset + BagSlotOffset);
            set => dataSource.ExtractedFile.WriteByteAtOffset(StartOffset + BagSlotOffset, (byte)value);
        }

        public byte InBattleUseID
        {
            get => dataSource.ExtractedFile.GetByteAtOffset(StartOffset + InBattleUseItemIDOffset);
            set => dataSource.ExtractedFile.WriteByteAtOffset(StartOffset + InBattleUseItemIDOffset, value);
        }
        public ushort Price
        {
            get => dataSource.ExtractedFile.GetUShortAtOffset(StartOffset + ItemPriceOffset);
            set => dataSource.ExtractedFile.WriteBytesAtOffset(StartOffset + ItemPriceOffset, value.GetBytes());
        }
        public ushort CouponPrice
        {
            get => dataSource.ExtractedFile.GetUShortAtOffset(StartOffset + ItemCouponCostOffset);
            set => dataSource.ExtractedFile.WriteBytesAtOffset(StartOffset + ItemCouponCostOffset, value.GetBytes());
        }
        public byte Parameter
        {
            get => dataSource.ExtractedFile.GetByteAtOffset(StartOffset + ItemParameterOffset);
            set => dataSource.ExtractedFile.WriteByteAtOffset(StartOffset + ItemParameterOffset, value);
        }
        public bool CanBeHeld
        {
            get => dataSource.ExtractedFile.GetByteAtOffset(StartOffset + ItemCantBeHeldOffset) == 0;
            set => dataSource.ExtractedFile.WriteByteAtOffset(StartOffset + InBattleUseItemIDOffset, value ? (byte)0 : (byte)1);
        }

        public byte[] FriendshipEffects
        {
            get => dataSource.ExtractedFile.GetBytesAtOffset(StartOffset + FirstFriendshipEffectOffset, NumberOfFriendshipEffects);
            set => dataSource.ExtractedFile.WriteBytesAtOffset(StartOffset + FirstFriendshipEffectOffset, value);
        }
    }
}
