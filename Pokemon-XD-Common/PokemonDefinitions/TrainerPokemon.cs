using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public interface ITrainerPokemon
    {
        ushort Pokemon { get; set; }
        bool IsShadow { get; }
        bool IsSet { get; }
        byte ShadowCatchRate { get; set; }
        byte Level { get; set; }
        byte ShadowLevel { get; set; }
        ushort Item { get; set; }
        ushort[] Moves { get; }
        void SetMove(int index, ushort moveNum);
        void SetShadowMove(int index, ushort moveNum);
    }

    public abstract class TrainerPokemon : ITrainerPokemon
    {
        protected abstract uint StartOffset { get; }
        protected abstract uint ShadowStartOffset { get; }
        protected abstract byte PokemonIndexOffset { get; }
        protected abstract byte PokemonItemOffset { get;  }
        protected abstract byte PokemonHappinessOffset { get; }
        protected abstract byte FirstPokemonEVOffset { get; }
        protected abstract byte ShadowCatchRateOffset { get; }
        protected abstract byte ShadowCounterOffset { get;}
        protected abstract byte FirstPokemonMoveOffset { get; }
        protected abstract byte PokemonMoveDataSize { get; }

        public abstract byte Level { get; set; }
        public abstract byte ShadowLevel { get; set; }
        public abstract bool IsShadow { get; }
        public abstract bool IsSet { get; }
        public abstract void SetShadowMove(int index, ushort moveNum);

        public int Index { get; }
        public ushort[] Moves { get; }

        protected ISO iso;
        protected ITrainerPool pool;

        public virtual ushort Pokemon
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + PokemonIndexOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + PokemonIndexOffset, value.GetBytes());
        }

        public virtual ushort Item
        {
            get => pool.ExtractedFile.GetUShortAtOffset(StartOffset + PokemonItemOffset);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + PokemonItemOffset, value.GetBytes());
        }

        public virtual byte Happiness
        {
            get => pool.ExtractedFile.GetByteAtOffset(StartOffset + PokemonHappinessOffset);
            set => pool.ExtractedFile.WriteByteAtOffset(StartOffset + PokemonHappinessOffset, value);
        }

        public virtual byte[] EVs
        {
            get => pool.ExtractedFile.GetBytesAtOffset(StartOffset + FirstPokemonEVOffset, Constants.NumberOfEVs);
            set => pool.ExtractedFile.WriteBytesAtOffset(StartOffset + FirstPokemonEVOffset, value);
        }

        public virtual byte ShadowCatchRate
        {
            get => pool.DarkPokemon.ExtractedFile.GetByteAtOffset(ShadowStartOffset + ShadowCatchRateOffset);
            set => pool.DarkPokemon.ExtractedFile.WriteByteAtOffset(ShadowStartOffset + ShadowCatchRateOffset, value);
        }
        public virtual ushort ShadowCounter
        {
            get => pool.DarkPokemon.ExtractedFile.GetUShortAtOffset(ShadowStartOffset + ShadowCounterOffset);
            set => pool.DarkPokemon.ExtractedFile.WriteBytesAtOffset(ShadowStartOffset + ShadowCounterOffset, value.GetBytes());
        }


        protected TrainerPokemon(int index, ITrainerPool trainerPool)
        {
            Index = index;
            Moves = new ushort[Constants.NumberOfPokemonMoves];
            pool = trainerPool;
        }

        public void SetMove(int index, ushort moveNum)
        {
            if (index >= 0 && index < Moves.Length)
            {
                Moves[index] = moveNum;
                pool.ExtractedFile.WriteBytesAtOffset(StartOffset + FirstPokemonMoveOffset + index * PokemonMoveDataSize, moveNum.GetBytes());
            }
        }
    }
}
