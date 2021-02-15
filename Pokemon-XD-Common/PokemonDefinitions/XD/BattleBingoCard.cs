using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class BattleBingoCard
    {
        const byte BattleBingo = 0x0;
        const byte NumberOfBingoCouponRewards = 0x0A;
        const byte NumberOfPanels = 0x10;
        const byte SizeOfBingoCardData = 0xB8;

        const byte NumberOfMysteryPanels = 0x03;
        const byte NumberOfPokemonPanels = 0x0D;
        const byte BingoCardIndexOffset = 0x00;
        const byte BingoCardDifficultyLevelOffset = 0x01;
        const byte BingoCardSubIndexOffset = 0x02;
        const byte BingoCardPokemonLevelOffset = 0x03;
        const byte BingoCardPokemonCountOffset = 0x06;
        const byte BingoCardMysteryPanelCountOffset = 0x07;
        const byte BingoCardNameIDOffset = 0x08;
        const byte BingoCardDetailsIDOffset = 0x0C;
        const byte BingoCardFirstCouponsRewardOffset = 0x10;
        const byte BingoCardFirstPokemonOffset = 0x24;
        const byte BingoCardFirstMysteryPanelOffset = 0xB0;
        const byte SizeOfBattleBingoPokemonData = 0x0A;

        public int StartOffset => (int)iso.CommonRel.GetPointer(BattleBingo) + (index * SizeOfBingoCardData);
        
        int index;
        ISO iso;
        public BattleBingoCard(int index, ISO iso)
        {
            this.index = index;
            this.iso = iso;

            StartingPokemon = new BattleBingoPokemon(StartOffset + BingoCardFirstPokemonOffset, iso);
            CouponRewards = new int[NumberOfBingoCouponRewards];
            var panels = new List<BattleBingoPanel>();


            for (int i = 1; i < NumberOfPokemonPanels; i++)
            {
                var pokemon = new BattleBingoPokemon(StartOffset + BingoCardFirstPokemonOffset + (i * SizeOfBattleBingoPokemonData), iso);
                panels.Add(new BattleBingoPanel(pokemon, iso));
            }

            var mysteryOffset = StartOffset + BingoCardFirstMysteryPanelOffset;
            for (int i = 0; i < NumberOfMysteryPanels; i++)
            {
                var itemType = (BattleBingoItemType)iso.CommonRel.ExtractedFile.GetByteAtOffset(mysteryOffset++);
                var panelPosition = iso.CommonRel.ExtractedFile.GetByteAtOffset(mysteryOffset++);
                var panel = new BattleBingoPanel(itemType, iso);
                if (panelPosition < panels.Count)
                    panels.Insert(panelPosition, panel);
                else
                    panels.Add(panel);
            }
            Panels = panels.ToArray();

            for (int i = 0; i < NumberOfBingoCouponRewards; i++)
            {
                CouponRewards[i] = iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + BingoCardFirstCouponsRewardOffset + i * 2);
            }
        }

        public byte Difficulty
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + BingoCardDifficultyLevelOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + BingoCardDifficultyLevelOffset, value);
        }

        public byte SubIndex
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + BingoCardSubIndexOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + BingoCardSubIndexOffset, value);
        }

        public byte PokemonLevel
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + BingoCardPokemonLevelOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + BingoCardPokemonLevelOffset, value);
        }

        public BattleBingoPokemon StartingPokemon { get; }
        public BattleBingoPanel[] Panels { get; }
        public int[] CouponRewards { get; }
    }
}
