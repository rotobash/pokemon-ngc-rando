using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public enum PokemonFileType
    {
        DPKM,
        DDPK
    }
    public class XDTrainerPokemon : TrainerPokemon
    {
        const byte SizeOfPokemonData = 0x20;
        const byte SizeOfShadowData = 0x18;
        const byte SizeOfMoveData = 0x2;

        const byte ConstPokemonIndexOffset = 0x00;
        const byte ConstPokemonLevelOffset = 0x02;
        const byte ConstPokemonHappinessOffset = 0x03;
        const byte ConstPokemonItemOffset = 0x04;
        const byte ConstFirstPokemonIVOffset = 0x08;
        const byte ConstFirstPokemonEVOffset = 0x0E;
        const byte ConstFirstPokemonMoveOffset = 0x14;
        const byte ConstPokemonPriority1Offset = 0x1D; // priority? in vanilla but moved by stars in xg
        const byte ConstPokemonPIDOffset = 0x1E;
        const byte ConstPokemonGenRandomOffset = 0x1F; // If value is set to 1 then the pokemon is generated with a random nature and gender. Always 0 in vanilla and unused in XG

        // added by stars by modifying dol behaviour
        const byte ConstPokemonshinynessOffset = 0x1C;
        const byte ConstPokemonPriority2Offset = 0x1F;

        const byte ConstFleeAfterBattleOffset = 0x00; // 0 = no flee. Other values probably chances of finding with mirorb. Higher value = more common encounter.
        const byte ConstShadowCatchRateOffset = 0x01; // this catch rate overrides the species' catch rate
        const byte ConstShadowLevelOffset = 0x02; // the pokemon's level after it's caught. Regular level can be increased so AI shadows are stronger
        const byte ConstShadowInUseFlagOffset = 0x03; // flags for whether pokemon is seen/caught/purified etc. default 0x80 and updated in save file
        const byte ConstShadowStoryIndexOffset = 0x06; // dpkm index of pokemon data in deck story
        const byte ConstShadowCounterOffset = 0x08; // the starting value of the heart gauge
        const byte ConstFirstShadowMoveOffset = 0x0C;
        const byte ConstShadowAggressionOffset = 0x14; // determines how often it enters reverse mode
        const byte ConstShadowAlwaysFleeOffset = 0x15; // the shadow pokemon is sent to miror b. even if you lose the battle

        const byte PurificationExperienceOffset = 0xA; // Should always be 0. The value gets increased as the pokemon gains exp and it is all gained at once upon purification.

        readonly PokemonFileType pokeType;

        public override bool IsShadow => pokeType == PokemonFileType.DDPK;
        public override bool IsSet => Index > 0;

        protected override uint StartOffset => (uint)((pool as XDTrainerPool).DPKMDataOffset + DPKMIndex * SizeOfPokemonData);
        protected override uint ShadowStartOffset => (uint)((pool.DarkPokemon as ShadowTrainerPool).DDPKDataOffset + Index * SizeOfShadowData);
        protected override byte PokemonIndexOffset => ConstPokemonIndexOffset;
        protected override byte PokemonItemOffset => ConstPokemonItemOffset;
        protected override byte PokemonHappinessOffset => ConstPokemonHappinessOffset;
        protected override byte FirstPokemonEVOffset => ConstFirstPokemonEVOffset;
        protected override byte FirstPokemonMoveOffset => ConstFirstPokemonMoveOffset;
        protected override byte ShadowCatchRateOffset => ConstShadowCatchRateOffset;
        protected override byte ShadowCounterOffset => ConstShadowCounterOffset;
        protected override byte PokemonMoveDataSize => SizeOfMoveData;

        public int DPKMIndex => IsShadow switch
        {
            true => pool.DarkPokemon.ExtractedFile.GetUShortAtOffset(ShadowStartOffset + ConstShadowStoryIndexOffset),
            false => Index,
        };

        public override byte Level
        {
            get => pool.ExtractedFile.GetByteAtOffset(StartOffset + ConstPokemonLevelOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + ConstPokemonLevelOffset, value);
        }

        public override byte ShadowLevel
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(ShadowStartOffset + ConstShadowLevelOffset);
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(ShadowStartOffset + ConstShadowLevelOffset, value);
        }

        public byte Ability
        {
            get => (byte)(pool.ExtractedFile.GetByteAtOffset(StartOffset + ConstPokemonPIDOffset) % 2);
            set
            {
                var pid = ((byte)Nature << 3) + ((byte)Gender << 1) + value;
                pool.ExtractedFile.WriteByteAtOffset(StartOffset + ConstPokemonPIDOffset, (byte)pid);
            }
        }
        public Genders Gender
        {
            get => (Genders)(pool.ExtractedFile.GetByteAtOffset(StartOffset + ConstPokemonPIDOffset) / 4 % 2);
            set
            {
                var pid = ((byte)Nature << 3) + ((byte)value << 1) + Ability;
                pool.ExtractedFile.WriteByteAtOffset(StartOffset + ConstPokemonPIDOffset, (byte)pid);
            }
        }
        public Natures Nature
        {
            get => (Natures)(pool.ExtractedFile.GetByteAtOffset(StartOffset + ConstPokemonPIDOffset) / 8);
            set
            {
                var pid = ((byte)value << 3) + ((byte)Gender << 1) + Ability;
                pool.ExtractedFile.WriteByteAtOffset(StartOffset + ConstPokemonPIDOffset, (byte)pid);
            }
        }

        public ushort[] ShadowMoves { get; }

        public XDTrainerPokemon(int index, XDTrainerPool team, PokemonFileType type) : base(index, team)
        {
            pokeType = type;
            for (int i = 0; i < Moves.Length; i++)
            {
                Moves[i] = pool.ExtractedFile.GetUShortAtOffset(StartOffset + FirstPokemonMoveOffset + i * PokemonMoveDataSize);
            }

            if (IsShadow)
            {
                ShadowMoves = new ushort[Constants.NumberOfPokemonMoves];
                for (int i = 0; i < Moves.Length; i++)
                {
                    ShadowMoves[i] = team.DarkPokemon.ExtractedFile.GetUShortAtOffset(StartOffset + ConstFirstShadowMoveOffset + i * 2); 
                }
            }
        }

        public override void SetShadowMove(int index, ushort moveNum)
        {
            pool.DarkPokemon.ExtractedFile.WriteBytesAtOffset(StartOffset + ConstFirstShadowMoveOffset + index * 2, moveNum.GetBytes());
            ShadowMoves[index] = moveNum;
        }

        public bool ShadowDataInUse
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(ShadowStartOffset + ConstShadowInUseFlagOffset) == 0x80;
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(ShadowStartOffset + ConstShadowInUseFlagOffset, value ? (byte)0x80 : (byte)0);
        }
        public byte ShadowFleeValue
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(ShadowStartOffset + ConstFleeAfterBattleOffset);
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(ShadowStartOffset + ConstFleeAfterBattleOffset, value);
        }
        public byte ShadowAgression
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(ShadowStartOffset + ConstShadowAggressionOffset);
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(ShadowStartOffset + ConstShadowAggressionOffset, value);
        }
        public byte ShadowAlwaysFlee
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(ShadowStartOffset + ConstShadowAlwaysFleeOffset);
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(ShadowStartOffset + ConstShadowAlwaysFleeOffset, value);
        }
    }
}
