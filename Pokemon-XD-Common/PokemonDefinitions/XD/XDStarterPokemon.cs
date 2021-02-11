using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class XDStarterPokemon : IGiftPokemon
    {
        const byte SpeciesOffset = 0x2;
        const byte LevelOffset = 0xB;
        const byte Move1Offset = 0x12;
        const byte Move2Offset = 0x16;
        const byte Move3Offset = 0x1A;
        const byte Move4Offset = 0x1E;
        const byte ExpValueOffset = 0x66;

        public byte Index => 0;

        public int StartOffset
        {
            get
            {
                return iso.Region switch
                {
                    Region.US => 0x1CBC50,
                    Region.Europe => 0x1CD724,
                    _ => 0x1C6AF4,
                };
            }
        }

        public byte Level
        {
            get => iso.DOL.ExtractedFile.GetByteAtOffset(StartOffset + LevelOffset);
            set => iso.DOL.ExtractedFile.WriteByteAtOffset(StartOffset + LevelOffset, value);
        }

        public ushort Exp
        {
            get => iso.DOL.ExtractedFile.GetUShortAtOffset(StartOffset + ExpValueOffset);
            set => iso.DOL.ExtractedFile.WriteBytesAtOffset(StartOffset + ExpValueOffset, value.GetBytes());
        }

        public ushort Pokemon
        {
            get => iso.DOL.ExtractedFile.GetUShortAtOffset(StartOffset + SpeciesOffset);
            set => iso.DOL.ExtractedFile.WriteBytesAtOffset(StartOffset + SpeciesOffset, value.GetBytes());
        }

        public ushort[] Moves
        {
            get;
        }

        public string GiftType => "Starter";

        ISO iso;
        public XDStarterPokemon(ISO iso)
        {
            this.iso = iso;
            Moves = new ushort[Constants.NumberOfPokemonMoves];
            for (int i = 0; i < Moves.Length; i++)
            {
                Moves[i] = iso.DOL.ExtractedFile.GetUShortAtOffset(StartOffset + Move1Offset + (i * Constants.SizeOfLevelUpData));
            }
        }

        public void SetMove(int index, ushort move)
        {
            iso.DOL.ExtractedFile.WriteBytesAtOffset(StartOffset + Move1Offset + (index * Constants.SizeOfLevelUpData), move.GetBytes());
        }
    }
}
