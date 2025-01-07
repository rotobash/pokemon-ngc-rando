using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class BattleField
    {
        const byte sizeOfBattleFieldData = 0x18;
        const byte battleFieldRoomTypeOffset = 0x00;
        const byte battleFieldRoomIDOffset = 0x02;

        int index;
        ISO iso;

        uint StartOffset => (uint)(iso.CommonRel.GetPointer(Constants.BattleFields) + (index * sizeOfBattleFieldData));

        public ushort RoomId 
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battleFieldRoomIDOffset); 
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + battleFieldRoomIDOffset, value.GetBytes());
        }

        public ushort RoomType
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battleFieldRoomTypeOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + battleFieldRoomTypeOffset, value.GetBytes());
        }

        public BattleField(int index, ISO iso)
        {
            this.index = index;
            this.iso = iso;
        }

        public static BattleField FromRoomId(int roomId, ISO iso)
        {
            for (int i = 0; i < iso.CommonRel.GetValueAtPointer(Constants.NumberOfBattleFields); i++)
            {
                var field = new BattleField(i, iso);
                if (field.RoomId == roomId)
                {
                    return field;
                }
            }
            return null;
        }
    }
}
