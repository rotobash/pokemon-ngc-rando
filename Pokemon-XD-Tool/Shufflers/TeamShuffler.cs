using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace Randomizer.Shufflers
{
    public struct TeamShufflerSettings
    {
        public bool RandomizePokemon;
    }

    public static class TeamShuffler
    {
        // invalid pokemon
        public static readonly List<int> BannedPokemon = new List<int>
        {
            0,
            252,
            253,
            254,
            255,
            256,
            257,
            258,
            259,
            260,
            261,
            262,
            263,
            264,
            265,
            266,
            267,
            268,
            269,
            270,
            271,
            272,
            273,
            274,
            275,
            276,
            412
        };


        public static void ShuffleTeams(Random random, TeamShufflerSettings settings, TrainerPool[] trainerPools, Pokemon[] pokemonList)
        {
            if (settings.RandomizePokemon)
            {
                // yikes
                foreach (var pool in trainerPools)
                {
                    foreach (var trainer in pool.AllTrainers)
                    {
                        if (trainer.TrainerClass == 0)
                            continue;

                        foreach (var pokemon in trainer.Pokemon)
                        {
                            if (pokemon.Pokemon.Index == 0)
                                continue;

                            var index = 0;
                            while (BannedPokemon.Contains(index))
                            {
                                index = random.Next(1, pokemonList.Length);
                            }
                            pokemon.SetPokemon((ushort)index);
                        }
                    }
                }
            }

        }
    }
}
