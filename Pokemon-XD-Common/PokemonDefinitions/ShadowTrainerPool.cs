using System;
using System.Collections.Generic;
using System.IO;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class ShadowTrainerPool : TrainerPool
    {
        internal int DDPKHeaderOffset => 0x10;
        internal int DDPKDataOffset => DDPKHeaderOffset + 0x10;

        public ShadowTrainerPool(ISO iso, Pokemon[] pokemon, Move[] moveList) : base(TrainerPoolType.DarkPokemon, iso, pokemon, moveList)
        {
            var deckArchive = iso.GetFSysFile("deck_archive.fsys");
            var fileEntry = deckArchive.GetEntryByFileName($"DeckData_{TrainerPoolType.DarkPokemon}.bin");
            ExtractedFile = fileEntry.ExtractedFile;
        }
    }
}
