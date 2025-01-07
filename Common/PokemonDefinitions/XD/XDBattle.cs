using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using XDCommon.Contracts;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class XDBattle : Battle
    {

        const byte battleBattleTypeOffset = 0x0;
        const byte battleTrainersPerSideOffset = 0x1;
        const byte battleStyleOffset = 0x2;
        const byte battlePokemonPerPlayerOffset = 0x3;
        const byte battleUnknown1Offset = 0x4; // 1 in story, 0 otherwise. could be whether or not to receive prize money or black out etc.
        const byte battleFieldOffset = 0x6;
        const byte battleBattleCDIDOffset = 0x8; // 2 bytes, set programmatically so is always 0 in the game files
        const byte battleBGMOffset = 0x10;
        const byte battleMysteryOffset = 0x14;
        const byte battleMystery2Offset = 0x16;
        const byte battleUnknown2Offset = 0x17;
        const byte battleMystery3Offset = 0x18;
        const byte battleMystery4Offset = 0x1a;
        const byte battleColosseumRoundOffset = 0x1b;

        int sizeOfBattleData(Region region) 
        {
            return region == Region.Europe ? 0x4C : 0x3C;
        }

        int battlePlayer1DeckIDOffset(Region region)
        {
            return region == Region.Europe ? 0x2C : 0x1C;
        }

        int battlePlayer1TrainerIDOffset(Region region){ return region == Region.Europe ? 0x2E : 0x1E; }
        int battlePlayer1ControlOffset(Region region){ return region == Region.Europe ? 0x23 : 0x23; }

        int battlePlayer2DeckIDOffset(Region region){ return region == Region.Europe ? 0x34 : 0x24; }
        int battlePlayer2TrainerIDOffset(Region region){ return region == Region.Europe ? 0x36 : 0x26; }
        int battlePlayer2ControlOffset(Region region){ return region == Region.Europe ? 0x3B : 0x2B; }

        int battlePlayer3DeckIDOffset(Region region){ return region == Region.Europe ? 0x3C : 0x2C; }
        int battlePlayer3TrainerIDOffset(Region region){ return region == Region.Europe ? 0x3E : 0x2E; }
        int battlePlayer3ControlOffset(Region region){ return region == Region.Europe ? 0x33 : 0x33; }

        int battlePlayer4DeckIDOffset(Region region){ return region == Region.Europe ? 0x44 : 0x34; }
        int battlePlayer4TrainerIDOffset(Region region){ return region == Region.Europe ? 0x46 : 0x36; }
        int battlePlayer4ControlOffset(Region region){ return region == Region.Europe ? 0x4B : 0x3B; }

        uint StartOffset => (uint)(iso.CommonRel.GetPointer(Constants.XDBattles) + (Index * sizeOfBattleData(iso.Region)));


        public BattleTypes BattleType
        {
            get => (BattleTypes)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + battleBattleTypeOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + battleBattleTypeOffset, (byte)value);
        }

        public BattleStyles BattleStyle
        {
            get => (BattleStyles)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + battleStyleOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + battleStyleOffset, (byte)value);
        }
        public ushort BattleFieldId
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battleFieldOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + battleFieldOffset, value.GetBytes());
        } 
        
        public XDColosseumRounds Round
        {
            get => (XDColosseumRounds)iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battleColosseumRoundOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + battleColosseumRoundOffset, (byte)value);
        }

        public ushort Mystery1 => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battleMysteryOffset);
        public ushort Mystery2 => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battleMystery2Offset);
        public ushort Unknown2 => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battleUnknown2Offset);
        public ushort Mystery3 => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battleMystery3Offset);
        public ushort Mystery4 => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battleMystery4Offset);


        public ushort FirstTrainerIndex
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battlePlayer1TrainerIDOffset(iso.Region));
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + battlePlayer1TrainerIDOffset(iso.Region), value.GetBytes());
        }

	    public TrainerPoolType FirstTrainerPoolType
        {
            get => (TrainerPoolType)iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battlePlayer1DeckIDOffset(iso.Region));
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + battlePlayer1DeckIDOffset(iso.Region), ((ushort)value).GetBytes());
        }

        public ushort SecondTrainerIndex
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battlePlayer2TrainerIDOffset(iso.Region));
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + battlePlayer2TrainerIDOffset(iso.Region), value.GetBytes());
        }

        public TrainerPoolType SecondTrainerPoolType
        {
            get => (TrainerPoolType)iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battlePlayer2DeckIDOffset(iso.Region));
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + battlePlayer2DeckIDOffset(iso.Region), ((ushort)value).GetBytes());
        }

        public ushort ThirdTrainerIndex
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battlePlayer3TrainerIDOffset(iso.Region));
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + battlePlayer3TrainerIDOffset(iso.Region), value.GetBytes());
        }
        public TrainerPoolType ThirdTrainerPoolType
        {
            get => (TrainerPoolType)iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battlePlayer3DeckIDOffset(iso.Region));
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + battlePlayer3DeckIDOffset(iso.Region), ((ushort)value).GetBytes());
        }

        public ushort FourthTrainerIndex
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battlePlayer4TrainerIDOffset(iso.Region));
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + battlePlayer4TrainerIDOffset(iso.Region), value.GetBytes());
        }
        public TrainerPoolType FourthTrainerPoolType
        {
            get => (TrainerPoolType)iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + battlePlayer4DeckIDOffset(iso.Region));
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + battlePlayer4DeckIDOffset(iso.Region), ((ushort)value).GetBytes());
        }

        public XDBattle(int index, ISO iso) : base(index, iso)
        {
        }

        public static XDBattle FindPlayerBattleFromTrainer(int trainerIndex, BattleTypes[] battleType, ISO iso)
        {
            for (int i = 0; i < iso.CommonRel.GetValueAtPointer(Constants.XDNumberOfBattles); i++)
            {
                var battle = new XDBattle(i, iso);
                if (!battleType.Contains(battle.BattleType))
                {
                    continue;
                }

                if (trainerIndex == battle.FirstTrainerIndex)
                {
                    return battle;
                }
                else if (trainerIndex == battle.SecondTrainerIndex)
                {
                    return battle;
                }
                else if (trainerIndex == battle.ThirdTrainerIndex)
                {
                    return battle;
                }
                else if (trainerIndex == battle.FourthTrainerIndex)
                {
                    return battle;
                }
            }
            return null;
        }
    }
}
