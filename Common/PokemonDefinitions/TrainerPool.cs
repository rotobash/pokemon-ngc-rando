using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public interface ITrainerPool
    {
        Stream ExtractedFile { get; }
        TrainerPoolType TeamType { get; }
        IEnumerable<ITrainer> AllTrainers { get; }
        TrainerPool DarkPokemon { get; set; }
    }
    public abstract class TrainerPool : ITrainerPool
    {
        public TrainerPoolType TeamType { get; }
        public IEnumerable<ITrainer> AllTrainers { get; protected set; }

        public Stream ExtractedFile { get; protected set; }
        internal Pokemon[] PokemonList { get; }
        internal Move[] MoveList { get; }

        protected ISO iso;

        // xd  only
        public FileTypes FileType;
        public virtual TrainerPool DarkPokemon { get; set; }

        protected TrainerPool(TrainerPoolType poolType, ISO iso, Pokemon[] pokemon, Move[] moveList)
        {
            this.iso = iso;
            FileType = FileTypes.BIN;
            TeamType = poolType;
            PokemonList = pokemon;
            MoveList = moveList;
        }

        internal int GetSize(int headerOffset)
        {
            return ExtractedFile.GetIntAtOffset(headerOffset + 4);
        }

        internal int GetEntries(int headerOffset)
        {
            return ExtractedFile.GetIntAtOffset(headerOffset + 8);
        }
    }
}
