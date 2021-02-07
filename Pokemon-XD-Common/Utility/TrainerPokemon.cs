using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.PokemonDefinitions;

namespace XDCommon.Utility
{
    public enum PokemonFileType
    {
        DPKM,
        DDPK
    }
    public class TrainerPokemon
    {
        PokemonFileType pokeType;
        TrainerPool pool;

        public bool IsShadow => pokeType == PokemonFileType.DDPK;
        public bool IsSet => Index > 0;

        public int StartOffset
        {
            get => pool.DPKMDataOffset + (Index * Constants.SizeOfPokemonData);
        }
        public int ShadowStartOffset
        {
            get => pool.DarkPokemon.DDPKHeaderOffset + (Index * Constants.SizeOfShadowData);
        }

        public int Index
        {
            get;
            set;
        }

        public int DPKMIndex
        {
            get => IsShadow ? pool.DarkPokemon.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.ShadowStoryIndexOffset) : Index;
        }

        Pokemon pokemon;
        public Pokemon Pokemon
        {
            get => pokemon;
        }

        public byte Level
        {
            get => pool.ExtractedFile.GetByteAtOffset(StartOffset + Constants.PokemonLevelOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.PokemonLevelOffset, value);
        }

        public byte Happiness
        {
            get => pool.ExtractedFile.GetByteAtOffset(StartOffset + Constants.PokemonHappinessOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.PokemonHappinessOffset, value);
        }
        public ushort Item
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.ShadowCounterOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.ShadowCounterOffset, value.GetBytes());
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
            get => (Genders)((pool.ExtractedFile.GetByteAtOffset(StartOffset + Constants.PokemonPIDOffset) / 4) % 2);
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

        public TrainerPokemon(int index, TrainerPool team, PokemonFileType type)
        {
            pool = team;
            pokeType = type;
            Index = index;
            var num = pool.DPKMDataOffset + (index * Constants.SizeOfPokemonData);
            pokemon = team.PokemonList[pool.ExtractedFile.GetUShortAtOffset(num)];

            Moves = new Move[4];
            for (int i = 0; i < Moves.Length; i++)
            {
                Moves[i] = pool.MoveList[pool.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstPokemonMoveOffset + (i * 2))];
            }

            if (IsShadow)
            {
                ShadowMoves = new Move[4];
                for (int i = 0; i < Moves.Length; i++)
                {
                    ShadowMoves[i] = pool.MoveList[pool.DarkPokemon.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.FirstShadowMoveOFfset + (i * 2))];
                }
            }
        }

        public void SetPokemon(ushort dexNum)
        {
            var num = pool.DPKMDataOffset + (Index * Constants.SizeOfPokemonData);
            pool.ExtractedFile.WriteBytesAtOffset(num, dexNum.GetBytes());
            pokemon = pool.PokemonList[dexNum];
        }
        public void SetMove(int index, ushort moveNum)
        {
            pool.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstPokemonMoveOffset + (index * 2), moveNum.GetBytes());
            Moves[index] = pool.MoveList[moveNum];
        }
        public void SetShadowMove(int index, ushort moveNum)
        {
            pool.DarkPokemon.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.FirstShadowMoveOFfset + (index * 2), moveNum.GetBytes());
            ShadowMoves[index] = pool.MoveList[moveNum];
        }

        public byte ShadowCatchRate
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(StartOffset + Constants.ShadowCatchRateOFfset);
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.ShadowCatchRateOFfset, value);
        }
        public ushort ShadowCounter
        {
            get => pool.DarkPokemon.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.ShadowCounterOffset);
            set => pool.DarkPokemon.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.ShadowCounterOffset, value.GetBytes());
        }
        public bool ShadowDataInUse
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(StartOffset + Constants.ShadowInUseFlagOffset) == 0x80;
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.ShadowInUseFlagOffset, value ? (byte)0x80 : (byte)0);
        }
        public byte ShadowFleeValue
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(StartOffset + Constants.FleeAfterBattleOffset);
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.FleeAfterBattleOffset, value);
        }
        public byte ShadowAgression
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(StartOffset + Constants.ShadowAggressionOffset);
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.ShadowAggressionOffset, value);
        }
        public byte ShadowAlwaysFlee
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(StartOffset + Constants.ShadowAlwaysFleeOffset);
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.ShadowAlwaysFleeOffset, value);
        }
    }
}
