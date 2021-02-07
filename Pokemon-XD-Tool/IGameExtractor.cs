﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;

namespace Randomizer
{
    public interface IGameExtractor
    {
        ITrainerPool[] ExtractPools(Pokemon[] pokemon, Move[] moves);
        Move[] ExtractMoves();
        Pokemon[] ExtractPokemon();
    }
}
