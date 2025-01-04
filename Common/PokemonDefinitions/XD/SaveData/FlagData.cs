using DolphinMemoryAccess;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions.XD.SaveData
{
    public class FlagData
    {
        Dolphin Dolphin;
        ISO ISO;

        public FlagData(Dolphin dolphin, ISO iso)
        {
            Dolphin = dolphin;
            ISO = iso;
        }

	    public Room MirorBLocation()
        {
		    var roomID = (int)GetFlag(FlagsTypes.MirorbLocation);
		    return new Room(roomID, ISO);
	    }

	    public XDStoryFlags StoryProgress() 
        {
            var storyFlag = GetFlag(FlagsTypes.Story);
		    return (XDStoryFlags)storyFlag;
        }

	    public Pokemon GetSpawnAtPokespot(PokeSpotFlagTypes flagType)
        {
            FlagsTypes flag;
		    switch (flagType) 
            {
		        case PokeSpotFlagTypes.Rock: 
                    flag = FlagsTypes.CurrentPokespotSpawnRock; 
                    break;
		        case PokeSpotFlagTypes.Oasis:
                    flag = FlagsTypes.CurrentPokespotSpawnOasis;
                    break;
		        case PokeSpotFlagTypes.Cave:
                    flag = FlagsTypes.CurrentPokespotSpawnCave;
                    break;
                default:
                    return null;
            }

		    var species = (int)GetFlag(flag);
		    return new XDPokemon(species, ISO);
        }

        public int GetSnacksAtPokespot(PokeSpotFlagTypes flagType)
        {
            FlagsTypes flag;
            switch (flagType)
            {
                case PokeSpotFlagTypes.Rock:
                    flag = FlagsTypes.CurrentPokespotSpawnRock;
                    break;
                case PokeSpotFlagTypes.Oasis:
                    flag = FlagsTypes.CurrentPokespotSpawnOasis;
                    break;
                case PokeSpotFlagTypes.Cave:
                    flag = FlagsTypes.CurrentPokespotSpawnCave;
                    break;
                default:
                    return 0;
            }
            return (int)GetFlag(flag);
        }

        public uint GetFlag(FlagsTypes flagType)
        {
            var region = (Region)Dolphin.GameRegionCode;
            var flagId = (int)flagType;

            var flagStartOffset = ISO.CommonRel.GetPointer(Constants.XDGeneralFlags) + REL.RELtoRAMOffset(region, Game.XD);
            var flagMetadatStartOffset = ISO.CommonRel.GetPointer(Constants.XDFlagsMetaData) + REL.RELtoRAMOffset(region, Game.XD);

            var flagOffset = flagStartOffset + flagId * 6;

            var unknown1 = BinaryPrimitives.ReadUInt16BigEndian(Dolphin.ReadData(flagOffset + 2, 2));
            var flagByte = (Dolphin.ReadData(flagOffset, 1)[0]);
            var unknown3 = unknown1 >> 5;
            var flagByteAnd = flagByte & 0x3f;
            var unknown5 = unknown1 & 0x1f;

            var unknownAddress = BinaryPrimitives.ReadUInt32BigEndian(Dolphin.ReadData(flagMetadatStartOffset + (flagByte >> 3 & 0x18) + 4, 4));
            if (unknownAddress <= 0)
                return 0;

            if (flagByteAnd < 2)
            {
                var flagMetaDataValue = BinaryPrimitives.ReadUInt32BigEndian(Dolphin.ReadData(unknownAddress + (unknown3 * 4), 4));
                return (flagMetaDataValue >> unknown5) & 1;
            }
            else
            {
                var offset = unknown3 * 4;
                var flagA = BinaryPrimitives.ReadUInt32BigEndian(Dolphin.ReadData(unknownAddress + offset + 4, 4));
                var flagB = BinaryPrimitives.ReadUInt32BigEndian(Dolphin.ReadData(unknownAddress + offset, 4));
                var flagC = BinaryPrimitives.ReadUInt32BigEndian(Dolphin.ReadData(0x8040b4a0 + (flagByteAnd * 4), 4));

                return ((flagA << (0x20 - unknown5)) | (flagB >> unknown5)) & flagC;
            }
        }
    }
}
