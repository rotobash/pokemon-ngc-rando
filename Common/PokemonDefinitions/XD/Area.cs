using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class Area
    {
        const byte AreaIDOffset = 0x0;
        const byte CharacterIDOffset = 0x2;
        const byte NameIDOffset = 0x1C;
        const byte RoomIDOffset = 0x20;

        int index;
        ISO iso;
        public int StartOffset => (int)(iso.CommonRel.GetPointer(Constants.XDWorldMapLocations) + index * 36);

        public ushort AreaID
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + AreaIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + AreaIDOffset, value.GetBytes());
        }
        public ushort CharacterID
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + CharacterIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + CharacterIDOffset, value.GetBytes());
        }

        public int NameID
        {
            get => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + NameIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + NameIDOffset, value.GetBytes());
        }
        
        public int RoomId
        {
            get => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + RoomIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + RoomIDOffset, value.GetBytes());
        }

        public string Name
        {
            get => iso.DolStringTable.GetStringWithId(NameID).ToString();
        }

        public Area(int index, ISO iso)
        {
            this.index = index;
            this.iso = iso;
        }
    }
}
