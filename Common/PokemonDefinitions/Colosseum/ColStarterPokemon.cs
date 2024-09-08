using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class ColStarterPokemon : IGiftPokemon
    {
        private ISO iso;
        const byte SpeciesOffset = 0x2;
        const byte LevelOffset = 0x7;
        const byte Move1Offset = 0x16;
        const byte Move2Offset = 0x26;
        const byte Move3Offset = 0x36;
        const byte Move4Offset = 0x46;

        public int Index => 0;

        public int StartOffset
        {
            get
            {
                return iso.Region switch
                {
                    Region.US => firstStarter ? 0x12DBF0 : 0x12DAC8,
                    Region.Europe => firstStarter ? 0x131E1C : 0x131CF4,
                    _ => firstStarter ? 0x12B2C0 : 0x12B198,
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
            get;
            set;
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
        bool firstStarter;

        public ColStarterPokemon(ISO iso, bool isFirstStarter)
        {
            this.iso = iso;
            firstStarter = isFirstStarter;
            Moves = new ushort[Constants.NumberOfPokemonMoves];

            for (int i = 0; i < Moves.Length; i++)
            {
                Moves[i] = iso.DOL.ExtractedFile.GetUShortAtOffset(StartOffset + Move1Offset + (i * Constants.ColSizeOfStarterMoveData));
            }
        }

        public void SetMove(int index, ushort move)
        {
            iso.DOL.ExtractedFile.WriteBytesAtOffset(StartOffset + Move1Offset + (index * Constants.ColSizeOfStarterMoveData), move.GetBytes());
        }
    }
}
