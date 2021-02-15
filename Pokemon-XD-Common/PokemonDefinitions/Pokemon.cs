using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class Pokemon
    {
        const byte PokemonNameIDOFfset = 0x18;
        const int SizeOfPokemonStats = 0x124;

        const byte EXPRateOffset = 0x00;
        const byte CatchRateOffset = 0x01;
        const byte GenderRatioOffset = 0x02;
        const byte BaseEXPOffset = 0x05;
        const byte BaseHappinessOffset = 0x07;
        const byte HeightOffset = 0x08;
        const byte WeightOffset = 0x0A;

        const byte NationalIndexOffset = 0x0E;
        const byte Type1Offset = 0x30;
        const byte Type2Offset = 0x31;
        const byte Ability1Offset = 0x32;
        const byte Ability2Offset = 0x33;
        const byte HeldItem1Offset = 0x7A;
        const byte HeldItem2Offset = 0x7C;

        const byte HPOffset = 0x8F;
        const byte AttackOffset = 0x91;
        const byte DefenseOffset = 0x93;
        const byte SpecialAttackOffset = 0x95;
        const byte SpecialDefenseOffset = 0x97;
        const byte SpeedOffset = 0x99;
        const byte FirstEVYieldOffset = 0x9A; // 1 byte between each one.
        const byte FirstTMOffset = 0x34;
        const byte FirstEvolutionOffset = 0xA6;
        const byte FirstTutorMoveOffset = 0x6E;
        const byte FirstLevelUpMoveOffset = 0xC4;
        const byte FirstEggMoveOffset = 0x7E;

        const byte EvolutionMethodOffset = 0x0;
        const byte EvolutionConditionOffset = 0x2;
        const byte EvovledFormOffset = 0x4;

        int dexNum;
        ISO iso;
        public int Index => dexNum;
        public int NameID => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + PokemonNameIDOFfset);
        public string Name => iso.CommonRelStringTable.GetStringWithId(NameID).ToString();
        public int SpeciesNameID => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + Constants.SpeciesNameIDOffset);
        public double Height => (double)iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + HeightOffset) / 10;
        public double Weight => (double)iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + WeightOffset) / 10;
        
        public int StartOffset
        {
            get
            {
                var stats = iso.Game == Game.XD ? Constants.XDPokemonStats : Constants.ColPokemonStats;
                return (int)(iso.CommonRel.GetPointer(stats) + (dexNum * SizeOfPokemonStats));
            }
        }

        public bool[] LearnableTMs { get; }
        public bool[] TutorMoves { get; }
        public LevelUpMove[] LevelUpMoves { get; }
        
        public Evolution[] Evolutions { get; set; }

        public ushort HeldItem
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + HeldItem1Offset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + HeldItem1Offset, value.GetBytes());
        }

        public byte Ability1
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Ability1Offset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Ability1Offset, value);
        }

        public byte Ability2
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Ability2Offset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Ability2Offset, value);
        }

        public PokemonTypes Type1
        {
            get => (PokemonTypes)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Type1Offset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Type1Offset, (byte)value);
        }

        public PokemonTypes Type2
        {
            get => (PokemonTypes)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Type2Offset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Type2Offset, (byte)value);
        }

        public GenderRatios GenderRatio
        {
            get => (GenderRatios)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + GenderRatioOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + GenderRatioOffset, (byte)value);
        }

        public ExpRate LevelUpRate
        {
            get => (ExpRate)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + EXPRateOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + EXPRateOffset, (byte)value);
        }

        public byte CatchRate
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + CatchRateOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + CatchRateOffset, value);
        }

        public byte BaseExp
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + BaseEXPOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + BaseEXPOffset, value);
        }

        public byte BaseHappiness
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + BaseHappinessOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + BaseHappinessOffset, value);
        }


        public int BST
        {
            get => HP + Attack + Defense + SpecialAttack + SpecialDefense + Speed;
        }

        public byte HP
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + HPOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + HPOffset, value);
        }

        public byte Speed
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + SpeedOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + SpeedOffset, value);
        }

        public byte Attack
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + AttackOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + AttackOffset, value);
        }

        public byte Defense
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + DefenseOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + DefenseOffset, value);
        }

        public byte SpecialAttack
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + SpecialAttackOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + SpecialAttackOffset, value);
        }

        public byte SpecialDefense
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + SpecialDefenseOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + SpecialDefenseOffset, value);
        }

        public ushort HPYield
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + FirstEVYieldOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + FirstEVYieldOffset, value.GetBytes());
        }
        public ushort AttackYield
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + FirstEVYieldOffset + 0x2);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + FirstEVYieldOffset + 0x2, value.GetBytes());
        }

        public ushort DefenseYield
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + FirstEVYieldOffset + 0x4);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + FirstEVYieldOffset + 0x4, value.GetBytes());
        }

        public ushort SpecialAttackYield
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + FirstEVYieldOffset + 0x6);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + FirstEVYieldOffset + 0x6, value.GetBytes());
        }

        public ushort SpecialDefenseYield
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + FirstEVYieldOffset + 0x8);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + FirstEVYieldOffset + 0x8, value.GetBytes());
        }

        public ushort SpeedYield
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + FirstEVYieldOffset + 0xA);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + FirstEVYieldOffset + 0xA, value.GetBytes());
        }

        public Pokemon(int dexNumber, ISO iso)
        {
            dexNum = dexNumber;
            this.iso = iso;

            LearnableTMs = new bool[Constants.NumberOfTMsandHMs];
            for (int i = 0; i < Constants.NumberOfTMsandHMs; i++)
            {
                LearnableTMs[i] = iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + FirstTMOffset + i) == 1;
            }

            if (iso.Game == Game.XD)
            {
                TutorMoves = new bool[Constants.NumberOfTutorMoves];
                for (int i = 0; i < Constants.NumberOfTutorMoves; i++)
                {
                    TutorMoves[i] = iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + FirstTutorMoveOffset + i) == 1;
                }
            }
            else
            {
                TutorMoves = Array.Empty<bool>();
            }

            var cuurentOffset = StartOffset + FirstLevelUpMoveOffset;
            LevelUpMoves = new LevelUpMove[Constants.NumberOfLevelUpMoves];
            for (int i = 0; i < Constants.NumberOfLevelUpMoves; i++)
            {
                var level = iso.CommonRel.ExtractedFile.GetByteAtOffset(cuurentOffset + Constants.LevelUpMoveLevelOffset);
                var move = iso.CommonRel.ExtractedFile.GetUShortAtOffset(cuurentOffset + Constants.LevelUpMoveIndexOffset);
                LevelUpMoves[i] = new LevelUpMove(
                    level,
                    move
                );
                cuurentOffset += Constants.SizeOfLevelUpData;
            }

            cuurentOffset = StartOffset + FirstEvolutionOffset;
            Evolutions = new Evolution[Constants.NumberOfEvolutions];
            for (int i = 0; i < Constants.NumberOfEvolutions; i++)
            {
                var method = iso.CommonRel.ExtractedFile.GetByteAtOffset(cuurentOffset + EvolutionMethodOffset);
                var condtion = iso.CommonRel.ExtractedFile.GetUShortAtOffset(cuurentOffset + EvolutionConditionOffset);
                var evolution = iso.CommonRel.ExtractedFile.GetUShortAtOffset(cuurentOffset + EvovledFormOffset);
                Evolutions[i] = new Evolution(
                    method, 
                    condtion,
                    evolution
                );
                cuurentOffset += Constants.SizeOfEvolutionData;
            }
        }

        public void SetLearnableTMS(int index, bool canLearn)
        {
            iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + FirstTMOffset + index, canLearn ? (byte)1 : (byte)0);
            LearnableTMs[index] = canLearn;
        }

        public void SetTutorMoves(int index, bool canLearn)
        {
            iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + FirstTMOffset + index, canLearn ? (byte)1 : (byte)0);
            TutorMoves[index] = canLearn;
        }

        public void SetEvolution(int index, byte method, ushort condition, ushort evolvesInto)
        {
            var offset = StartOffset + FirstLevelUpMoveOffset + (index * Constants.SizeOfLevelUpData);
            iso.CommonRel.ExtractedFile.WriteByteAtOffset(offset + EvolutionMethodOffset, method);
            iso.CommonRel.ExtractedFile.WriteBytesAtOffset(offset + EvolutionConditionOffset, condition.GetBytes());
            iso.CommonRel.ExtractedFile.WriteBytesAtOffset(offset + EvovledFormOffset, evolvesInto.GetBytes());
            Evolutions[index] = new Evolution(method, condition, evolvesInto);
        }

        public void SetLevelUpMove(int index, byte level, ushort move)
        {
            var offset = StartOffset + FirstLevelUpMoveOffset + (index * Constants.SizeOfLevelUpData);
            iso.CommonRel.ExtractedFile.WriteByteAtOffset(offset + Constants.LevelUpMoveLevelOffset, level);
            iso.CommonRel.ExtractedFile.WriteBytesAtOffset(offset + Constants.LevelUpMoveIndexOffset, move.GetBytes());
            LevelUpMoves[index] = new LevelUpMove(level, move);
        }

        public IEnumerable<LevelUpMove> CurrentLevelMoves(int level)
        {
            return LevelUpMoves.Where(m => m.Level != 0 && m.Level <= level).TakeLast(Constants.NumberOfPokemonMoves);
        }
    }
}
