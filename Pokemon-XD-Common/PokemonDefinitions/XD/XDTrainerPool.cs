using System;
using System.Collections.Generic;
using System.IO;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{

    public class XDTrainerPool : TrainerPool
    {
        public XDTrainerPool(TrainerPoolType poolType, ISO iso, Pokemon[] pokemon, Move[] moveList) : base(poolType, iso, pokemon, moveList)
        {
            var deckArchive = iso.GetFSysFile("deck_archive.fsys");
            var fileEntryName = iso.Region == Region.Europe ? $"DeckData_{poolType}_EU.bin" : $"DeckData_{poolType}.bin";
            var fileEntry = deckArchive.GetEntryByFileName(fileEntryName);
            ExtractedFile = fileEntry.ExtractedFile;
        }

        // load trainers separately because they might need the shadow deck
        public void LoadTrainers()
        {
            var trainerCount = GetEntries(DTNRHeaderOffset);
            var trainers = new ITrainer[trainerCount];
            for (int i = 0; i < trainerCount; i++)
            {
                trainers[i] = new XDTrainer(i, this, iso);
            }
            AllTrainers = trainers;
        }
    }
}
