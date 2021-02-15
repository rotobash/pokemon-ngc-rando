using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class XDShadowGiftPokemon : IGiftPokemon
    {
        const byte TradeShadowPokemonSpeciesOffset = 0x02;
        const byte TradeShadowDDPKIDOffset = 0x06;
        const byte TradeShadowPokemonLevelOffset = 0x0B;
        const byte TradeShadowPokemonMoveOffset = 0x0E;

        public ushort Pokemon
        {
            get => iso.DOL.ExtractedFile.GetUShortAtOffset(TogepiOffset + TradeShadowPokemonSpeciesOffset);
            set => iso.DOL.ExtractedFile.WriteBytesAtOffset(TogepiOffset + TradeShadowPokemonSpeciesOffset, value.GetBytes());
        }
        public ushort ShadowId
        {
            get => iso.DOL.ExtractedFile.GetUShortAtOffset(TogepiOffset + TradeShadowDDPKIDOffset);
            set => iso.DOL.ExtractedFile.WriteBytesAtOffset(TogepiOffset + TradeShadowDDPKIDOffset, value.GetBytes());
        }
        public byte Level
        {
            get => iso.DOL.ExtractedFile.GetByteAtOffset(TogepiOffset + TradeShadowPokemonLevelOffset);
            set => iso.DOL.ExtractedFile.WriteByteAtOffset(TogepiOffset + TradeShadowPokemonLevelOffset, value);
        }

        public ushort[] Moves { get; }

        public string GiftType => "Shadow Pokemon";

        public byte Index { get; }
        public ushort Exp { get; }

        int TogepiOffset
        {
            get 
            {
                return iso.Region switch
                {
                    Region.US => 0x1C5760,
                    Region.Europe => 0x1C705C,
                    _ => 0x1C0C70
                };
            }
        }

        ISO iso;
        public XDShadowGiftPokemon(ISO iso)
        {
            this.iso = iso;

            Moves = new ushort[Constants.NumberOfPokemonMoves];
            for (int i = 0; i < Moves.Length; i++)
            {
                Moves[i] = iso.DOL.ExtractedFile.GetUShortAtOffset(TogepiOffset + TradeShadowPokemonMoveOffset + i * 4);
            }
        }

        public void SetMove(int i, ushort move)
        {
            iso.DOL.ExtractedFile.WriteBytesAtOffset(TogepiOffset + TradeShadowPokemonMoveOffset + i * 4, move.GetBytes());
        }
    }
}
