using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class BattleBingoCard
    {
        public int StartOffset => (int)iso.CommonRel.GetPointer(Constants.BattleBingo) + (index * Constants.SizeOfBingoCardData);
        
        int index;
        ISO iso;
        public BattleBingoCard(int index, ISO iso)
        {
            this.index = index;
            this.iso = iso;

            StartingPokemon = new BattleBingoPokemon(StartOffset + Constants.BingoCardFirstPokemonOffset, iso);
            CouponRewards = new int[Constants.NumberOfBingoCouponRewards];
            var panels = new List<BattleBingoPanel>();


            for (int i = 1; i < Constants.NumberOfPokemonPanels; i++)
            {
                var pokemon = new BattleBingoPokemon(StartOffset + Constants.BingoCardFirstPokemonOffset + (i * Constants.SizeOfBattleBingoPokemonData), iso);
                panels.Add(new BattleBingoPanel(pokemon, iso));
            }

            var mysteryOffset = StartOffset + Constants.BingoCardFirstMysteryPanelOffset;
            for (int i = 0; i < Constants.NumberOfMysteryPanels; i++)
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

            for (int i = 0; i < Constants.NumberOfBingoCouponRewards; i++)
            {
                CouponRewards[i] = iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.BingoCardFirstCouponsRewardOffset + i * 2);
            }
        }

        public byte Difficulty
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.BingoCardDifficultyLevelOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.BingoCardDifficultyLevelOffset, value);
        }

        public byte SubIndex
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.BingoCardSubIndexOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.BingoCardSubIndexOffset, value);
        }

        public byte PokemonLevel
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.BingoCardPokemonLevelOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.BingoCardPokemonLevelOffset, value);
        }

        public BattleBingoPokemon StartingPokemon { get; }
        public BattleBingoPanel[] Panels { get; }
        public int[] CouponRewards { get; }
    }
}
