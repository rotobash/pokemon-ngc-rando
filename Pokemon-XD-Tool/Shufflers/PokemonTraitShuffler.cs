using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility;

namespace Randomizer.Shufflers
{
    public static class PokemonTraitShuffler
    {
        const int BSTRange = 50;
        public static void RandomizePokemonTraits(Random random, PokemonTraitShufflerSettings settings, ExtractedGame extractedGame)
        {
            // store pokemon we've randomized already in a list, for follows evolution
            var pokeBaseStatsRandomized = new List<string>();
            var pokeAbilitiesRandomized = new List<string>();
            var pokeTypesRandomized = new List<string>();
            var pokeEvosRandomized = new List<int>();
            var easyEvolutions = new List<string>();

            // set up filtered lists here to avoid recalculating it every loop
            IEnumerable<Move> movefilter = extractedGame.ValidMoves;
            if (settings.MoveSetOptions.BanShadowMoves)
            {
                movefilter = movefilter.Where(m => !m.IsShadowMove);
            }

            IEnumerable<Ability> abilitiesFilter = extractedGame.Abilities;
            if (!settings.AllowWonderGuard)
            {
                abilitiesFilter = abilitiesFilter.Where(a => a.Index != RandomizerConstants.WonderGuardIndex);
            }

            if (settings.BanNegativeAbilities)
            {
                abilitiesFilter = abilitiesFilter.Where(a => !RandomizerConstants.BadAbilityList.Contains(a.Index));
            }

            // do this first for "follow evolution" checks
            if (settings.RandomizeEvolutions)
            {
                foreach (var poke in extractedGame.PokemonList)
                {
                    // prevent loops and multiple pokemon evolving into the same pokemon
                    pokeEvosRandomized.Add(poke.Index);
                    var pokeFilter = extractedGame.PokemonList.Where(p => !pokeEvosRandomized.Contains(p.Index));

                    if (settings.EvolutionHasSameType)
                    {
                        pokeFilter = pokeFilter.Where(p => p.Type1 == poke.Type1 || p.Type2 == poke.Type2 || p.Type1 == poke.Type2);
                    }

                    for (int i = 0; i < poke.Evolutions.Length; i++)
                    {
                        var evolution = poke.Evolutions[i];
                        if (evolution.EvolutionMethod == EvolutionMethods.None) continue;

                        if (settings.EvolutionHasSimilarStrength)
                        {
                            var count = 1;
                            var similarStrengths = pokeFilter.Where(p => p.BST >= poke.BST - BSTRange && p.BST <= poke.BST + BSTRange);
                            while (!similarStrengths.Any() && count < 3)
                            {
                                // anybody? hello?
                                count++;
                                similarStrengths = pokeFilter.Where(p => p.BST >= poke.BST - (count * BSTRange) && p.BST <= poke.BST + (count * BSTRange));
                            }
                            pokeFilter = similarStrengths;
                        }

                        var potentialPokes = pokeFilter.ToArray();
                        if (potentialPokes.Length == 0)
                        {
                            // null it out
                            poke.SetEvolution(i, 0, 0, 0);
                        }
                        else
                        {
                            var newPoke = potentialPokes[random.Next(0, potentialPokes.Length)];
                            // same evolution, just evolves into something else
                            poke.SetEvolution(i, (byte)evolution.EvolutionMethod, evolution.EvolutionCondition, (ushort)newPoke.Index);
                            pokeEvosRandomized.Add(newPoke.Index);
                        }
                    }
                }
            }

            foreach (var poke in extractedGame.PokemonList)
            {
                if (RandomizerConstants.SpecialPokemon.Contains(poke.Index))
                    continue;

                ChangeCompatibility(random, settings.TMCompatibility, poke, extractedGame, true);
                if (extractedGame.TutorMoves.Length > 0)
                    ChangeCompatibility(random, settings.TutorCompatibility, poke, extractedGame, false);

                // todo: use enum for bst option
                if (settings.RandomizeBaseStats > 0 && settings.BaseStatsFollowEvolution && !pokeBaseStatsRandomized.Contains(poke.Name))
                {
                    IList<byte> newBsts;
                    if (settings.RandomizeBaseStats == 1)
                    {
                        // shuffle
                        newBsts = new List<byte>
                        {
                            poke.HP,
                            poke.Attack,
                            poke.Defense,
                            poke.SpecialAttack,
                            poke.SpecialDefense,
                            poke.Speed
                        };

                        // everyone do the the fisher-yates shuffle
                        for (int i = newBsts.Count; i > 0; i--)
                        {
                            var j = random.Next(0, i + 1);

                            // apparently xor swapping is slower than using a temp variable
                            // take that john mcaffee
                            var temp = newBsts[j];
                            newBsts[j] = newBsts[i];
                            newBsts[i] = temp;
                        }
                    }
                    else
                    {
                        // random within total
                        var pokeBst = poke.BST;
                        var randomBsts = new byte[6];
                        newBsts = new byte[6];
                        random.NextBytes(randomBsts);

                        var randomSum = randomBsts.Sum(b => b);
                        for (int i = 0; i < newBsts.Count; i++)
                        {
                            newBsts[i] = (byte)(((float)randomBsts[i] / randomSum) * pokeBst);
                        }
                    }

                    poke.HP = newBsts[0];
                    poke.Attack = newBsts[1];
                    poke.Defense = newBsts[2];
                    poke.SpecialAttack = newBsts[3];
                    poke.SpecialDefense = newBsts[4];
                    poke.Speed = newBsts[5];

                    if (settings.BaseStatsFollowEvolution)
                    {
                        pokeBaseStatsRandomized.Add(poke.Name);
                    }
                }

                if (settings.UpdateBaseStats)
                {
                    // todo
                    // need some way of loading them without doing something... regrettable
                }

                if (settings.StandardizeEXPCurves)
                {
                    poke.LevelUpRate = RandomizerConstants.Legendaries.Contains(poke.Index)
                        ? ExpRate.Slow
                        : ExpRate.Fast;
                }

                if (settings.RandomizeAbilities && !pokeAbilitiesRandomized.Contains(poke.Name))
                {
                    var abilities = abilitiesFilter.ToArray();
                    RandomizeAbility(random, abilities, poke);

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
                                evoPoke.Ability1 = poke.Ability1;
                                evoPoke.Ability2 = poke.Ability2;
                                currentPoke = evoPoke;
                            }
                        }
                    }

                }

