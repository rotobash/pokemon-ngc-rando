using System;
using System.Collections.Generic;
using System.IO;
using XDCommon.Utility;
using XDCommon.Utility.Extensions;

namespace XDCommon.PokemonDefinitions.XD.SaveData
{
    public class PokemonInstance
    {
        public const int SizeOfData = 0xC4;
        const byte SpeciesOffset = 0x00;
        const byte ItemOffset = 0x02;
        const byte CurrentHPsOffset = 0x04;
        const byte HappinessOffset = 0x06;
        const byte LevelObtainedOffset = 0x0E;
        const byte PokeballTypeOffset = 0x0F;
        const byte CurrentLevelOffset = 0x10;
        const byte SecretIDOffset = 0x24;
        const byte TrainerIDOffset = 0x26;
        const byte PIDOffset = 0x28;
        const byte OTNameOffset = 0x39;
        const byte NameFieldOffset = 0x4F;
        const byte NameField2Offset = 0x65;
        const byte RibbonsBitsOffset = 0x7C;
        const byte Move1Offset = 0x80;
        const byte Move1PPOffset = 0x82;

        const byte HPStatOffset = 0x90;
        const byte AtkStatOffset = 0x92;
        const byte DefStatOffset = 0x94;
        const byte SpAStatOffset = 0x96;
        const byte SpDStatOffset = 0x98;
        const byte SpEStatOffset = 0x9A;
        const byte HPEVOffset = 0x9C;
        const byte AtkEVOffset = 0x9E;
        const byte DefEVOffset = 0xA0;
        const byte SpAEVOffset = 0xA2;
        const byte SpDEVOffset = 0xA4;
        const byte SpEEVOffset = 0xA6;
        const byte HPIVOffset = 0xA8;
        const byte AtkIVOffset = 0xA9;
        const byte DefIVOffset = 0xAA;
        const byte SpAIVOffset = 0xAB;
        const byte SpDIVOffset = 0xAC;
        const byte SpEIVOffset = 0xAD;

        public ushort Species 
        { 
            get => Data.GetUShortAtOffset(SpeciesOffset);
            set => Data.WriteBytesAtOffset(SpeciesOffset, value.GetBytes());
        }

        public ushort HeldItem
        {
            get => Data.GetUShortAtOffset(ItemOffset);
            set => Data.WriteBytesAtOffset(ItemOffset, value.GetBytes());
        }
        public ushort CurrentHP
        {
            get => Data.GetUShortAtOffset(CurrentHPsOffset);
            set => Data.WriteBytesAtOffset(CurrentHPsOffset, value.GetBytes());
        }

        public ushort Happiness
        {
            get => Data.GetUShortAtOffset(HappinessOffset);
            set => Data.WriteBytesAtOffset(HappinessOffset, value.GetBytes());
        }

        public byte LevelObtained
        {
            get => Data.GetByteAtOffset(LevelObtainedOffset);
            set => Data.WriteByteAtOffset(LevelObtainedOffset, value);
        }

        public byte PokeballType
        {
            get => Data.GetByteAtOffset(PokeballTypeOffset);
            set => Data.WriteByteAtOffset(PokeballTypeOffset, value);
        }

        public ushort CurrentLevel
        {
            get => Data.GetUShortAtOffset(CurrentLevelOffset);
            set => Data.WriteBytesAtOffset(CurrentLevelOffset, value.GetBytes());
        }
        public ushort SecretId
        {
            get => Data.GetUShortAtOffset(SecretIDOffset);
            set => Data.WriteBytesAtOffset(SecretIDOffset, value.GetBytes());
        }

        public ushort TrainerId
        {
            get => Data.GetUShortAtOffset(TrainerIDOffset);
            set => Data.WriteBytesAtOffset(TrainerIDOffset, value.GetBytes());
        }

        public int PID
        {
            get => Data.GetIntAtOffset(PIDOffset);
            set => Data.WriteBytesAtOffset(PIDOffset, value.GetBytes());
        }
        public UnicodeString OTName
        {
            get => Data.GetStringAtOffset(OTNameOffset);
            set => Data.WriteBytesAtOffset(OTNameOffset, value.ToByteArray());
        }

        public UnicodeString Name
        {
            get => Data.GetStringAtOffset(NameFieldOffset);
            set => Data.WriteBytesAtOffset(NameFieldOffset, value.ToByteArray());
        }

        public UnicodeString Name2
        {
            get => Data.GetStringAtOffset(NameField2Offset);
            set => Data.WriteBytesAtOffset(NameField2Offset, value.ToByteArray());
        }

