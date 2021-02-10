using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class OverworldItem
    {
        int index;
        ISO iso;
        public int StartOffset => (int)(iso.CommonRel.GetPointer(iso.Game == Game.XD ? Constants.XDTreasureBoxData : Constants.ColTreasureBoxData) + index * Constants.SizeOfTreasure);

        public byte Quantity
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.TreasureQuantityOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.TreasureQuantityOffset, value);
        }

        public byte Model
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.TreasureModelIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.TreasureModelIDOffset, value);
        }
        public ushort Item
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.TreasureItemIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.TreasureItemIDOffset, value.GetBytes());
        }

        public OverworldItem(int index, ISO iso)
        {
            this.index = index;
            this.iso = iso;
        }
    }
}
