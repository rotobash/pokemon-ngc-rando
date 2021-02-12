using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace Randomizer.Shufflers
{
    public static class PokemonTraitShuffler
    {
        public static void RandomizePokemonTraits(Random random, PokemonTraitShufflerSettings settings, ExtractedGame extractedGame)
        {
            // store pokemon we've randomized already in a list, for follows evolution
            var pokeBaseStatsRandomized = new List<string>();
            var pokeAbilitiesRandomized = new List<string>();
            var pokeTypesRandomized = new List<string>();

            // do this first for "follow evolution" checks
            if (settings.RandomizeEvolutions)
            {
                foreach (var poke in extractedGame.PokemonList)
                {

                }
            }

            foreach (var poke in extractedGame.PokemonList)
            {
                if (RandomizerConstants.SpecialPokemon.Contains(poke.Index))
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
                    RandomizeAbility(random, settings.AllowWonderGuard, settings.BanNegativeAbilities, extractedGame.Abilities, poke);

                    if (settings.AbilitiesFollowEvolution)
                    {
                        var endOrSplitEvolution = false;
                        Pokemon currentPoke = poke;
                        while (!endOrSplitEvolution)
                        {
                            endOrSplitEvolution = CheckForSplitOrEndEvolution(currentPoke, out var _);

                            if (!endOrSplitEvolution)
                            {
                                var evoPoke = extractedGame.PokemonList[currentPoke.Evolutions[0].EvolvesInto];
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
                                var evoPoke = extractedGame.PokemonList[currentPoke.Evolutions[0].EvolvesInto];
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
                    // todo, set level?
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

                // so I heard you like a challenge...
                if (settings.NoEXP)
                {
                    poke.BaseExp = 0;
                }

                // randomize level up moves
                if (settings.RandomizeMovesets)
                {
                    for (int i = 0; i < poke.LevelUpMoves.Length; i++)
                    {
                        var move = poke.LevelUpMoves[i];
                        if (move.Level == 0)
                            continue;

                        if (settings.MetronomeOnly)
                        {
                            poke.SetLevelUpMove(i, move.Level, RandomizerConstants.MetronomeIndex);
                        }
                        else
                        {
                            poke.SetLevelUpMove(i, move.Level, (ushort)random.Next(0, extractedGame.MoveList.Length));
                        }
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

        public static void RandomizeAbility(Random random, bool allowWonderGuard, bool banNegativeAbility, Ability[] abilities, Pokemon poke)
        {
            // don't do my boy shedinja dirty like this
            if (poke.Name.ToLower() == "shedinja")
            {
                return;
            }

            var numAbilities = abilities.Length;
            IEnumerable<Ability> abilitiesFilter = abilities;            

            if (!allowWonderGuard)
            {
                abilitiesFilter = abilitiesFilter.Where(a => a.Index != RandomizerConstants.WonderGuardIndex);
            }

            if (banNegativeAbility)
            {
                abilitiesFilter = abilitiesFilter.Where(a => !RandomizerConstants.BadAbilityList.Contains(a.Index));

            }

            var potentialAbilities = abilitiesFilter.ToArray();
            var firstAbility = (byte)potentialAbilities[random.Next(0, potentialAbilities.Length)].Index;
            poke.SetAbility1(firstAbility);

            if (!string.IsNullOrEmpty(poke.Ability2.Name))
            {
                var newAbility = firstAbility;
                while (newAbility != firstAbility)
                {
                    newAbility = (byte)potentialAbilities[random.Next(0, potentialAbilities.Length)].Index;
                }
                poke.SetAbility2(newAbility);
            }
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
