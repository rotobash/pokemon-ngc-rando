using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;

namespace Randomizer.Shufflers
{
    public struct PokemonTraitShufflerSettings
    {
        public int RandomizeBaseStats;
        public bool StandardizeEXPCurves;
        public bool BaseStatsFollowEvolution;
        public bool UpdateBaseStats;

        public bool RandomizeAbilities;
        public bool AllowWonderGuard;
        public bool AbilitiesFollowEvolution;
        public bool BanNegativeAbilities;

        public bool RandomizeTypes;
        public bool TypesFollowEvolution;

        public bool RandomizeEvolutions;
        public bool EvolutionHasSimilarStrength;
        public bool EvolutionHasSameType;
        public bool ThreeStageEvolution;
        public bool EasyEvolutions;
        public bool FixImpossibleEvolutions;
    }

    public static class PokemonTraitShuffler
    {
        public static void RandomizePokemonTraits(Random random, Pokemon[] pokemon, PokemonTraitShufflerSettings settings)
        {
            foreach (var poke in pokemon)
            {
                if (settings.RandomizeBaseStats > 0)
                {
                    // todo

                    if (settings.BaseStatsFollowEvolution)
                    {

                    }
                }

                if (settings.UpdateBaseStats)
                {
                    // todo
                }

                if (settings.StandardizeEXPCurves)
                {
                    poke.LevelUpRate = ExpRate.Fast;
                }

                if (settings.RandomizeAbilities)
                {
                    // yes I know but it's easier to set it there because it needs to know what game we're extracting
                    var numAbilities = pokemon[0].Ability1.NumberOfAbilities;
                    bool validAbility;
                    do
                    {
                        if (poke.Name != "Shedinja")
                        {
                            validAbility = true;
                            continue;
                        }

                        poke.SetAbility1((byte)random.Next(1, numAbilities));
                        validAbility = !settings.AllowWonderGuard && poke.Ability1.Name == "Wonder Guard";
                        validAbility |= settings.BanNegativeAbilities && (poke.Ability1.Name == "Truant" || poke.Ability1.Name == "Slow Start" || poke.Ability1.Name == "Defeatist");
                    } while (!validAbility);

                    if (settings.AbilitiesFollowEvolution)
                    {
                        // do something here
                    }

                }

                if (settings.RandomizeTypes)
                {
                    var types = Enum.GetValues<PokemonTypes>();
                    PokemonTypes type;
                    do
                    {
                        type = types[random.Next(0, types.Length)];
                    } while (type == PokemonTypes.None);

                    if (poke.Type2 != poke.Type1 && poke.Type2 != PokemonTypes.None)
                    {
                        PokemonTypes type2;
                        do
                        {
                            type2 = types[random.Next(0, types.Length)];
                        } while (type2 == PokemonTypes.None || type == type2);
                        poke.Type2 = type2;
                    }
                    poke.Type1 = type;

                    if (settings.TypesFollowEvolution)
                    {
                        // do something here
                    }
                }

                if (settings.RandomizeEvolutions)
                {
                    // todoS
                }

                if (settings.FixImpossibleEvolutions)
                {
                    // todoS
                }

                if (settings.EasyEvolutions)
                {
                    // todoS
                }
            }
        }
    }
}
