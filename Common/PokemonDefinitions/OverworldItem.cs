using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class OverworldItem
    {
        const byte TreasureModelIDOffset = 0x0;
        const byte TreasureQuantityOffset = 0x1;
        const byte TreasureAngleOffset = 0x2;
        const byte TreasureRoomIDOffset = 0x4;
        const byte TreasureFlagOffset = 0x6;
        const byte TreasureItemIDOffset = 0xE;
        const byte TreasureXCoordOffset = 0x10;
        const byte TreasureYCoordOffset = 0x14;
        const byte TreasureZCoordOffset = 0x18;

        public int index;
        ISO iso;
        public int StartOffset => (int)(iso.CommonRel.GetPointer(iso.Game == Game.XD ? Constants.XDTreasureBoxData : Constants.ColTreasureBoxData) + index * Constants.SizeOfTreasure);

        public byte Quantity
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + TreasureQuantityOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + TreasureQuantityOffset, value);
        }

        public TreasureTypes Model
        {
            get => (TreasureTypes)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + TreasureModelIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + TreasureModelIDOffset, (byte)value);
        }
        public ushort TreasureRoom
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + TreasureRoomIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + TreasureRoomIDOffset, value.GetBytes());
        }
        
        public ushort Item
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + TreasureItemIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + TreasureItemIDOffset, value.GetBytes());
        }

        public OverworldItem(int index, ISO iso)
        {
            this.index = index;
            this.iso = iso;
        }
    }
}
