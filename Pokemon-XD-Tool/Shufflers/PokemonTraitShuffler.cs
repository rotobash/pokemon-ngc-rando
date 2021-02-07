using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace Randomizer.Shufflers
{
    public enum MoveCompatibility
    {
        Unchanged,
        RandomPreferType,
        Random,
        Full
    }

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

        public MoveCompatibility TMCompatibility;
        public MoveCompatibility TutorCompatibility;

        public bool NoEXP;
        public bool RandomizeMovesets;
    }

    public static class PokemonTraitShuffler
    {
        public static void RandomizePokemonTraits(Random random, Pokemon[] pokemon, Move[] moves, PokemonTraitShufflerSettings settings)
        {
            // store pokemon we've randomized already in a list, for follows evolution
            var pokeBaseStatsRandomized = new List<string>();
            var pokeAbilitiesRandomized = new List<string>();
            var pokeTypesRandomized = new List<string>();

            // do this first for "follow evolution" checks
            if (settings.RandomizeEvolutions)
            {
                foreach (var poke in pokemon)
                {

                }
            }

            foreach (var poke in pokemon)
            {
                if (TeamShuffler.SpecialPokemon.Contains(poke.Index))
                    continue;

                ChangeCompatibility(random, settings.TMCompatibility, poke, true);
                ChangeCompatibility(random, settings.TutorCompatibility, poke, false);

                if (settings.RandomizeBaseStats > 0 && settings.BaseStatsFollowEvolution && !pokeBaseStatsRandomized.Contains(poke.Name))
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

                if (settings.RandomizeAbilities && !pokeAbilitiesRandomized.Contains(poke.Name))
                {
                    RandomizeAbility(random, settings.AllowWonderGuard, settings.BanNegativeAbilities, poke);

                    if (settings.AbilitiesFollowEvolution)
                    {
                        var endOrSplitEvolution = false;
                        Pokemon currentPoke = poke;
                        while (!endOrSplitEvolution)
                        {
                            endOrSplitEvolution = CheckForSplitOrEndEvolution(currentPoke, out var _);

                            if (!endOrSplitEvolution)
                            {
                                var evoPoke = pokemon[currentPoke.Evolutions[0].EvolvesInto];
                                pokeAbilitiesRandomized.Add(currentPoke.Name);
                                evoPoke.SetAbility1((byte)poke.Ability1.Index);
                                evoPoke.SetAbility2((byte)poke.Ability2.Index);
                                currentPoke = evoPoke;
                            }
                        }
                    }

                }

                if (settings.RandomizeTypes && !pokeTypesRandomized.Contains(poke.Name))
                {
                    RandomizeTypes(random, settings, poke);

                    if (settings.TypesFollowEvolution)
                    {
                        var endOrSplitEvolution = false;
                        Pokemon currentPoke = poke;
                        while (!endOrSplitEvolution)
                        {
                            endOrSplitEvolution = CheckForSplitOrEndEvolution(currentPoke, out var _);

                            if (!endOrSplitEvolution)
                            {
                                var evoPoke = pokemon[currentPoke.Evolutions[0].EvolvesInto];
                                pokeTypesRandomized.Add(currentPoke.Name);
                                evoPoke.Type1 = currentPoke.Type1;
                                evoPoke.Type2 = currentPoke.Type2;
                                currentPoke = evoPoke;
                            }
                        }
                    }
                }

                if (settings.FixImpossibleEvolutions)
                {
                    // todoS
                    for (int i = 0; i < poke.Evolutions.Length; i++)
                    {
                        var method = poke.Evolutions[i].EvolutionMethod;
                        if (method == EvolutionMethods.TradeWithItem || method == EvolutionMethods.Trade || method == EvolutionMethods.HappinessDay || method == EvolutionMethods.HappinessNight)
                        {
                            poke.SetEvolution(i, (byte)EvolutionMethods.LevelUp, (ushort)EvolutionConditionType.Level, poke.Evolutions[i].EvolvesInto);
                            break;
                        }
                    }
                }

                if (settings.EasyEvolutions)
                {
                    // todoS
                }

                if (settings.NoEXP)
                {
                    poke.BaseExp = 0;
                }

                if (settings.RandomizeMovesets)
                {
                    for (int i = 0; i < poke.LevelUpMoves.Length; i++)
                    {
                        var move = poke.LevelUpMoves[i];
                        if (move.Level == 0)
                            continue;
                        poke.SetLevelUpMove(i, move.Level, (ushort)random.Next(0, moves.Length));
                    }
                }
            }
        }

        public static bool CheckForSplitOrEndEvolution(Pokemon currentPoke, out int count)
        {
            bool endOrSplitEvolution = false;
            count = 0;
            for (int i = 0; i < currentPoke.Evolutions.Length; i++)
            {
                if (i == 0 && currentPoke.Evolutions[i].EvolutionMethod == EvolutionMethods.None
                    || currentPoke.Evolutions[i].EvolutionMethod != EvolutionMethods.None && i > 0)
                    endOrSplitEvolution = true;

                if (currentPoke.Evolutions[i].EvolutionMethod != EvolutionMethods.None)
                    count++;
            }

            return endOrSplitEvolution;
        }

        private static void RandomizeTypes(Random random, PokemonTraitShufflerSettings settings, Pokemon poke)
        {

            var types = Enum.GetValues<PokemonTypes>();
            PokemonTypes originalType = poke.Type1;
            PokemonTypes type;
            PokemonTypes type2 = PokemonTypes.None;
            bool validTyping;
            do
            {
                type = types[random.Next(0, types.Length)];
                validTyping = type != PokemonTypes.None;
                var forceSecondType = random.Next(0, 10) > 6;
                if ((validTyping && poke.Type2 != PokemonTypes.None) || forceSecondType)
                {
                    if (poke.Type2 != originalType || forceSecondType)
                    {
                        type2 = types[random.Next(0, types.Length)];
                        validTyping = type != PokemonTypes.None || type == type2;
                    }
                    else
                    {
                        type2 = type;
                    }
                }
            } while (!validTyping);


            poke.Type1 = type;
            poke.Type2 = type2;
        }

        public static void RandomizeAbility(Random random, bool allowWonderGuard, bool negativeAbility, Pokemon poke)
        {
            // yes I know but it's easier to set it there because it needs to know what game we're extracting
            var numAbilities = poke.Ability1.NumberOfAbilities;
            bool validAbility;
            do
            {
                if (poke.Name.ToLower() == "shedinja")
                {
                    validAbility = true;
                    continue;
                }

                poke.SetAbility1((byte)random.Next(1, numAbilities));
                var ability1Name = poke.Ability1.Name.ToLower();
                validAbility = CheckValidAbility(allowWonderGuard, negativeAbility, ability1Name);

                if (validAbility && !string.IsNullOrEmpty(poke.Ability2.Name))
                {
                    poke.SetAbility2((byte)random.Next(1, numAbilities));
                    var ability2Name = poke.Ability2.Name.ToLower();
                    validAbility |= CheckValidAbility(allowWonderGuard, negativeAbility, ability2Name);
                }

            } while (!validAbility);
        }

        private static bool CheckValidAbility(bool allowWonderGuard, bool negativeAbility, string abilityName)
        {
            return !((!allowWonderGuard && abilityName == "wonder guard") || (negativeAbility && (abilityName == "truant" || abilityName == "slow start" || abilityName == "defeatist")));
        }

        private static void ChangeCompatibility(Random random, MoveCompatibility moveCompatibility, Pokemon pokemon, bool tms)
        {
            switch (moveCompatibility)
            {
                case MoveCompatibility.Full:
                    {
                        if (tms)
                        {
                            for (int i = 0; i < pokemon.LearnableTMs.Length; i++)
                            {
                                pokemon.SetLearnableTMS(i, true);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < pokemon.TutorMoves.Length; i++)
                            {
                                pokemon.SetTutorMoves(i, true);
                            }
                        }
                    }
                    break;
                case MoveCompatibility.Random:
                    {
                        if (tms)
                        {
                            for (int i = 0; i < pokemon.LearnableTMs.Length; i++)
                            {
                                pokemon.SetLearnableTMS(i, random.Next(0, 2) > 0);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < pokemon.TutorMoves.Length; i++)
                            {
                                pokemon.SetTutorMoves(i, random.Next(0, 2) > 0);
                            }
                        }
                    }
                    break;
                case MoveCompatibility.RandomPreferType:
                    {
                        if (tms)
                        {
                            // todo lookup TM to see type
                            for (int i = 0; i < pokemon.LearnableTMs.Length; i++)
                            {
                                pokemon.SetLearnableTMS(i, random.Next(0, 2) > 0);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < pokemon.TutorMoves.Length; i++)
                            {
                                pokemon.SetTutorMoves(i, random.Next(0, 2) > 0);
                            }
                        }
                    }
                    break;
                default:
                case MoveCompatibility.Unchanged:
                    break;
            }
        }
    }
}
