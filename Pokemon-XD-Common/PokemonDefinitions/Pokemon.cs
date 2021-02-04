using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class Pokemon
    {
        int dexNum;
        REL commonRel;
        StringTable commonRelStrTbl;
        Game game;
        public int Index => dexNum;
        public int NameID => commonRel.ExtractedFile.GetIntAtOffset(StartOffset + Constants.PokemonNameIDOFfset);
        public UnicodeString Name => commonRelStrTbl.GetStringWithId(NameID);
        public int SpeciesNameID => commonRel.ExtractedFile.GetIntAtOffset(StartOffset + Constants.SpeciesNameIDOffset);
        public int Height => commonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.HeightOffset) / 10;
        public int Weight => commonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.WeightOffset) / 10;
        
        public uint StartOffset
        {
            get
            {
                var stats = game == Game.XD ? Constants.XDPokemonStats : Constants.ColPokemonStats;
                return (uint)(commonRel.GetPointer(stats) + (dexNum * Constants.SizeOfPokemonStats));
            }
        }

        public bool[] LearnableTMs { get; }
        public bool[] TutorMoves { get; }
        
        public object[] Evolutions { get; set; }

        public Items HeldItem
        {
            get => null;//(Items)commonRel.ExtractedFile.GetUshortAtOffset(StartOffset + Constants.HeldItem1Offset);
            set => commonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.HeldItem1Offset, Array.Empty<byte>());
        }

        public Abilities Ability1
        {
            get => (Abilities)commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.Ability1Offset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.Ability1Offset, (byte)value);
        }

        public Abilities Ability2
        {
            get => (Abilities)commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.Ability2Offset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.Ability2Offset, (byte)value);
        }

        public PokemonTypes Type1
        {
            get => (PokemonTypes)commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.Type1Offset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.Type1Offset, (byte)value);
        }

        public PokemonTypes Type2
        {
            get => (PokemonTypes)commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.Type2Offset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.Type2Offset, (byte)value);
        }

        public GenderRatios GenderRatio
        {
            get => (GenderRatios)commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.GenderRatioOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.GenderRatioOffset, (byte)value);
        }

        public ExpRate LevelUpRate
        {
            get => (ExpRate)commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.EXPRateOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.EXPRateOffset, (byte)value);
        }

        public byte CatchRate
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.CatchRateOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.CatchRateOffset, value);
        }

        public byte BaseExp
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.BaseEXPOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.BaseEXPOffset, value);
        }

        public byte BaseHappiness
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.BaseHappinessOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.BaseHappinessOffset, value);
        }


        public byte HP
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.HPOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.HPOffset, value);
        }

        public byte Speed
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.SpeedOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.SpeedOffset, value);
        }

        public byte Attack
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.AttackOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.AttackOffset, value);
        }

        public byte Defense
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.DefenseOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.DefenseOffset, value);
        }

        public byte SpecialAttack
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.SpecialAttackOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.SpecialAttackOffset, value);
        }

        public byte SpecialDefense
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.SpecialDefenseOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.SpecialDefenseOffset, value);
        }

        public ushort HPYield
        {
            get => commonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstEVYieldOffset);
            set => commonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstEVYieldOffset, value.GetBytes());
        }

        public ushort SpeedYield
        {
            get => commonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x2);
            set => commonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x2, value.GetBytes());
        }

        public ushort AttackYield
        {
            get => commonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x4);
            set => commonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x4, value.GetBytes());
        }

        public ushort DefenseYield
        {
            get => commonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x6);
            set => commonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x6, value.GetBytes());
        }

        public ushort SpecialAttackYield
        {
            get => commonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x8);
            set => commonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0x8, value.GetBytes());
        }

        public ushort SpecialDefenseYield
        {
            get => commonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0xA);
            set => commonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstEVYieldOffset + 0xA, value.GetBytes());
        }

        public Stats Stats
        {
            get;
        }

        public Pokemon(int dexNumber, ISO iso)
        {
            dexNum = dexNumber;
            commonRel = iso.CommonRel();
            commonRelStrTbl = iso.CommonRelStringTable();
            game = iso.Game;
            Stats = new Stats(dexNumber);
        }

        
    }
}
