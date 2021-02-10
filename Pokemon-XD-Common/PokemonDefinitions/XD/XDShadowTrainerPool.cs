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


        public XDShadowTrainerPool(ISO iso, Pokemon[] pokemon, Move[] moveList) : base(TrainerPoolType.DarkPokemon, iso, pokemon, moveList)
        {
        }
    }
}
