using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class Room
    {
        const byte AreaIDOffset = 0x0;
        const byte RoomIDOffset = 0x2;
        const byte NameIDOffset = 0x18;
        int index;
        ISO iso;
        public int StartOffset => (int)(iso.CommonRel.GetPointer(iso.Game == Game.XD ? Constants.XDRooms : Constants.ColRooms) + index * 64);

        public byte AreaID
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + AreaIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + AreaIDOffset, value);
        }
        public int NameID
        {
            get => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + NameIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + NameIDOffset, value.GetBytes());
        }

        public ushort RoomId
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + RoomIDOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + RoomIDOffset, value.GetBytes());
        }
        public string Name
        {
            get => iso.CommonRelStringTable.GetStringWithId(NameID).ToString();
        }

        public Room(int index, ISO iso)
        {
            this.index = index;
            this.iso = iso;
        }

        public static Room FromId(int roomId, ISO iso)
        {
            var numOfRooms = iso.CommonRel.GetValueAtPointer(Constants.XDNumberOfRooms);
            for (int i = 0; i < numOfRooms; i++)
            {
                var room = new Room(i, iso);
                if (room.RoomId == roomId)
                {
                    return room;
                }
            }
            return null;
        }
    }
}
