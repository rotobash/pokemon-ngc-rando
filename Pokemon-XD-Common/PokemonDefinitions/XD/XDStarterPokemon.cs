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
            get => iso.DOL.ExtractedFile.GetByteAtOffset(StartOffset + ExpValueOffset);
            set => iso.DOL.ExtractedFile.WriteBytesAtOffset(StartOffset + ExpValueOffset, value.GetBytes());
        }

        public ushort Pokemon
        {
            get => iso.DOL.ExtractedFile.GetByteAtOffset(StartOffset + SpeciesOffset);
            set => iso.DOL.ExtractedFile.WriteBytesAtOffset(StartOffset + SpeciesOffset, value.GetBytes());
        }

        public ushort Move1
        {
            get => iso.DOL.ExtractedFile.GetByteAtOffset(StartOffset + Move1Offset);
            set => iso.DOL.ExtractedFile.WriteBytesAtOffset(StartOffset + Move1Offset, value.GetBytes());
        }

        public ushort Move2
        {
            get => iso.DOL.ExtractedFile.GetByteAtOffset(StartOffset + Move2Offset);
            set => iso.DOL.ExtractedFile.WriteBytesAtOffset(StartOffset + Move2Offset, value.GetBytes());
        }

        public ushort Move3
        {
            get => iso.DOL.ExtractedFile.GetByteAtOffset(StartOffset + Move3Offset);
            set => iso.DOL.ExtractedFile.WriteBytesAtOffset(StartOffset + Move3Offset, value.GetBytes());
        }

        public ushort Move4
        {
            get => iso.DOL.ExtractedFile.GetByteAtOffset(StartOffset + Move4Offset);
            set => iso.DOL.ExtractedFile.WriteBytesAtOffset(StartOffset + Move4Offset, value.GetBytes());
        }

        public string GiftType => "Starter";

        public bool UseLevelUpMoves
        {
            get;
            set;
        }

        ISO iso;
        public XDStarterPokemon(ISO iso)
        {
            this.iso = iso;
        }
    }
}