                if (settings.RandomizeTypes && !pokeTypesRandomized.Contains(poke.Name))
                {
                    RandomizeTypes(random, poke);

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

                if (settings.FixImpossibleEvolutions || settings.EasyEvolutions)
                {
                    for (int i = 0; i < poke.Evolutions.Length; i++)
                    {
                        if (settings.FixImpossibleEvolutions)
                        {
                            var method = poke.Evolutions[i].EvolutionMethod;
                            if (method == EvolutionMethods.TradeWithItem || method == EvolutionMethods.Trade || method == EvolutionMethods.HappinessDay || method == EvolutionMethods.HappinessNight)
                            {
                                poke.SetEvolution(i, (byte)EvolutionMethods.LevelUp, (ushort)Configuration.TradePokemonEvolutionLevel, poke.Evolutions[i].EvolvesInto);
                                break;
                            }
                        }

                        if (settings.EasyEvolutions)
                        {
                            if (!CheckForSplitOrEndEvolution(poke, out int _))
                            {
                                var evolution = poke.Evolutions[0];
                                var evoPoke = extractedGame.PokemonList[evolution.EvolvesInto];

                                // check if we evolve into something else
                                // i.e. if three stage
                                if (!CheckForSplitOrEndEvolution(evoPoke, out int count))
                                {
                                    var evoPokeEvolution = evoPoke.Evolutions[0];
                                    if (evoPokeEvolution.EvolutionMethod == EvolutionMethods.LevelUp && evoPokeEvolution.EvolutionCondition > 40)
                                    {
                                        // make a bold assumption that if the third stage evolves by level up then the second does too
                                        evoPoke.SetEvolution(0, (byte)evoPokeEvolution.EvolutionMethod, 40, evoPokeEvolution.EvolvesInto);
                                        poke.SetEvolution(0, (byte)evolution.EvolutionMethod, 30, evolution.EvolvesInto);
                                    }
                                }
                                else if (count == 0 && evolution.EvolutionMethod == EvolutionMethods.LevelUp && evolution.EvolutionCondition > 40)
                                {
                                    // this is the last stage
                                    poke.SetEvolution(0, (byte)evolution.EvolutionMethod, 40, evolution.EvolvesInto);
                                }
                            }
                        }
                    }
                }

                // so I heard you like a challenge...
                if (settings.NoEXP)
                {
                    poke.BaseExp = 0;
                }

                // randomize level up moves
                if (settings.MoveSetOptions.RandomizeMovesets)
                {
                    var typeFilter = movefilter;
                    if (settings.MoveSetOptions.PreferType)
                    {
                        // allow 20% chance for move to not be same type
                        typeFilter = typeFilter.Where(m => m.Type == poke.Type1 || m.Type == poke.Type2 || random.Next(0, 10) >= 8).ToArray();
                        if (!typeFilter.Any())
                            typeFilter = movefilter;
                    }

                    var potentialMoves = typeFilter.ToArray();
                    for (int i = 0; i < poke.LevelUpMoves.Length; i++)
                    {
                        var move = poke.LevelUpMoves[i];
                        if (move.Level == 0)
                            continue;

                        if (settings.MoveSetOptions.MetronomeOnly)
                        {
                            poke.SetLevelUpMove(i, move.Level, RandomizerConstants.MetronomeIndex);
                        }
                        else
                        {
                            var newMove = potentialMoves[random.Next(0, potentialMoves.Length)];
                            poke.SetLevelUpMove(i, move.Level, (ushort)newMove.MoveIndex);
                        }
                    }
                }
            }
        }

        private static void RandomizeTypes(Random random, Pokemon poke)
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

        public static void RandomizeAbility(Random random, Ability[] potentialAbilities, Pokemon poke)
        {
            // don't do my boy shedinja dirty like this
            if (poke.Index == RandomizerConstants.ShedinjaIndex)
            {
                return;
            }

            var firstAbility = (byte)potentialAbilities[random.Next(0, potentialAbilities.Length)].Index;
            poke.Ability1 = firstAbility;

            if (poke.Ability2 > 0)
            {
                var newAbility = firstAbility;
                while (newAbility != firstAbility)
                {
                    newAbility = (byte)potentialAbilities[random.Next(0, potentialAbilities.Length)].Index;
                }
                poke.Ability2 = newAbility;
            }
        }

        private static void ChangeCompatibility(Random random, MoveCompatibility moveCompatibility, Pokemon pokemon, ExtractedGame extractedGame, bool tms)
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
                            pokemon.SetLearnableTMS(i, random.Next(0, 2) == 0);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < pokemon.TutorMoves.Length; i++)
                        {
                            pokemon.SetTutorMoves(i, random.Next(0, 2) == 0);
                        }
                    }
                }
                break;
                case MoveCompatibility.RandomPreferType:
                {
                    if (tms)
                    {
                        var tmMoves = extractedGame.TMs.Select(t => extractedGame.MoveList[t.Move]).ToArray();
                        for (int i = 0; i < pokemon.LearnableTMs.Length; i++)
                        {
                            var isCompatible = tmMoves[i].Type == pokemon.Type1 || tmMoves[i].Type == pokemon.Type2 || random.Next(0, 10) >= 8;
                            pokemon.SetLearnableTMS(i, isCompatible);
                        }
                    }
                    else
                    {
                        var tutorMoves = extractedGame.TutorMoves.Select(t => extractedGame.MoveList[t.Move]).ToArray();
                        for (int i = 0; i < pokemon.TutorMoves.Length; i++)
                        {
                            var isCompatible = tutorMoves[i].Type == pokemon.Type1 || tutorMoves[i].Type == pokemon.Type2 || random.Next(0, 10) >= 8;
                            pokemon.SetTutorMoves(i, isCompatible);
                        }
                    }
                }
                break;
                default:
                case MoveCompatibility.Unchanged:
                    break;
            }
        }

        public static bool CheckForSplitOrEndEvolution(Pokemon currentPoke, out int count)
        {
            bool endOrSplitEvolution = false;
            count = 0;
            for (int i = 0; i < currentPoke.Evolutions.Length; i++)
            {
                // if more than one definition found or the first evolution is none
                if (i == 0 && currentPoke.Evolutions[i].EvolutionMethod == EvolutionMethods.None
                    || currentPoke.Evolutions[i].EvolutionMethod != EvolutionMethods.None && i > 0)
                    endOrSplitEvolution = true;

                // keep count for split evos
                if (currentPoke.Evolutions[i].EvolutionMethod != EvolutionMethods.None)
                    count++;
            }

            return endOrSplitEvolution;
        }
    }
}
