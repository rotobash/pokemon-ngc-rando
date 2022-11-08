using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class ColPokemon : Pokemon
    {
        const ushort NumberOfPokemon = 0x19D;
        const ushort ColSizeOfPokemonStats = 0x11C;

        const byte ColBaseEXPOffset = 0x07;
        const byte ColBaseHappinessOffset = 0x09;
        const byte ColHeightOffset = 0x0A;
        const byte ColWeightOffset = 0x0C;

        const byte ColHeldItem1Offset = 0x70;
        const byte ColHeldItem2Offset = 0x72;

        const byte ColHPOffset = 0x85;
        const byte ColAttackOffset = 0x87;
        const byte ColDefenseOffset = 0x89;
        const byte ColSpecialAttackOffset = 0x8B;
        const byte ColSpecialDefenseOffset = 0x8D;
        const byte ColSpeedOffset = 0x8F;

        const byte ColFirstEVYieldOffset = 0x90; // 1 byte between each one.
        const byte ColFirstEvolutionOffset = 0x9C;
        const byte ColFirstLevelUpMoveOffset = 0xBA;

        const byte PokemonCryIndexOffset = 0x0E;

        public override ushort SizeOfPokemonStats => ColSizeOfPokemonStats;
        public override ushort BaseEXPOffset => ColBaseEXPOffset;
        public override ushort BaseHappinessOffset => ColBaseHappinessOffset;
        public override ushort HeightOffset => ColHeightOffset;
        public override ushort WeightOffset => ColWeightOffset;
        public override ushort HeldItem1Offset => ColHeldItem1Offset;
        public override ushort HPOffset => ColHPOffset;
        public override ushort AttackOffset => ColAttackOffset;
        public override ushort DefenseOffset => ColDefenseOffset;
        public override ushort SpecialAttackOffset => ColSpecialAttackOffset;
        public override ushort SpecialDefenseOffset => ColSpecialDefenseOffset;
        public override ushort SpeedOffset => ColSpeedOffset;
        public override ushort FirstEVYieldOffset => ColFirstEVYieldOffset;
        public override ushort FirstEvolutionOffset => ColFirstEvolutionOffset;
        public override ushort FirstLevelUpMoveOffset => ColFirstLevelUpMoveOffset;
        public override ushort FirstTutorMoveOffset => 0;

        public ColPokemon(int dexNum, ISO iso) : base(dexNum, iso)
        {

        }
    }
}
