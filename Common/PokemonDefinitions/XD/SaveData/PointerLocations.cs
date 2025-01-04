using DolphinMemoryAccess;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions.XD.SaveData
{
    public static class PointerLocations
    {
        public const int SaveDataR13Offset = -0x4728;
        public const int SaveDataR13NameOffset = -0x45E8;
        public const uint PlayerBattleDataPtr = 0x804A1730;

        public const uint R13Address = 0x804efe20;

        public static uint GetR13RelativePointer(Dolphin process, int offset)
        {
            var data = process.ReadData(R13Address + offset, 4);
            if (data.Length >= 4)
            {
                return BinaryPrimitives.ReadUInt32BigEndian(data);
            }
            return 0;
        }
    }
}
