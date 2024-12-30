using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class XDPokemon : Pokemon
    {
        const int XDSizeOfPokemonStats = 0x124;

        const byte XDBaseEXPOffset = 0x05;
        const byte XDBaseHappinessOffset = 0x07;
        const byte XDHeightOffset = 0x08;
        const byte XDWeightOffset = 0x0A;

        const byte XDHeldItem1Offset = 0x7A;
        const byte XDHeldItem2Offset = 0x7C;

        const byte XDHPOffset = 0x8F;
        const byte XDAttackOffset = 0x91;
        const byte XDDefenseOffset = 0x93;
        const byte XDSpecialAttackOffset = 0x95;
        const byte XDSpecialDefenseOffset = 0x97;
        const byte XDSpeedOffset = 0x99;

        const byte XDFirstEVYieldOffset = 0x9A; // 1 byte between each one.
        const byte XDFirstEvolutionOffset = 0xA6;
        const byte XDFirstTutorMoveOffset = 0x6E;
        const byte XDFirstLevelUpMoveOffset = 0xC4;

        public override ushort SizeOfPokemonStats => XDSizeOfPokemonStats;
        public override ushort BaseEXPOffset => XDBaseEXPOffset;
        public override ushort BaseHappinessOffset => XDBaseHappinessOffset;
        public override ushort HeightOffset => XDHeightOffset;
        public override ushort WeightOffset => XDWeightOffset;
        public override ushort HeldItem1Offset => XDHeldItem1Offset;
        public override ushort HPOffset => XDHPOffset;
        public override ushort AttackOffset => XDAttackOffset;
        public override ushort DefenseOffset => XDDefenseOffset;
        public override ushort SpecialAttackOffset => XDSpecialAttackOffset;
        public override ushort SpecialDefenseOffset => XDSpecialDefenseOffset;
        public override ushort SpeedOffset => XDSpeedOffset;
        public override ushort FirstEVYieldOffset => XDFirstEVYieldOffset;
        public override ushort FirstTutorMoveOffset => XDFirstTutorMoveOffset;
        public override ushort FirstLevelUpMoveOffset => XDFirstLevelUpMoveOffset;
        public override ushort FirstEvolutionOffset => XDFirstEvolutionOffset;

        public XDPokemon(int dexNum, ISO iso) : base(dexNum, iso)
        {

        }


        public static void FixBonsly(Pokemon bonsly)
        {
            var levelUpMoves = new LevelUpMove[]
            {
                new LevelUpMove(1, 313),
                new LevelUpMove(6, 175),
                new LevelUpMove(9, 67),
                new LevelUpMove(14, 88),
                new LevelUpMove(17, 102),
                new LevelUpMove(22, 335),
                new LevelUpMove(25, 185),
                new LevelUpMove(30, 317),
                new LevelUpMove(33, 157),
                new LevelUpMove(38, 21),
                new LevelUpMove(46, 38),
            };

            for (int i = 0; i < levelUpMoves.Length; i++)
            {
                var levelUpMove = levelUpMoves[i];
                bonsly.SetLevelUpMove(1, levelUpMove.Level, levelUpMove.Move);
            }

            var tmCompatibility = new int[]
            {
                4, 6, 10, 11, 17, 21, 27, 28,
                31, 32, 37, 39, 42, 43, 44, 45, 46
            };

            foreach (int i in tmCompatibility)
            {
                bonsly.SetLearnableTMS(i - 1, true);
            }

            var tutorMoveCompatibility = new int[]
            {
                1, 5, 7, 11, 12
            };

            foreach (int i in tutorMoveCompatibility)
            {
                bonsly.SetTutorMove(i - 1, true);
            }

            bonsly.SetEvolution(0, (byte)EvolutionMethods.LevelUp, 18, 185);
        }
    }
}
