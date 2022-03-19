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
        public static void RandomizePokemonTraits(ShuffleSettings shuffleSettings)
        {
            var settings = shuffleSettings.RandomizerSettings.PokemonTraitShufflerSettings;
            var extractedGame = shuffleSettings.ExtractedGame;
            var random = shuffleSettings.RNG;

            // store pokemon we've randomized already in a list, for follows evolution
            var pokeBaseStatsRandomized = new List<string>();
            var pokeAbilitiesRandomized = new List<string>();
            var pokeTypesRandomized = new List<string>();
            var pokeEvosRandomized = new List<int>();
            var easyEvolutions = new List<string>();

            Logger.Log("=============================== Pokemon ===============================\n\n");

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
                Logger.Log($"Setting New Evolutions\n\n");
                foreach (var poke in extractedGame.ValidPokemon)
                {
                    // prevent loops and multiple pokemon evolving into the same pokemon
                    pokeEvosRandomized.Add(poke.Index);
                    var pokeFilter = extractedGame.ValidPokemon.Where(p => !pokeEvosRandomized.Contains(p.Index));
                    var isEevee = poke.Name.Equals("eevee", StringComparison.InvariantCultureIgnoreCase);

                    // check for eevee, don't bother with type filtering
                    if (settings.EvolutionHasSameType && !isEevee)
                    {
                        pokeFilter = pokeFilter.Where(p => p.Type1 == poke.Type1 || p.Type2 == poke.Type2 || p.Type1 == poke.Type2);
                    }

                    for (int i = 0; i < poke.Evolutions.Length; i++)
                    {
                        var evolution = poke.Evolutions[i];
                        if (evolution.EvolutionMethod == EvolutionMethods.None && i > 0) continue;

                        if (settings.EvolutionHasSimilarStrength)
                        {
                            var count = 1;
                            var similarStrengthPoke = evolution.EvolutionMethod != EvolutionMethods.None ? extractedGame.PokemonList[evolution.EvolvesInto] : poke;
                            IEnumerable<Pokemon> similarStrengths = Array.Empty<Pokemon>();
                            while (!similarStrengths.Any() && count < 3)
                            {
                                // anybody? hello?
                                similarStrengths = pokeFilter.Where(p => p.BST >= similarStrengthPoke.BST - (count * BSTRange) && p.BST <= similarStrengthPoke.BST + (count * BSTRange));
                                count++;
                            }
                            pokeFilter = similarStrengths;
                        }

                        var potentialPokes = pokeFilter.ToArray();
                        if (potentialPokes.Length == 0 || (settings.EvolutionLinesEndRandomly && evolution.EvolutionMethod == EvolutionMethods.LevelUp && random.Next(4) > 2))
                        {
                            // null it out
                            Logger.Log($"End of the line for {poke.Name}.\n");
                            poke.SetEvolution(i, 0, 0, 0);
                        }
                        else
                        {
                            var newPoke = potentialPokes[random.Next(0, potentialPokes.Length)];
                            var evolvesIntoText = evolution.EvolutionMethod == EvolutionMethods.None
                                ? string.Empty
                                : $"instead of { extractedGame.PokemonList[evolution.EvolvesInto].Name}";

                            Logger.Log($"{poke.Name} evolves into {newPoke.Name}\n");
                            // same evolution, just evolves into something else
                            poke.SetEvolution(i, (byte)evolution.EvolutionMethod, evolution.EvolutionCondition, (ushort)newPoke.Index);
                            pokeEvosRandomized.Add(newPoke.Index);
                        }
                        Logger.Log($"\n");
                    }
                }
            }

            foreach (var poke in extractedGame.PokemonList)
            {
                if (RandomizerConstants.SpecialPokemon.Contains(poke.Index))
                    continue;

                Logger.Log($"{poke.Name}\n");

                ChangeCompatibility(random, extractedGame, settings.TMCompatibility, poke, true);
                if (extractedGame.TutorMoves.Length > 0)
                    ChangeCompatibility(random, extractedGame, settings.TutorCompatibility, poke, false);

                // todo: use enum for bst option
                // and follow evolution
                if (settings.RandomizeBaseStats > 0) // || (settings.BaseStatsFollowEvolution && !pokeBaseStatsRandomized.Contains(poke.Name)))
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
                        for (int i = newBsts.Count - 1; i >= 0; i--)
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

                    Logger.Log($"Setting BSTs: H {newBsts[0]}/A {newBsts[1]}/D {newBsts[2]}/SpA {newBsts[3]}/SpD {newBsts[4]}/S {newBsts[5]}.\n");

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

                    Logger.Log($"Setting EXP rate to {poke.LevelUpRate}.\n");
                }

                if (settings.RandomizeAbilities && !pokeAbilitiesRandomized.Contains(poke.Name))
                {
                    var abilities = abilitiesFilter.ToArray();
                    RandomizeAbility(random, abilities, poke);

                    if (settings.AbilitiesFollowEvolution)
                    {
                        var endOrSplitEvolution = false;
                        Pokemon currentPoke = poke;
                        pokeAbilitiesRandomized.Add(currentPoke.Name);
                        while (!endOrSplitEvolution)
                        {
                            endOrSplitEvolution = CheckForSplitOrEndEvolution(currentPoke, out var _);

                            if (!endOrSplitEvolution)
                            {
                                var evoPoke = extractedGame.PokemonList[currentPoke.Evolutions[0].EvolvesInto];
                                evoPoke.Ability1 = poke.Ability1;
                                evoPoke.Ability2 = poke.Ability2;

                                Logger.Log($"Setting {evoPoke.Name}'s Ability to match {poke.Name}.\n");

                                pokeAbilitiesRandomized.Add(evoPoke.Name);
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
                        pokeTypesRandomized.Add(currentPoke.Name);
                        while (!endOrSplitEvolution)
                        {
                            endOrSplitEvolution = CheckForSplitOrEndEvolution(currentPoke, out var _);

                            if (!endOrSplitEvolution)
                            {
                                var evoPoke = extractedGame.PokemonList[currentPoke.Evolutions[0].EvolvesInto];
                                evoPoke.Type1 = currentPoke.Type1;
                                evoPoke.Type2 = currentPoke.Type2;

                                Logger.Log($"Setting {evoPoke.Name}'s Type to match {poke.Name}.\n");

                                pokeTypesRandomized.Add(evoPoke.Name);
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
                                poke.SetEvolution(i, (byte)EvolutionMethods.LevelUp, (ushort)Configuration.PokemonImpossibleEvolutionLevel, poke.Evolutions[i].EvolvesInto);
                                Logger.Log($"Setting to evolve via level up at level {Configuration.PokemonImpossibleEvolutionLevel}\n");
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
                                        // make a bold assumption that if the second stage evolves by level up then the first does too
                                        evoPoke.SetEvolution(0, (byte)evoPokeEvolution.EvolutionMethod, 40, evoPokeEvolution.EvolvesInto);
                                        poke.SetEvolution(0, (byte)evolution.EvolutionMethod, 30, evolution.EvolvesInto);

                                        Logger.Log($"Setting {poke.Name} to evolve at level 30.\n");
                                        Logger.Log($"Setting {evoPoke.Name} to evolve at level 40.\n");
                                    }
                                }
                                else if (count == 0 && evolution.EvolutionMethod == EvolutionMethods.LevelUp && evolution.EvolutionCondition > 40)
                                {
                                    // this is the last stage
                                    poke.SetEvolution(0, (byte)evolution.EvolutionMethod, 40, evolution.EvolvesInto);
                                    Logger.Log($"Setting to evolve at level 40.\n");
                                }
                            }
                        }
                    }
                    Logger.Log($"\n");
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
                        typeFilter = typeFilter.Where(m =>  m.Type == poke.Type1 || m.Type == poke.Type2);

                        if (!typeFilter.Any())
                            typeFilter = movefilter;
                    }

                    var potentialMoves = typeFilter.ToArray();
                    var levelUpSet = new HashSet<int>();

                    // pick non duplicate moves for all level up moves using a set
                    // to ensure no duplicates, we have to pick all the moves first then assign them
                    while (levelUpSet.Count < poke.LevelUpMoves.Length)
                    {
                        int moveIndex;
                        if (settings.MoveSetOptions.MetronomeOnly)
                            moveIndex = extractedGame.MoveList[RandomizerConstants.MetronomeIndex].MoveIndex;
                        else if (settings.MoveSetOptions.PreferType && random.Next(0, 10) >= 8)
                            // allow 20% chance for move to not be same type
                            moveIndex = movefilter.ElementAt(random.Next(0, movefilter.Count())).MoveIndex;
                        else
                            moveIndex = potentialMoves[random.Next(0, potentialMoves.Length)].MoveIndex;

                        levelUpSet.Add(moveIndex);
                    }

                    // TODO: add move power re-ordering

                    // go through each level up move and set them
                    for (int i = 0; i < poke.LevelUpMoves.Length; i++)
                    {
                        var move = poke.LevelUpMoves[i];
                        if (move.Level == 0)
                            continue;

                        Move newMove = extractedGame.MoveList[levelUpSet.ElementAt(i)];

                        poke.SetLevelUpMove(i, move.Level, (ushort)newMove.MoveIndex);
                        Logger.Log($"Level Up Move at level {move.Level}: {newMove.Name}\n");
                    }
                }

                Logger.Log($"\n");
            }
        }

        private static void RandomizeTypes(AbstractRNG random, Pokemon poke)
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


            Logger.Log($"Type 1: {type}\n");
            Logger.Log($"Type 2: {type2}\n");

            poke.Type1 = type;
            poke.Type2 = type2;
        }

        public static void RandomizeAbility(AbstractRNG random, Ability[] potentialAbilities, Pokemon poke)
        {
            // don't do my boy shedinja dirty like this
            if (poke.Index == RandomizerConstants.ShedinjaIndex)
            {
                Logger.Log($"No");
                return;
            }

            var firstAbility = potentialAbilities[random.Next(potentialAbilities.Length)];
            poke.Ability1 = (byte)firstAbility.Index;
            Logger.Log($"Ability 1: {firstAbility.Name}\n");

            if (poke.Ability2 > 0)
            {
                var newAbility = firstAbility;
                while (newAbility.Index != firstAbility.Index)
                {
                    newAbility = potentialAbilities[random.Next(0, potentialAbilities.Length)];
                }
                poke.Ability2 = (byte)newAbility.Index;
                Logger.Log($"Ability 2: {newAbility.Name}\n");
            }
        }

        private static void ChangeCompatibility(AbstractRNG random, ExtractedGame extractedGame, MoveCompatibility moveCompatibility, Pokemon pokemon, bool tms)
        {
            switch (moveCompatibility)
            {
                case MoveCompatibility.Full:
                {
                    if (tms)
                    {
                        Logger.Log($"TM Compatibility: Full\n");
                        for (int i = 0; i < pokemon.LearnableTMs.Length; i++)
                        {
                            pokemon.SetLearnableTMS(i, true);
                        }
                    }
                    else
                    {
                        Logger.Log($"Tutor Move Compatibility: Full\n");
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
                        Logger.Log($"TM Compatibility:\n");
                        for (int i = 0; i < pokemon.LearnableTMs.Length; i++)
                        {
                            var compatible = random.Next(0, 2) == 0;
                            pokemon.SetLearnableTMS(i, compatible);
                            Logger.Log($"TM{extractedGame.TMs[i].TMIndex} - {compatible}\n");
                        }
                    }
                    else
                    {
                        Logger.Log($"Tutor Move Compatibility:\n");
                        for (int i = 0; i < pokemon.TutorMoves.Length; i++)
                        {
                            var compatible = random.Next(0, 2) == 0;
                            pokemon.SetTutorMoves(i, compatible);
                            Logger.Log($"{i + 1} - {compatible}\n");
                        }
                    }
                    Logger.Log($"\n");
                }
                break;
                case MoveCompatibility.RandomPreferType:
                {
                    if (tms)
                    {
                        Logger.Log($"TM Compatibility:\n");
                        var tmMoves = extractedGame.TMs.Select(t => extractedGame.MoveList[t.Move]).ToArray();
                        for (int i = 0; i < pokemon.LearnableTMs.Length; i++)
                        {
                            var isCompatible = tmMoves[i].Type == pokemon.Type1 || tmMoves[i].Type == pokemon.Type2 || random.Next(0, 10) >= 8;
                            pokemon.SetLearnableTMS(i, isCompatible);
                            Logger.Log($"TM{extractedGame.TMs[i].TMIndex} - {isCompatible}\n");
                        }
                    }
                    else
                    {
                        Logger.Log($"Tutor Move Compatibility:\n");
                        var tutorMoves = extractedGame.TutorMoves.Select(t => extractedGame.MoveList[t.Move]).ToArray();
                        for (int i = 0; i < pokemon.TutorMoves.Length; i++)
                        {
                            var isCompatible = tutorMoves[i].Type == pokemon.Type1 || tutorMoves[i].Type == pokemon.Type2 || random.Next(0, 10) >= 8;
                            pokemon.SetTutorMoves(i, isCompatible);
                            Logger.Log($"{i + 1} - {isCompatible}\n");
                        }
                    }
                    Logger.Log($"\n");
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
