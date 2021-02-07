using System;
using System.Collections.Generic;
using System.IO;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class XDShadowTrainerPool : XDTrainerPool
    {
        internal int DDPKHeaderOffset => 0x10;
        internal int DDPKDataOffset => DDPKHeaderOffset + 0x10;

        public override XDShadowTrainerPool DarkPokemon => this;


        public XDShadowTrainerPool(IExtractedFile fileEntry, ISO iso, Pokemon[] pokemon, Move[] moveList) : base(TrainerPoolType.DarkPokemon, fileEntry, pokemon, moveList)
        {
            // todo: when extracting files works remove fileEntry parameter and get it from ISO like so:
            //var deckArchive = iso.GetFSysFile("deck_archive.fsys");
            //var fileEntry = deckArchive.ExtractEntryByFileName($"DeckData_DarkPokemon.bin");
        }
    }
}
