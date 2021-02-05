using System;
using System.Collections.Generic;
using System.IO;
using XDCommon.Contracts;
using XDCommon.PokemonDefinitions;

namespace XDCommon.Utility
{
    public class TrainerPool
    {
        public static TrainerTeamTypes[] MainTeams = new[] { TrainerTeamTypes.Story, TrainerTeamTypes.Colosseum, TrainerTeamTypes.Hundred, TrainerTeamTypes.Virtual };
        public static TrainerTeamTypes[] Trainers = new[] { TrainerTeamTypes.Story, TrainerTeamTypes.Colosseum, TrainerTeamTypes.Hundred, TrainerTeamTypes.Virtual, TrainerTeamTypes.Imasugu, TrainerTeamTypes.Bingo, TrainerTeamTypes.Sample };

        static byte[] kOffensiveDTAI = new byte[] { 0x0F, 0x3A, 0x00, 0x00, 0x73, 0x73, 0x74, 0x73, 0x73, 0x74, 0x82, 0x00, 0x2C, 0x27, 0x50, 0x00, 0x50, 0x32, 0x14, 0x0A, 0x09, 0x09, 0x32, 0x32, 0x00, 0x09, 0x00, 0x09, 0x32, 0x32, 0x08, 00 };
        static byte[] kDefensiveDTAI = new byte[] { 0x0F, 0x3A, 0x00, 0x00, 0x73, 0x73, 0x74, 0x73, 0x73, 0x74, 0x82, 0x00, 0x2C, 0x27, 0x50, 0x00, 0x50, 0x1E, 0x00, 0x0A, 0x07, 0x09, 0x4B, 0x32, 0x00, 0x03, 0x00, 0x01, 0x4B, 0x19, 0x04, 0x00 };
        static byte[] kSimpleDTAI = new byte[] { 0x4F, 0x3E, 0x00, 0x00, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x75, 0x00, 0x2B, 0x29, 0x32, 0x64, 0x32, 0x32, 0x32, 0x32, 0x09, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        static byte[] kCycleDTAI = new byte[] { 0x27, 0x2A, 0x00, 0x00, 0x73, 0x73, 0x74, 0x73, 0x73, 0x74, 0x82, 0x00, 0x2C, 0x27, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        internal int DDPKHeaderOffset => 0x10;
        internal int DDPKDataOffset => DDPKHeaderOffset + 0x10;
        internal int DTNRHeaderOffset => 0x10;
        internal int DTNRDataOffset => DTNRHeaderOffset + 0x10;
        internal int DPKMHeaderOffset => DTNRHeaderOffset + GetSize(DTNRHeaderOffset);
        internal int DPKMDataOffset => DPKMHeaderOffset + 0x10;
        internal int DTAIHeaderOffset => DPKMHeaderOffset + GetSize(DPKMHeaderOffset);
        internal int DTAIDataOffset => DTAIHeaderOffset + 0x10;
        internal int DSTRHeaderOffset => DTAIHeaderOffset + GetSize(DTAIHeaderOffset);
        internal int DSTRDataOffset => DSTRHeaderOffset + 0x10;

        public TrainerTeamTypes TeamType { get; }
        public IEnumerable<Trainer> AllTrainers { get; }
        public IEnumerable<TrainerPoolPokemon> AllPokemon { get; }
        public IEnumerable<TrainerPokemon> AllTrainerPokemon { get; }

        public Stream ExtractedFile;
        public FileTypes FileType;

        public TrainerPool(TrainerTeamTypes teamType, IExtractedFile fileEntry, ISO iso)
        {
            ExtractedFile = fileEntry.ExtractedFile;
            FileType = FileTypes.BIN;
            TeamType = teamType;

            var trainerCount = GetEntries(DTNRHeaderOffset);
            var trainers = new Trainer[trainerCount];
            for (int i = 0; i < trainerCount; i++)
            {
                trainers[i] = new Trainer(i, this, iso);
            }
            AllTrainers = trainers;

            TrainerPoolPokemon[] pokemon;
            if (TeamType == TrainerTeamTypes.DarkPokemon) 
            {
                var pokemonCount = GetEntries(DDPKDataOffset);
                pokemon = new TrainerPoolPokemon[pokemonCount];
                for (int i = 0; i < pokemonCount; i++)
                {
                    pokemon[i] = new TrainerPoolPokemon(i, this, PokemonFileType.DDPK, iso);
                }
            }
            else
            {
                var pokemonCount = GetEntries(DPKMDataOffset);
                pokemon = new TrainerPoolPokemon[pokemonCount];
                for (int i = 0; i < pokemonCount; i++)
                {
                    pokemon[i] = new TrainerPoolPokemon(i, this, PokemonFileType.DPKM, iso);
                }
            }
            AllPokemon = pokemon;

            var trainerPokemon = new List<TrainerPokemon>();
            foreach (var poke in AllPokemon)
            {
                if (poke.PokeType == PokemonFileType.DPKM)
                {
                    trainerPokemon.Add(new TrainerPokemon(poke));
                }
            }
            AllTrainerPokemon = trainerPokemon;
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
            ExtractedFile.Seek(DTAIDataOffset + Trainer.kSizeOfAIData, SeekOrigin.Begin);
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
