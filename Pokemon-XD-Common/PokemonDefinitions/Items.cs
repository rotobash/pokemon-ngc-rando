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
        int index;
        public int Index => (index > iso.CommonRel.GetValueAtPointer(Constants.NumberOfItems) && index < 0x250) ? index - 150 : index;
        public virtual uint StartOffset => iso.CommonRel.GetPointer(Constants.Items) + (uint)(index * Constants.SizeOfItemData);
        public int NameId => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + Constants.ItemNameIDOffset);
        public int DescriptionId => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + Constants.ItemDescriptionIDOffset);
        
        public string Name => iso.CommonRelStringTable.GetStringWithId(NameId).ToString();
        public string Description => pocketMenu.GetStringWithId(DescriptionId).ToString();
        

        protected ISO iso;
        protected StringTable pocketMenu;
        public Items(int index, ISO iso)
        {
            this.index = index;
            this.iso = iso;
            pocketMenu = iso.GetFSysFile("pocket_menu.fsys").ExtractEntryByFileName("pocket_menu.msg") as StringTable;
        }

        public BagSlots BagSlot
        {
            get => (BagSlots)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.BagSlotOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.BagSlotOffset, (byte)value);
        }

        public byte InBattleUseID
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.InBattleUseItemIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.InBattleUseItemIDOffset, value);
        }
        public ushort Price
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.ItemPriceOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.ItemPriceOffset, value.GetBytes());
        }
        public ushort CouponPrice
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.ItemCouponCostOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.ItemCouponCostOffset, value.GetBytes());
        }
        public byte Parameter
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.ItemParameterOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.ItemParameterOffset, value);
        }
        public bool CanBeHeld
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.ItemCantBeHeldOffset) == 0;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.InBattleUseItemIDOffset, value ? (byte)0 : (byte)1);
        }

        public byte[] FriendshipEffects
        {
            get => iso.CommonRel.ExtractedFile.GetBytesAtOffset(StartOffset + Constants.FirstFriendshipEffectOffset, Constants.NumberOfFriendshipEffects);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstFriendshipEffectOffset, value);
        }
    }
}
