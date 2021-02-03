using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class Moves
	{
		public int FirstShadowMoveIndex;
		public int LastShadowMoveIndex;
		public int FirstMoveOffset;
		public int MoveCategoryOffset; // added through hacking in colosseum
		public int BasePowerOffset;
		public int EffectOffset;
		public int AnimationIndexOffset;

		public int MoveIndex { get; }
        StringTable dolTbl;
        StringTable commonRelStrTbl;
        REL commonRel;
		Game game;
        public Moves(int number, ISO iso)
        {
			MoveIndex = number;

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

		public int NameID => commonRelStrTbl.ExtractedFile.GetIntAtOffset(StartOffset + Constants.MoveNameIDOffset);
		public UnicodeString Name => commonRelStrTbl.GetStringWithId(NameID);
	
		public int DescriptionID => commonRelStrTbl.ExtractedFile.GetIntAtOffset(StartOffset + Constants.MoveDescriptionIDOffset);	
		public UnicodeString MDescription => dolTbl.GetStringWithId(DescriptionID);

		public int StartOffset
		{
			get
			{
				var moves = game == Game.XD ? Constants.XDMoves : Constants.ColMoves;
				return (int)(commonRel.GetPointer(moves) + (MoveIndex * Constants.SizeOfMoveData));
			}
		}

		public PokemonTypes Type
		{
			get
			{
				return (PokemonTypes)commonRelStrTbl.ExtractedFile.GetByteAtOffset(StartOffset + Constants.MoveTypeOffset);
			}
			set
            {
				commonRel.ExtractedFile.Seek(StartOffset + Constants.MoveTypeOffset, SeekOrigin.Begin);
				commonRel.ExtractedFile.WriteByte((byte)value);
			}
		}

        public MoveCategories Category
		{
			get
			{
				return (MoveCategories)commonRelStrTbl.ExtractedFile.GetByteAtOffset(StartOffset + MoveCategoryOffset);
			}
			set
			{
				commonRel.ExtractedFile.Seek(StartOffset + MoveCategoryOffset, SeekOrigin.Begin);
				commonRel.ExtractedFile.WriteByte((byte)value);
			}
		}

		public bool IsShadowMove
        {
			get
            {
				return MoveIndex >= FirstShadowMoveIndex && MoveIndex <= LastShadowMoveIndex;
            }
        }
    }
}
