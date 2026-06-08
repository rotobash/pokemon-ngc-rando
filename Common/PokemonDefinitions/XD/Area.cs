using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class Area
    {
        const byte CameraIDOffset = 0x0;
        const byte CharacterIDOffset = 0x2;
        const byte AvailabilityFlagOffset = 0x4;
        const byte NameIDOffset = 0x1C;
        const byte RoomIDOffset = 0x22;
        const byte SizeOfArea = 0x24;

        public int Index;
        ISO iso;
        public int StartOffset => (int)(iso.CommonRel.GetPointer(Constants.XDWorldMapLocations) + Index * SizeOfArea);

        public ushort CameraID
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + CameraIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + CameraIDOffset, value.GetBytes());
        }
        public ushort CharacterID
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + CharacterIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + CharacterIDOffset, value.GetBytes());
        }
        public uint AvailabilityFlag
        {
            get => iso.CommonRel.ExtractedFile.GetUIntAtOffset(StartOffset + AvailabilityFlagOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + AvailabilityFlagOffset, value.GetBytes());
        }

        public int NameID
        {
            get => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + NameIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + NameIDOffset, value.GetBytes());
        }
        
        public ushort RoomId
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + RoomIDOffset + 2);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + RoomIDOffset + 2, value.GetBytes());
        }

        public string Name
        {
            get => iso.DolStringTable.GetStringWithId(NameID).ToString();
        }

        public Area(int index, ISO iso)
        {
            this.Index = index;
            this.iso = iso;
        }
    }
}
