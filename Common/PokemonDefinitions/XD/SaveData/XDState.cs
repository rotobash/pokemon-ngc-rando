using DolphinMemoryAccess;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions.XD.SaveData
{
    public enum DolphinState
    {
        NotRunning,
        ProcessRunning,
        GameBooted,
        Hooked
    }

    public class XDState
    {
        readonly byte[] UnloadedSaveDataMarker = new byte[] { 0x30, 0, 0, 0, 0, 0, 0, 0 };
        const uint BattleFlagOffset = 0x804EB910;

        public readonly PlayerSaveData SaveData = new PlayerSaveData();
        public readonly BattleDataLayout BattleData = new BattleDataLayout();
        public readonly FlagData FlagData;

        public ushort BattleId => Dolphin?.IsRunning == true ? Dolphin.ReadData(BattleFlagOffset + 2, 2).GetUShort() : (ushort)0;
        public bool InBattle => BattleId > 0;  

        public Room CurrentRoom
        {
            get;
            private set;
        }

        Dolphin Dolphin;
        ISO ISO;

        public XDState(Dolphin dolphinProcess, ISO iso)
        {
            Dolphin = dolphinProcess;
            ISO = iso;
            FlagData = new FlagData(Dolphin, ISO);
        }

        public async Task<bool> Setup(string gamePath)
        {
            return await Dolphin.SetupDolphin(gamePath);
        }

        public DolphinState Update()
        {
            if (Dolphin.IsRunning)
            {
                var saveDataPtr = PointerLocations.GetR13RelativePointer(Dolphin, PointerLocations.SaveDataR13Offset) + 0x140;
                if (saveDataPtr < Dolphin.EmulatedMemoryBase)
                {
                    return DolphinState.ProcessRunning;
                }

                var saveDataStart = Dolphin.ReadData(saveDataPtr, 8);
                if (saveDataStart.SequenceEqual(UnloadedSaveDataMarker))
                {
                    return DolphinState.GameBooted;
                }

                SaveData.LoadFromMemory(Dolphin);


                var roomId = BinaryPrimitives.ReadUInt16BigEndian(Dolphin.ReadData(0x80814ab6, 2));
                CurrentRoom = Room.FromId(roomId, ISO);


                if (InBattle)
                    BattleData.LoadFromMemory(Dolphin);

                return DolphinState.Hooked;
            }

            return DolphinState.NotRunning;
        }
    }
}
