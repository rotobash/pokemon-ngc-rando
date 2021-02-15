using System;
using System.Collections.Generic;
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

        int index;
        public int Index => (index > iso.CommonRel.GetValueAtPointer(Constants.NumberOfItems) && index < 0x250) ? index - 150 : index;
        public virtual uint StartOffset => iso.CommonRel.GetPointer(Constants.Items) + (uint)(index * Constants.SizeOfItemData);
        public int NameId => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + ItemNameIDOffset);
        public int DescriptionId => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + ItemDescriptionIDOffset);
        
        public string Name => iso.CommonRelStringTable.GetStringWithId(NameId).ToString();
        public string Description => pocketMenu.GetStringWithId(DescriptionId).ToString();
        

        protected ISO iso;
        protected StringTable pocketMenu;
        public Items(int index, ISO iso)
        {
            this.index = index;
            this.iso = iso;
            pocketMenu = iso.GetFSysFile("pocket_menu.fsys").GetEntryByFileName("pocket_menu.msg") as StringTable;
        }

        public BagSlots BagSlot
        {
            get => (BagSlots)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + BagSlotOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + BagSlotOffset, (byte)value);
        }

        public byte InBattleUseID
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + InBattleUseItemIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + InBattleUseItemIDOffset, value);
        }
        public ushort Price
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + ItemPriceOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + ItemPriceOffset, value.GetBytes());
        }
        public ushort CouponPrice
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + ItemCouponCostOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + ItemCouponCostOffset, value.GetBytes());
        }
        public byte Parameter
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + ItemParameterOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + ItemParameterOffset, value);
        }
        public bool CanBeHeld
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + ItemCantBeHeldOffset) == 0;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + InBattleUseItemIDOffset, value ? (byte)0 : (byte)1);
        }

        public byte[] FriendshipEffects
        {
            get => iso.CommonRel.ExtractedFile.GetBytesAtOffset(StartOffset + FirstFriendshipEffectOffset, NumberOfFriendshipEffects);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + FirstFriendshipEffectOffset, value);
        }
    }
}