        public ushort RibbonsBits
        {
            get => Data.GetUShortAtOffset(RibbonsBitsOffset);
            set => Data.WriteBytesAtOffset(RibbonsBitsOffset, value.GetBytes());
        }

        public LearnedMove[] LearnedMoves { get; set; } = new LearnedMove[4];



        public ushort HP
        {
            get => Data.GetUShortAtOffset(HPStatOffset);
            set => Data.WriteBytesAtOffset(HPStatOffset, value.GetBytes());
        }

        public ushort Speed
        {
            get => Data.GetUShortAtOffset(SpEStatOffset);
            set => Data.WriteBytesAtOffset(SpEStatOffset, value.GetBytes());
        }

        public ushort Attack
        {
            get => Data.GetUShortAtOffset(AtkStatOffset);
            set => Data.WriteBytesAtOffset(AtkStatOffset, value.GetBytes());
        }

        public ushort Defense
        {
            get => Data.GetUShortAtOffset(DefStatOffset);
            set => Data.WriteBytesAtOffset(DefStatOffset, value.GetBytes());
        }

        public ushort SpecialAttack
        {
            get => Data.GetUShortAtOffset(SpAStatOffset);
            set => Data.WriteBytesAtOffset(SpAStatOffset, value.GetBytes());
        }

        public ushort SpecialDefense
        {
            get => Data.GetUShortAtOffset(SpDStatOffset);
            set => Data.WriteBytesAtOffset(SpDStatOffset, value.GetBytes());
        }

        public ushort HPEV
        {
            get => Data.GetUShortAtOffset(HPEVOffset);
            set => Data.WriteBytesAtOffset(HPEVOffset, value.GetBytes());
        }

        public ushort SpeedEV
        {
            get => Data.GetUShortAtOffset(SpEEVOffset);
            set => Data.WriteBytesAtOffset(SpEEVOffset, value.GetBytes());
        }

        public ushort AttackEV
        {
            get => Data.GetUShortAtOffset(AtkEVOffset);
            set => Data.WriteBytesAtOffset(AtkEVOffset, value.GetBytes());
        }

        public ushort DefenseEV
        {
            get => Data.GetUShortAtOffset(DefEVOffset);
            set => Data.WriteBytesAtOffset(DefEVOffset, value.GetBytes());
        }

        public ushort SpecialAttackEV
        {
            get => Data.GetUShortAtOffset(SpAEVOffset);
            set => Data.WriteBytesAtOffset(SpAEVOffset, value.GetBytes());
        }

        public ushort SpecialDefenseEV
        {
            get => Data.GetUShortAtOffset(SpDEVOffset);
            set =>  Data.WriteBytesAtOffset(SpDEVOffset, value.GetBytes());
        }

        public byte HPIV
        {
            get => Data.GetByteAtOffset(HPIVOffset);
            set => Data.WriteByteAtOffset(HPIVOffset, value);
        }

        public byte SpeedIV
        {
            get => Data.GetByteAtOffset(SpEIVOffset);
            set => Data.WriteByteAtOffset(SpEIVOffset, value);
        }

        public byte AttackIV
        {
            get => Data.GetByteAtOffset(AtkIVOffset);
            set => Data.WriteByteAtOffset(AtkIVOffset, value);
        }

        public byte DefenseIV
        {
            get => Data.GetByteAtOffset(DefIVOffset);
            set => Data.WriteByteAtOffset(DefIVOffset, value);
        }

        public byte SpecialAttackIV
        {
            get => Data.GetByteAtOffset(SpAIVOffset);
            set => Data.WriteByteAtOffset(SpAIVOffset, value);
        }

        public byte SpecialDefenseIV
        {
            get => Data.GetByteAtOffset(SpDIVOffset);
            set => Data.WriteByteAtOffset(SpDIVOffset, value);
        }

        public bool IsSet => Species != 0;

        MemoryStream Data { get; }

        public PokemonInstance(byte[] bytes)
        {
            Data = new MemoryStream(bytes);

            for (int i = 0; i < 4; i++)
            {
                ushort moveId = Data.GetUShortAtOffset(Move1Offset + (i * 4));
                byte pp = Data.GetByteAtOffset(Move1PPOffset + (i * 4));
                LearnedMoves[i] = new LearnedMove 
                {
                   Index = moveId,
                   PP = pp
                };
            }
        }

        ~PokemonInstance()
        {
            if (Data != null)
                Data.Dispose();
        }

        public void SetMove(int index, ushort moveId, byte pp)
        {
            if (index < 0 || index > 3)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 3");

            Data.WriteBytesAtOffset(Move1Offset + (index * 4), moveId.GetBytes());
            Data.WriteByteAtOffset(Move1PPOffset + (index * 4), pp);
        }
    }
}
