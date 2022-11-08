using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class XDTradePokemon : IGiftPokemon
    {
        static readonly ushort[][] DukingTradeRequestedPokemonIndices = new ushort[][]
        {
            // Trapinch Offsets
            new ushort[] { 0x0B4A, 0x0D1E, 0x0D3A, 0x1756 },
            // Surskit Offsets
            new ushort[] { 0x0B9A, 0x0D62, 0x0D7E, 0x175E },
            // Wooper Offsets
            new ushort[] { 0x0BEA, 0x0DA6, 0x0DC2, 0x1766 }
        };
        static readonly ushort[] DukingTradeGivenPokemonIndices = new ushort[] { 0x0D32, 0x0D76, 0x0DBA };

        const byte TradePokemonSpeciesOffset = 0x02;
        const byte TradePokemonLevelOffset = 0x0B;
        const byte TradePokemonMoveOffset = 0x26;

        public byte Index { get; }
        public string GiftType => Index == 0 ? "Hordel" : "Duking";

        public byte Level
        {
            get => iso.DOL.ExtractedFile.GetByteAtOffset(StartOffset + TradePokemonLevelOffset);
            set => iso.DOL.ExtractedFile.WriteByteAtOffset(StartOffset + TradePokemonLevelOffset, value);
        }

        public ushort Pokemon
        {
            get => iso.DOL.ExtractedFile.GetUShortAtOffset(StartOffset + TradePokemonSpeciesOffset);
            set => iso.DOL.ExtractedFile.WriteBytesAtOffset(StartOffset + TradePokemonSpeciesOffset, value.GetBytes());
        }
        public ushort[] Moves { get; }

        public ushort Exp { get; }
        int StartOffset
        {
            get
            {
                return Index switch
                {
                    0 => ElekidOffset,
                    1 => MedititeOffset,
                    2 => ShuckleOffset,
                    _ => LarvitarOffset
                };
            }
        }

        int ElekidOffset
        {
            get
            {
                return iso.Region switch
                {
                    Region.US => 0x1C57A4,
                    Region.Europe => 0x1C70A0,
                    _ => 0x1C0CB4,
                };
            }
        }

        int MedititeOffset
        {
            get
            {
                return iso.Region switch
                {
                    Region.US => 0x1C5888,
                    Region.Europe => 0x1C7184,
                    _ => 0x1C0D1C,
                };
            }
        }

        int ShuckleOffset
        {
            get
            {
                return iso.Region switch
                {
                    Region.US => 0x1C58D8,
                    Region.Europe => 0x1C71D4,
                    _ => 0x1C0D6C,
                };
            }
        }

        int LarvitarOffset
        {
            get
            {
                return iso.Region switch
                {
                    Region.US => 0x1C5928,
                    Region.Europe => 0x1C7224,
                    _ => 0x1C0DBC,
                };
            }
        }

        ISO iso;
        public XDTradePokemon(byte index, ISO iso)
        {
            Index = index;
            this.iso = iso;
            Moves = new ushort[Constants.NumberOfPokemonMoves];
            for (int i = 0; i < Moves.Length; i++)
            {
                Moves[i] = iso.DOL.ExtractedFile.GetUShortAtOffset(StartOffset + TradePokemonMoveOffset + i * 4);
            }
        }

        public void SetMove(int i, ushort move)
        {
            iso.DOL.ExtractedFile.WriteBytesAtOffset(StartOffset + TradePokemonMoveOffset + i * 4, move.GetBytes());
        }

        public static void UpdateTrades(ISO iso, Pokemon[] newRequestedPokemon, Pokemon[] newGivenPokemon)
        {
            var tradeScript = iso.GetFSysFile("M2_guild_1F_2.fsys").GetEntryByFileName("M2_guild_1F_2.scd");

            if (newGivenPokemon.Length < 3)
            {
                throw new ArgumentException();
            }
            
            if (newRequestedPokemon.Length < 3)
            {
                throw new ArgumentException();
            }


            for (int i = 0; i < DukingTradeGivenPokemonIndices.Length; i++)
            {
                var requestPokemonNameId = newRequestedPokemon[i].NameID.GetBytes();
                foreach (var requestedOffset in DukingTradeRequestedPokemonIndices[i])
                {
                    tradeScript.ExtractedFile.WriteBytesAtOffset(requestedOffset, requestPokemonNameId);
                }

                var tradePoke = newGivenPokemon[i].NameID;
                var givenOffset = DukingTradeGivenPokemonIndices[i];
                tradeScript.ExtractedFile.WriteBytesAtOffset(givenOffset, tradePoke.GetBytes());
            }
        }
    }
}
