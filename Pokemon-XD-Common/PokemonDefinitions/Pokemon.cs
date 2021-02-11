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
        int dexNum;
        ISO iso;
        public int Index => dexNum;
        public int NameID => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + Constants.PokemonNameIDOFfset);
        public string Name => iso.CommonRelStringTable.GetStringWithId(NameID).ToString();
        public int SpeciesNameID => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + Constants.SpeciesNameIDOffset);
        public double Height => (double)iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.HeightOffset) / 10;
        public double Weight => (double)iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.WeightOffset) / 10;
        
        public int StartOffset
        {
            get
            {
                var stats = iso.Game == Game.XD ? Constants.XDPokemonStats : Constants.ColPokemonStats;
                return (int)(iso.CommonRel.GetPointer(stats) + (dexNum * Constants.SizeOfPokemonStats));
            }
        }

        public bool[] LearnableTMs { get; }
        public bool[] TutorMoves { get; }
        public LevelUpMove[] LevelUpMoves { get; }
        
        public Evolution[] Evolutions { get; set; }

        public Items HeldItem
        {
            get => null;//(Items)commonRel.ExtractedFile.GetUshortAtOffset(StartOffset + Constants.HeldItem1Offset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.HeldItem1Offset, Array.Empty<byte>());
        }

        public Ability Ability1
        {
            get;
            private set;
        }

        public Ability Ability2
        {
            get;
            private set;
        }

        public PokemonTypes Type1
        {
            get => (PokemonTypes)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.Type1Offset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.Type1Offset, (byte)value);
        }

        public PokemonTypes Type2
        {
            get => (PokemonTypes)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.Type2Offset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.Type2Offset, (byte)value);
        }

        public GenderRatios GenderRatio
        {
            get => (GenderRatios)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.GenderRatioOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.GenderRatioOffset, (byte)value);
        }

        public ExpRate LevelUpRate
        {
            get => (ExpRate)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.EXPRateOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.EXPRateOffset, (byte)value);
        }

        public byte CatchRate
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.CatchRateOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.CatchRateOffset, value);
        }

        public byte BaseExp
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.BaseEXPOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.BaseEXPOffset, value);
        }

        public byte BaseHappiness
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.BaseHappinessOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.BaseHappinessOffset, value);
        }


        public int BST
        {
            get => Attack + Defense + SpecialAttack + SpecialDefense + Speed;
        }

        public byte HP
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.HPOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.HPOffset, value);
        }

        public byte Speed
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.SpeedOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.SpeedOffset, value);
        }

        public byte Attack
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.AttackOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.AttackOffset, value);
        }

        public byte Defense
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.DefenseOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.DefenseOffset, value);
        }

        public byte SpecialAttack
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.SpecialAttackOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.SpecialAttackOffset, value);
        }

        public byte SpecialDefense
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.SpecialDefenseOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.SpecialDefenseOffset, value);
        }

        public ushort HPYield
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstEVYieldOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstEVYieldOffset, value.GetBytes());
        }
        public ushort AttackYield
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x2);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x2, value.GetBytes());
        }

        public ushort DefenseYield
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x4);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x4, value.GetBytes());
        }

        public ushort SpecialAttackYield
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x6);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x6, value.GetBytes());
        }

        public ushort SpecialDefenseYield
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x8);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x8, value.GetBytes());
        }

        public ushort SpeedYield
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0xA);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0xA, value.GetBytes());
        }

        public Pokemon(int dexNumber, ISO iso)
        {
            dexNum = dexNumber;
            this.iso = iso;

            Ability1 = new Ability(iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.Ability1Offset), iso);
            Ability2 = new Ability(iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.Ability2Offset), iso);

            LearnableTMs = new bool[Constants.NumberOfTMsandHMs];
            for (int i = 0; i < Constants.NumberOfTMsandHMs; i++)
            {
                LearnableTMs[i] = iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.FirstTMOffset + i) == 1;
            }

            if (iso.Game == Game.XD)
            {
                TutorMoves = new bool[Constants.NumberOfTutorMoves];
                for (int i = 0; i < Constants.NumberOfTutorMoves; i++)
                {
                    TutorMoves[i] = iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.FirstTutorMoveOffset + i) == 1;
                }
            }
            else
            {
                TutorMoves = Array.Empty<bool>();
            }

            var cuurentOffset = StartOffset + Constants.FirstLevelUpMoveOffset;
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

            cuurentOffset = StartOffset + Constants.FirstEvolutionOffset;
            Evolutions = new Evolution[Constants.NumberOfEvolutions];
            for (int i = 0; i < Constants.NumberOfEvolutions; i++)
            {
                var method = iso.CommonRel.ExtractedFile.GetByteAtOffset(cuurentOffset + Constants.EvolutionMethodOffset);
                var condtion = iso.CommonRel.ExtractedFile.GetUShortAtOffset(cuurentOffset + Constants.EvolutionConditionOffset);
                var evolution = iso.CommonRel.ExtractedFile.GetUShortAtOffset(cuurentOffset + Constants.EvovledFormOffset);
                Evolutions[i] = new Evolution(
                    method, 
                    condtion,
                    evolution
                );
                cuurentOffset += Constants.SizeOfEvolutionData;
            }
        }

        public void SetAbility1(byte abilityID)
        {
            iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.Ability1Offset, abilityID);
            Ability1 = new Ability(abilityID, iso);
        }

        public void SetAbility2(byte abilityID)
        {
            iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.Ability2Offset, abilityID);
            Ability2 = new Ability(abilityID, iso);
        }

        public void SetLearnableTMS(int index, bool canLearn)
        {
            iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.FirstTMOffset + index, canLearn ? (byte)1 : (byte)0);
            LearnableTMs[index] = canLearn;
        }

        public void SetTutorMoves(int index, bool canLearn)
        {
            iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.FirstTMOffset + index, canLearn ? (byte)1 : (byte)0);
            TutorMoves[index] = canLearn;
        }

        public void SetEvolution(int index, byte method, ushort condition, ushort evolvesInto)
        {
            var offset = StartOffset + Constants.FirstLevelUpMoveOffset + (index * Constants.SizeOfLevelUpData);
            iso.CommonRel.ExtractedFile.WriteByteAtOffset(offset + Constants.EvolutionMethodOffset, method);
            iso.CommonRel.ExtractedFile.WriteBytesAtOffset(offset + Constants.EvolutionConditionOffset, condition.GetBytes());
            iso.CommonRel.ExtractedFile.WriteBytesAtOffset(offset + Constants.EvovledFormOffset, evolvesInto.GetBytes());
            Evolutions[index] = new Evolution(method, condition, evolvesInto);
        }

        public void SetLevelUpMove(int index, byte level, ushort move)
        {
            var offset = StartOffset + Constants.FirstLevelUpMoveOffset + (index * Constants.SizeOfLevelUpData);
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
