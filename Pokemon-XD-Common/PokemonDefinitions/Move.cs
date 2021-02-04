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
        StringTable dolTbl;
        StringTable commonRelStrTbl;
        REL commonRel;
        ISO iso;
		Game game;
        public Move(int number, ISO iso)
        {
			MoveIndex = number;

            this.iso = iso;
			commonRel = iso.CommonRel();
			commonRelStrTbl = iso.CommonRelStringTable();
			dolTbl = iso.DolStringTable();
			game = iso.Game;

			FirstShadowMoveIndex = iso.Game == Game.XD ? 0x164 : 0x163;
			LastShadowMoveIndex = iso.Game == Game.XD ? 0x176 : 0x163;

			FirstMoveOffset = iso.Game == Game.XD ? 0xA2710 : 0x11E010;
			MoveCategoryOffset = iso.Game == Game.XD ? 0x13 : 0x1F; // added through hacking in colosseum
			BasePowerOffset = iso.Game == Game.XD ? 0x19 : 0x17;
			EffectOffset = iso.Game == Game.XD ? 0x1C : 0x1A;
			AnimationIndexOffset = iso.Game == Game.XD ? 0x1E : 0x1C;
        }

		public int NameID => commonRel.ExtractedFile.GetIntAtOffset(StartOffset + Constants.MoveNameIDOffset);
		public UnicodeString Name => commonRelStrTbl.GetStringWithId(NameID);
	
		public int DescriptionID => commonRel.ExtractedFile.GetIntAtOffset(StartOffset + Constants.MoveDescriptionIDOffset);	
		public UnicodeString MDescription => dolTbl.GetStringWithId(DescriptionID);
		
		public int StartOffset
		{
			get
			{
				var moves = game == Game.XD ? Constants.XDMoves : Constants.ColMoves;
				return (int)(commonRel.GetPointer(moves) + (MoveIndex * Constants.SizeOfMoveData));
			}
		}

		public bool ContactMove
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.ContactFlagOffset) == 1;
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.ContactFlagOffset, value ? (byte)1 : (byte)0);
        }

        public bool ProtectMove
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.ProtectFlagOffset) == 1;
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.ProtectFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool MagicCoat
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.MagicCoatFlagOffset) == 1;
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.MagicCoatFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool Snatch
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.SnatchFlagOffset) == 1;
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.SnatchFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool MirrorMove
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.MirrorMoveFlagOffset) == 1;
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.MirrorMoveFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool KingsRock
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.KingsRockFlagOffset) == 1;
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.KingsRockFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool SoundBased
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.SoundBasedFlagOffset) == 1;
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.SoundBasedFlagOffset, value ? (byte)1 : (byte)0);
        }
        public bool HMFlag
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.HMFlagOffset) == 1;
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.HMFlagOffset, value ? (byte)1 : (byte)0);
        }

        public PokemonTypes Type
        {
            get => (PokemonTypes)commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.MoveTypeOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.MoveTypeOffset, (byte)value);
        }

        public MoveTargets Target
        {
            get => (MoveTargets)commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.TargetsOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.TargetsOffset, (byte)value);
        }

        public MoveCategories Category
        {
            get => (MoveCategories)commonRel.ExtractedFile.GetByteAtOffset(StartOffset + MoveCategoryOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + MoveCategoryOffset, (byte)value);
        }

        public MoveEffectTypes EffectType
        {
            get => (MoveEffectTypes)commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.MoveEffectTypeOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.MoveEffectTypeOffset, (byte)value);
        }

        public ushort Effect
        {
            get => commonRel.ExtractedFile.GetUShortAtOffset(StartOffset + EffectOffset);
            set => commonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + EffectOffset, value.GetBytes());
        }
        public byte EffectAccuracy
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.EffectAccuracyOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.EffectAccuracyOffset, value);
        }
        public byte BasePower
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + BasePowerOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + BasePowerOffset, value);
        }
        public byte Accuracy
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.AccuracyOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.AccuracyOffset, value);
        }
        public byte PP
        {
            get => commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.PPOffset);
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.PPOffset, value);
        }
        public sbyte Priority
        {
            get
            {
                var p = commonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.PriorityOffset);
                return p > 128 ? (sbyte)(p - 256) : (sbyte)p;
            }
            set => commonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.PriorityOffset, (byte)value);
        }

        public ushort AnimationID
        {
            get => commonRel.ExtractedFile.GetUShortAtOffset(StartOffset + AnimationIndexOffset);
            set => commonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + AnimationIndexOffset, value.GetBytes());
        }

        public ushort Animation2ID
        {
            get => commonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.Animation2IndexOffset);
            set => commonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.Animation2IndexOffset, value.GetBytes());
        }

        public bool IsShadowMove => MoveIndex >= FirstShadowMoveIndex && MoveIndex <= LastShadowMoveIndex;
    }
}
