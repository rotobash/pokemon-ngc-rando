using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class Move
    {
        const byte MoveNameIDOffset = 0x20;
        const byte MoveDescriptionIDOffset = 0x2C;
        const byte MoveEffectTypeOffset = 0x34; // used by AI

        const byte PriorityOffset = 0x00;
        const byte PPOffset = 0x01;
        const byte MoveTypeOffset = 0x02;
        const byte TargetsOffset = 0x03;
        const byte AccuracyOffset = 0x04;
        const byte EffectAccuracyOffset = 0x05;
        const byte Animation2IndexOffset = 0x32;

        const byte ContactFlagOffset = 0x06;
        const byte ProtectFlagOffset = 0x07;
        const byte MagicCoatFlagOffset = 0x08;
        const byte SnatchFlagOffset = 0x09;
        const byte MirrorMoveFlagOffset = 0x0A;
        const byte KingsRockFlagOffset = 0x0B;
        const byte SoundBasedFlagOffset = 0x10;
        const byte HMFlagOffset = 0x12;

        int FirstShadowMoveIndex;
		int LastShadowMoveIndex;
		int FirstMoveOffset;
		int MoveCategoryOffset; // added through hacking in colosseum
		int BasePowerOffset;
		int EffectOffset;
		int AnimationIndexOffset;

		public int MoveIndex { get; }
        ISO iso;
        public Move(int number, ISO iso)
        {
			MoveIndex = number;

            this.iso = iso;

			FirstShadowMoveIndex = iso.Game == Game.XD ? 0x164 : 0x163;
			LastShadowMoveIndex = iso.Game == Game.XD ? 0x176 : 0x163;

			FirstMoveOffset = iso.Game == Game.XD ? 0xA2710 : 0x11E010;
			MoveCategoryOffset = iso.Game == Game.XD ? 0x13 : 0x1F; // added through hacking in colosseum
			BasePowerOffset = iso.Game == Game.XD ? 0x19 : 0x17;
			EffectOffset = iso.Game == Game.XD ? 0x1C : 0x1A;
			AnimationIndexOffset = iso.Game == Game.XD ? 0x1E : 0x1C;
        }

		public int NameID => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + MoveNameIDOffset);
		public UnicodeString Name
        {
            get => iso.CommonRelStringTable.GetStringWithId(NameID);
            set => iso.CommonRelStringTable.ReplaceString(NameID, value);
        }
	
		public int DescriptionID => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + MoveDescriptionIDOffset);	
		public UnicodeString Description
        {
            get => iso.DolStringTable.GetStringWithId(DescriptionID);
            set => iso.DolStringTable.ReplaceString(DescriptionID, value);
        }
		
		public int StartOffset
		{
			get
			{
				var moves = iso.Game == Game.XD ? Constants.XDMoves : Constants.ColMoves;
				return (int)(iso.CommonRel.GetPointer(moves) + (MoveIndex * Constants.SizeOfMoveData));
			}
		}

		public bool ContactMove
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + ContactFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + ContactFlagOffset, value ? (byte)1 : (byte)0);
        }

        public bool ProtectMove
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + ProtectFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + ProtectFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool MagicCoat
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + MagicCoatFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + MagicCoatFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool Snatch
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + SnatchFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + SnatchFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool MirrorMove
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + MirrorMoveFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + MirrorMoveFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool KingsRock
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + KingsRockFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + KingsRockFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool SoundBased
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + SoundBasedFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + SoundBasedFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool HMFlag
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + HMFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + HMFlagOffset, value ? (byte)1 : (byte)0);
        }

        public PokemonTypes Type
        {
            get => (PokemonTypes)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + MoveTypeOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + MoveTypeOffset, (byte)value);
        }

        public MoveTargets Target
        {
            get => (MoveTargets)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + TargetsOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + TargetsOffset, (byte)value);
        }

        public MoveCategories Category
        {
            get => (MoveCategories)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + MoveCategoryOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + MoveCategoryOffset, (byte)value);
        }

        public MoveEffectTypes EffectType
        {
            get => (MoveEffectTypes)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + MoveEffectTypeOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + MoveEffectTypeOffset, (byte)value);
        }

        public ushort Effect
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + EffectOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + EffectOffset, value.GetBytes());
        }
        public byte EffectAccuracy
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + EffectAccuracyOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + EffectAccuracyOffset, value);
        }
        public byte BasePower
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + BasePowerOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + BasePowerOffset, value);
        }
        public byte Accuracy
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + AccuracyOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + AccuracyOffset, value);
        }
        public byte PP
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + PPOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + PPOffset, value);
        }
        public sbyte Priority
        {
            get
            {
                var p = iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + PriorityOffset);
                return p > 128 ? (sbyte)(p - 256) : (sbyte)p;
            }
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + PriorityOffset, (byte)value);
        }

        public ushort AnimationID
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + AnimationIndexOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + AnimationIndexOffset, value.GetBytes());
        }

        public ushort Animation2ID
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Animation2IndexOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Animation2IndexOffset, value.GetBytes());
        }

        public bool IsShadowMove => MoveIndex >= FirstShadowMoveIndex && MoveIndex <= LastShadowMoveIndex;
    }
}
