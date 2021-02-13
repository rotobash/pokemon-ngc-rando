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
    public class XDTrainerPokemon : ITrainerPokemon
    {
        PokemonFileType pokeType;
        XDTrainerPool pool;

        public bool IsShadow => pokeType == PokemonFileType.DDPK;
        public bool IsSet => Index > 0;

        public int StartOffset
        {
            get => pool.DPKMDataOffset + DPKMIndex * Constants.SizeOfPokemonData;
        }
        
        public int ShadowStartOffset
        {
            get => pool.DarkPokemon.DDPKDataOffset + Index * Constants.SizeOfShadowData;
        }

        public int Index
        {
            get;
            set;
        }

        public int DPKMIndex
        {
            get => IsShadow switch
            {
                true => pool.DarkPokemon.ExtractedFile.GetUShortAtOffset(ShadowStartOffset + Constants.ShadowStoryIndexOffset),
                false => Index,
            };
        }

        public ushort Pokemon
        {
            get => pool.ExtractedFile.GetUShortAtOffset((pool.DPKMDataOffset + DPKMIndex * Constants.SizeOfPokemonData) + Constants.PokemonIndexOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset((pool.DPKMDataOffset + DPKMIndex * Constants.SizeOfPokemonData) + Constants.PokemonIndexOffset, value.GetBytes());
        }

        public byte Level
        {
            get => IsShadow switch
            {
                true => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(ShadowStartOffset + Constants.ShadowLevelOffset),
                false => pool.ExtractedFile.GetByteAtOffset(StartOffset + Constants.PokemonLevelOffset)
            };
            set
            {
                if (IsShadow)
                    pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(ShadowStartOffset + Constants.ShadowLevelOffset, value);
                else
                    pool.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.PokemonLevelOffset, value);
            }
        }

        public byte Happiness
        {
            get => pool.ExtractedFile.GetByteAtOffset(StartOffset + Constants.PokemonHappinessOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.PokemonHappinessOffset, value);
        }
        public ushort Item
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.PokemonItemOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.PokemonItemOffset, value.GetBytes());
        }

        public byte Ability
        {
            get => (byte)(pool.ExtractedFile.GetByteAtOffset(StartOffset + Constants.PokemonPIDOffset) % 2);
            set
            {
                var pid = ((byte)Nature << 3) + ((byte)Gender << 1) + value;
                pool.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.PokemonPIDOffset, (byte)pid);
            }
        }
        public Genders Gender
        {
            get => (Genders)(pool.ExtractedFile.GetByteAtOffset(StartOffset + Constants.PokemonPIDOffset) / 4 % 2);
            set
            {
                var pid = ((byte)Nature << 3) + ((byte)value << 1) + Ability;
                pool.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.PokemonPIDOffset, (byte)pid);
            }
        }
        public Natures Nature
        {
            get => (Natures)(pool.ExtractedFile.GetByteAtOffset(StartOffset + Constants.PokemonPIDOffset) / 8);
            set
            {
                var pid = ((byte)value << 3) + ((byte)Gender << 1) + Ability;
                pool.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.PokemonPIDOffset, (byte)pid);
            }
        }

        public byte[] EVs
        {
            get => pool.ExtractedFile.GetBytesAtOffset(StartOffset + Constants.FirstPokemonEVOffset, Constants.NumberOfIVs);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.PokemonLevelOffset, value);
        }

        public Move[] Moves
        {
            get;
        }
        public Move[] ShadowMoves
        {
            get;
        }

        public XDTrainerPokemon(int index, XDTrainerPool team, PokemonFileType type)
        {
            pool = team;
            pokeType = type;
            Index = index;

            Moves = new Move[4];
            for (int i = 0; i < Moves.Length; i++)
            {
                Moves[i] = pool.MoveList[pool.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstPokemonMoveOffset + i * 2)];
            }

            if (IsShadow)
            {
                ShadowMoves = new Move[4];
                for (int i = 0; i < Moves.Length; i++)
                {
                    var moveIndex = pool.DarkPokemon.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstShadowMoveOFfset + i * 2);
                    ShadowMoves[i] = pool.MoveList[moveIndex];
                }
            }
        }

        public void SetMove(int index, ushort moveNum)
        {
            pool.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstPokemonMoveOffset + index * 2, moveNum.GetBytes());
            Moves[index] = pool.MoveList[moveNum];
        }
        public void SetShadowMove(int index, ushort moveNum)
        {
            pool.DarkPokemon.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstShadowMoveOFfset + index * 2, moveNum.GetBytes());
            ShadowMoves[index] = pool.MoveList[moveNum];
        }

        public byte ShadowCatchRate
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(ShadowStartOffset + Constants.ShadowCatchRateOFfset);
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(ShadowStartOffset + Constants.ShadowCatchRateOFfset, value);
        }
        public ushort ShadowCounter
        {
            get => pool.DarkPokemon.ExtractedFile.GetUShortAtOffset(ShadowStartOffset + Constants.ShadowCounterOffset);
            set => pool.DarkPokemon.ExtractedFile.WriteBytesAtOffset(ShadowStartOffset + Constants.ShadowCounterOffset, value.GetBytes());
        }
        public bool ShadowDataInUse
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(ShadowStartOffset + Constants.ShadowInUseFlagOffset) == 0x80;
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(ShadowStartOffset + Constants.ShadowInUseFlagOffset, value ? (byte)0x80 : (byte)0);
        }
        public byte ShadowFleeValue
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(ShadowStartOffset + Constants.FleeAfterBattleOffset);
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(ShadowStartOffset + Constants.FleeAfterBattleOffset, value);
        }
        public byte ShadowAgression
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(ShadowStartOffset + Constants.ShadowAggressionOffset);
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(ShadowStartOffset + Constants.ShadowAggressionOffset, value);
        }
        public byte ShadowAlwaysFlee
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(ShadowStartOffset + Constants.ShadowAlwaysFleeOffset);
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(ShadowStartOffset + Constants.ShadowAlwaysFleeOffset, value);
        }
    }
}
