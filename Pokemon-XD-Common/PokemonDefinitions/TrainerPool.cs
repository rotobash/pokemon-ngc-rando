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
        ShadowTrainerPool DarkPokemon { get; set; }
    }
    public abstract class TrainerPool : ITrainerPool
    {
        public static TrainerPoolType[] MainTeams = new[] { TrainerPoolType.Story, TrainerPoolType.Colosseum, TrainerPoolType.Hundred, TrainerPoolType.Virtual };
        public static TrainerPoolType[] Trainers = new[] { TrainerPoolType.Story, TrainerPoolType.Colosseum, TrainerPoolType.Hundred, TrainerPoolType.Virtual, TrainerPoolType.Imasugu, TrainerPoolType.Bingo, TrainerPoolType.Sample };

        static byte[] kOffensiveDTAI = new byte[] { 0x0F, 0x3A, 0x00, 0x00, 0x73, 0x73, 0x74, 0x73, 0x73, 0x74, 0x82, 0x00, 0x2C, 0x27, 0x50, 0x00, 0x50, 0x32, 0x14, 0x0A, 0x09, 0x09, 0x32, 0x32, 0x00, 0x09, 0x00, 0x09, 0x32, 0x32, 0x08, 00 };
        static byte[] kDefensiveDTAI = new byte[] { 0x0F, 0x3A, 0x00, 0x00, 0x73, 0x73, 0x74, 0x73, 0x73, 0x74, 0x82, 0x00, 0x2C, 0x27, 0x50, 0x00, 0x50, 0x1E, 0x00, 0x0A, 0x07, 0x09, 0x4B, 0x32, 0x00, 0x03, 0x00, 0x01, 0x4B, 0x19, 0x04, 0x00 };
        static byte[] kSimpleDTAI = new byte[] { 0x4F, 0x3E, 0x00, 0x00, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x00, 0x2B, 0x29, 0x32, 0x64, 0x32, 0x32, 0x32, 0x32, 0x09, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        static byte[] kCycleDTAI = new byte[] { 0x27, 0x2A, 0x00, 0x00, 0x73, 0x73, 0x74, 0x73, 0x73, 0x74, 0x82, 0x00, 0x2C, 0x27, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        internal int DTNRHeaderOffset => 0x10;
        internal int DTNRDataOffset => DTNRHeaderOffset + 0x10;
        internal int DPKMHeaderOffset => DTNRHeaderOffset + GetSize(DTNRHeaderOffset);
        internal int DPKMDataOffset => DPKMHeaderOffset + 0x10;
        internal int DTAIHeaderOffset => DPKMHeaderOffset + GetSize(DPKMHeaderOffset);
        internal int DTAIDataOffset => DTAIHeaderOffset + 0x10;
        internal int DSTRHeaderOffset => DTAIHeaderOffset + GetSize(DTAIHeaderOffset);
        internal int DSTRDataOffset => DSTRHeaderOffset + 0x10;

        public FileTypes FileType;

        public TrainerPoolType TeamType { get; }
        public IEnumerable<ITrainer> AllTrainers { get; protected set; }

        public Stream ExtractedFile { get; protected set; }
        internal Pokemon[] PokemonList { get; }
        internal Move[] MoveList { get; }

        public ShadowTrainerPool DarkPokemon { get; set; }

        protected ISO iso;

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

        void AddOrreColoAI()
        {
            ExtractedFile.Seek(DTAIDataOffset + XDTrainer.SizeOfAIData, SeekOrigin.Begin);
            ExtractedFile.Write(kOffensiveDTAI);

            ExtractedFile.Seek(kOffensiveDTAI.Length, SeekOrigin.Current);
            ExtractedFile.Write(kOffensiveDTAI);

            ExtractedFile.Seek(kDefensiveDTAI.Length, SeekOrigin.Current);
            ExtractedFile.Write(kDefensiveDTAI);

            ExtractedFile.Seek(kSimpleDTAI.Length, SeekOrigin.Current);
            ExtractedFile.Write(kSimpleDTAI);

            ExtractedFile.Seek(kCycleDTAI.Length, SeekOrigin.Current);
            ExtractedFile.Write(kCycleDTAI);

            ExtractedFile.Flush();
        }
    }
}
