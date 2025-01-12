using DolphinMemoryAccess;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
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
        const uint RoomIdOffset = 0x80814ab6;

        public readonly PlayerSaveData SaveData = new PlayerSaveData();
        public readonly BattleDataLayout BattleData = new BattleDataLayout();
        public FlagData FlagData { get; private set; }

        public ushort BattleId => Dolphin?.IsRunning == true ? Dolphin.ReadData(BattleFlagOffset, 2).GetUShort() : (ushort)0;
        public bool InBattle => BattleId > 0;  

        public Room CurrentRoom
        {
            get;
            private set;
        }

        public Dolphin Dolphin { get; private set; }

        ISO ISO;

        public XDState(ISO iso)
        {
            ISO = iso;
        }

        public async Task<bool> StartNewInstance(string dolphinPath, string gamePath)
        {
            Dolphin = new Dolphin(dolphinPath);
            FlagData = new FlagData(Dolphin, ISO);

            var started = await Dolphin.SetupDolphin(gamePath);
            if (started)
            {
                Dolphin.WriteData(0x803ee7C0, File.ReadAllBytes("ASM/archipelago.bin"));
                Dolphin.WriteData(0x81500000, new byte[] { 0xFF, 0xFF });
            }
            return started;
        }
        public DolphinState Update()
        {
            if (Dolphin?.IsRunning == true)
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

                var roomId = Dolphin.ReadData(RoomIdOffset, 2).GetUShort();
                CurrentRoom = Room.FromId(roomId, ISO);


                if (InBattle)
                    BattleData.LoadFromMemory(Dolphin);

                return DolphinState.Hooked;
            }

            return DolphinState.NotRunning;
        }
    }
}
