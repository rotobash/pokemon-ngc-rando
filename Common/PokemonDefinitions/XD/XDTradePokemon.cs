using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class XDTradePokemon : IGiftPokemon
    {
        static readonly ushort[][] DukingTradeRequestedPokemonIDIndices = new ushort[][]
        {
            // Trapinch ID Offsets
            new ushort[] { 0x0B4A, 0x0D1E },
            // Surskit ID Offsets
            new ushort[] { 0x0B9A, 0x0D62 },
            // Wooper ID Offsets
            new ushort[] { 0x0BEA, 0x0DA6 }
        };

        static readonly ushort[][] DukingTradeRequestedPokemonNameIDIndices = new ushort[][]
        {
            // Trapinch Offsets
            new ushort[] { 0x0D3A, 0x1756 },
            // Surskit Offsets
            new ushort[] { 0x0D7E, 0x175E },
            // Wooper Offsets
            new ushort[] { 0x0DC2, 0x1766 }
        };

        static readonly ushort[][] HordelTradeRequestedPokemonIDIndices = new ushort[][]
        {
            // Togepi Offsets
            new ushort[] { 0x0F26, 0x1352 },
            // Togetic Offsets
            new ushort[] { 0x0F36, 0x1332 },
        };

        static readonly ushort[] DukingTradeGivenPokemonNameIDIndices = new ushort[] { 0x0D32, 0x0D76, 0x0DBA };

        static readonly (ushort, ushort)[] PokemonNameOffsets = new (ushort, ushort)[]
        {
            (0x628, 8),
            (0x63C, 7),
            (0x659, 6)
        };

        const byte TradeMessageCharacterCopyIndex = 42;
        const int TradeMessageStringId = 37236;

        const byte TradePokemonSpeciesOffset = 0x02;
        const byte TradePokemonLevelOffset = 0x0B;
        const byte TradePokemonMoveOffset = 0x26;

        public int Index { get; }
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

        public static void UpdateDukingTrades(ISO iso, Pokemon[] newRequestedPokemon, Pokemon[] newGivenPokemon)
        {
            var fsysFile = iso.GetFSysFile("M2_guild_1F_2.fsys");
            var tradeScript = fsysFile.GetEntryByFileName("M2_guild_1F_2.scd");
            var message = fsysFile.GetEntryByFileName("M2_guild_1F_2.msg") as StringTable;

            if (newRequestedPokemon?.Length > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    var requestedPokemon = newRequestedPokemon[i];
                    var requestPokemonId = ((ushort)requestedPokemon.Index).GetBytes();
                    foreach (var requestedOffset in DukingTradeRequestedPokemonIDIndices[i])
                    {
                        tradeScript.ExtractedFile.WriteBytesAtOffset(requestedOffset, requestPokemonId);
                    }

                    var requestPokemonNameId = ((ushort)requestedPokemon.NameID).GetBytes();
                    foreach (var requestedOffset in DukingTradeRequestedPokemonNameIDIndices[i])
                    {
                        tradeScript.ExtractedFile.WriteBytesAtOffset(requestedOffset, requestPokemonNameId);
                    }
                }

                var endMessageLine1 = UnicodeString.FromString($"{newRequestedPokemon[0].UnicodeName}, {newRequestedPokemon[1].UnicodeName}");
                var lineBreak = new[] { new SpecialUnicodeCharacters(SpecialCharacters.NewLine) };
                var endMessageLine2 = UnicodeString.FromString($"or a {newRequestedPokemon[2].UnicodeName}?");

                var str = new UnicodeString(message.GetStringWithId(TradeMessageStringId).Take(TradeMessageCharacterCopyIndex).Concat(endMessageLine1).Concat(lineBreak).Concat(endMessageLine2));

                message.ReplaceString(TradeMessageStringId, str);
            }

            if (newGivenPokemon?.Length > 0)
            {
                for (int i = 0; i < DukingTradeGivenPokemonNameIDIndices.Length; i++)
                {
                    var tradePoke = newGivenPokemon[i].NameID;
                    var givenOffset = DukingTradeGivenPokemonNameIDIndices[i];
                    tradeScript.ExtractedFile.WriteBytesAtOffset(givenOffset, tradePoke.GetBytes());
                }
            }
        }

        public static void UpdateHordelTrade(ISO iso, Pokemon shadowGivenPokemon, Pokemon tradePokemon)
        {
            if (shadowGivenPokemon == null && tradePokemon == null) return;

            var fsysFile = iso.GetFSysFile("S1_shop_1F.fsys");
            var tradeScript = fsysFile.GetEntryByFileName("S1_shop_1F.scd");
            var message = fsysFile.GetEntryByFileName("S1_shop_1F.msg") as StringTable;

            var togepiStr = UnicodeString.FromString("TOGEPI");
            var elekidStr = UnicodeString.FromString("ELEKID");

            if (shadowGivenPokemon != null)
            {
                foreach (var requestedPokemonIdOffset in HordelTradeRequestedPokemonIDIndices)
                {
                    foreach (var offset in requestedPokemonIdOffset)
                    {
                        tradeScript.ExtractedFile.WriteBytesAtOffset(offset, ((ushort)shadowGivenPokemon.Index).GetBytes());
                    }
                }
            }

            foreach (var messageId in message.StringIds)
            {
                var str = message.GetStringWithId(messageId);
                if (shadowGivenPokemon != null && str.Contains(togepiStr))
                {
                    message.ReplaceString(messageId, str.Replace(togepiStr, shadowGivenPokemon.UnicodeName));
                }

                if (tradePokemon != null && str.Contains(elekidStr))
                {
                    message.ReplaceString(messageId, str.Replace(elekidStr, tradePokemon.UnicodeName));
                }
            }
        }
    }
}
