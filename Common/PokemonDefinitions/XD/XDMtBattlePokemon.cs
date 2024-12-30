using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class XDMtBattlePokemon : IGiftPokemon
    {
        const byte MtBattlePokemonSpeciesOffset = 0x02;
        const byte MtBattlePokemonMoveOffset = 0x06;
        public int Index { get; set; }

        public ushort Exp { get; set; }

        public ushort[] Moves { get; }
        public string GiftType => "Mt Battle";

        public byte Level 
        {
            get => iso.DOL.ExtractedFile.GetByteAtOffset(StartOffset + LevelOffset);
            set => iso.DOL.ExtractedFile.WriteByteAtOffset(StartOffset + LevelOffset, value);
        }
        public ushort Pokemon 
        {
            get => iso.DOL.ExtractedFile.GetUShortAtOffset(StartOffset + MtBattlePokemonSpeciesOffset);
            set => iso.DOL.ExtractedFile.WriteBytesAtOffset(StartOffset + MtBattlePokemonSpeciesOffset, value.GetBytes());
        }

        public int StartOffset 
        {
            get
            {
                return Index switch
                {
                    0 => ChikoritaOffset,
                    1 => CyndaquilOffset,
                    _ => TotodileOffset
                };
            }
        }

        int ChikoritaOffset
        {
            get
            {
                return iso.Region switch
                {
                    Region.US => 0x1C5974,
                    Region.Europe => 0x1C7270,
                    _ => 0x1C0E08,
                };
            }
        }
        int CyndaquilOffset
        {
            get
            {
                return iso.Region switch
                {
                    Region.US => 0x1C59A0,
                    Region.Europe => 0x1C729C,
                    _ => 0x1C0E34,
                };
            }
        }

        int TotodileOffset
        {
            get
            {
                return iso.Region switch
                {
                    Region.US => 0x1C59CC,
                    Region.Europe => 0x1C72C8,
                    _ => 0x1C0E60,
                };
            }
        }

        int LevelOffset
        {
            get
            {
                return iso.Region switch
                {
                    Region.US => 0x1C56E8,
                    Region.Europe => 0x1C6FE4,
                    _ => 0x1C0BF8,
                };
            }
        }

        ISO iso;
        public XDMtBattlePokemon(byte index, ISO iso)
        {
            Index = index;
            this.iso = iso;

            Moves = new ushort[Constants.NumberOfPokemonMoves];
            for (int i = 0; i < Moves.Length; i++)
            {
                iso.DOL.ExtractedFile.GetUShortAtOffset(StartOffset + MtBattlePokemonMoveOffset + i * 4);
            }
        }

        public void SetMove(int i, ushort move)
        {
            iso.DOL.ExtractedFile.WriteBytesAtOffset(StartOffset + MtBattlePokemonMoveOffset + i * 4, move.GetBytes());
        }
    }
}
