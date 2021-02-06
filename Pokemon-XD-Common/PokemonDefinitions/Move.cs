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

		public int NameID => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + Constants.MoveNameIDOffset);
		public string Name => iso.CommonRelStringTable.GetStringWithId(NameID).ToString();
	
		public int DescriptionID => iso.CommonRel.ExtractedFile.GetIntAtOffset(StartOffset + Constants.MoveDescriptionIDOffset);	
		public string MDescription => iso.DolStringTable.GetStringWithId(DescriptionID).ToString();
		
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
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.ContactFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.ContactFlagOffset, value ? (byte)1 : (byte)0);
        }

        public bool ProtectMove
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.ProtectFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.ProtectFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool MagicCoat
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.MagicCoatFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.MagicCoatFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool Snatch
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.SnatchFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.SnatchFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool MirrorMove
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.MirrorMoveFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.MirrorMoveFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool KingsRock
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.KingsRockFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.KingsRockFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool SoundBased
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.SoundBasedFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.SoundBasedFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool HMFlag
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.HMFlagOffset) == 1;
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.HMFlagOffset, value ? (byte)1 : (byte)0);
        }

        public PokemonTypes Type
        {
            get => (PokemonTypes)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.MoveTypeOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.MoveTypeOffset, (byte)value);
        }

        public MoveTargets Target
        {
            get => (MoveTargets)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.TargetsOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.TargetsOffset, (byte)value);
        }

        public MoveCategories Category
        {
            get => (MoveCategories)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + MoveCategoryOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + MoveCategoryOffset, (byte)value);
        }

        public MoveEffectTypes EffectType
        {
            get => (MoveEffectTypes)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.MoveEffectTypeOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.MoveEffectTypeOffset, (byte)value);
        }

        public ushort Effect
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + EffectOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + EffectOffset, value.GetBytes());
        }
        public byte EffectAccuracy
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.EffectAccuracyOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.EffectAccuracyOffset, value);
        }
        public byte BasePower
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + BasePowerOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + BasePowerOffset, value);
        }
        public byte Accuracy
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.AccuracyOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.AccuracyOffset, value);
        }
        public byte PP
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.PPOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.PPOffset, value);
        }
        public sbyte Priority
        {
            get
            {
                var p = iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.PriorityOffset);
                return p > 128 ? (sbyte)(p - 256) : (sbyte)p;
            }
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.PriorityOffset, (byte)value);
        }

        public ushort AnimationID
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + AnimationIndexOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + AnimationIndexOffset, value.GetBytes());
        }

        public ushort Animation2ID
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.Animation2IndexOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.Animation2IndexOffset, value.GetBytes());
        }

        public bool IsShadowMove => MoveIndex >= FirstShadowMoveIndex && MoveIndex <= LastShadowMoveIndex;
    }
}
