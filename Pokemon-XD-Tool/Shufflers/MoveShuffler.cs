﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.PokemonDefinitions;

namespace Randomizer.Shufflers
{
    public static class MoveShuffler
    {
        public static void RandomizeMoves(Random random, MoveShufflerSettings settings, ExtractedGame extractedGame)
        {
            foreach (var move in extractedGame.MoveList)
            {
                if (settings.RandomMovePower && move.BasePower > 0)
                {
                    // sample move power with the power of math!!
                    // basically moves will average at 80 power but can be from 10 to 150
                    move.BasePower = (byte)random.Sample(80, 70);
                }

                if (settings.RandomMoveAcc && move.Accuracy > 0)
                {
                    // randomize accuracy, repick if the moves is OHKO and 100% accurate cause that's a yikes
                    byte acc;
                    do 
                    {
                        acc = (byte)Math.Min(100, random.Sample(80, 30));
                    } while (acc == 100 && move.EffectType == MoveEffectTypes.OHKO);
                    move.Accuracy = acc;
                }

                if (settings.RandomMovePP)
                {
                    // pick from 1, 8, multiply by 5 to get the 5 - 40 range for PP
                    move.PP = (byte)Math.Max(5, Math.Round(random.Sample(4, 4)) * 5);
                }

                if (settings.RandomMoveTypes && move.Type != PokemonTypes.None)
                {
                    var typesCount = Enum.GetValues<PokemonTypes>();
                    PokemonTypes newType;
                    // pick new move type but not the none type
                    do
                    {
                        newType = typesCount[random.Next(0, typesCount.Length)];
                    }
                    while (newType == PokemonTypes.None);
                    move.Type = newType;
                }

                // I don't know if this will work for colo, probably not
                if (settings.RandomMoveCategory && move.Category != MoveCategories.None)
                {
                    var newCategory = (MoveCategories)random.Next(1, 3);
                    move.Category = newCategory;
                }
            }
        }

        public static ushort[] GetRandomMoveset(Random random, bool banShadowMoves, bool preferType, int minimumGoodMoves, ushort pokemon, ExtractedGame extractedGame)
        {
            var poke = extractedGame.PokemonList[pokemon];
            var moveSet = new HashSet<ushort>();
            var moveFilter = banShadowMoves ? extractedGame.MoveList.Where(m => !m.IsShadowMove) : extractedGame.MoveList;

            var typeFilter = moveFilter;
            if (preferType)
            {
                // allow 20% chance for move to not be same type
                typeFilter = typeFilter.Where(m => m.Type == poke.Type1 || m.Type == poke.Type2 || random.Next(0, 10) >= 8).ToArray();
                if (!moveFilter.Any())
                    typeFilter = moveFilter;
            }

            var potentialMoves = typeFilter.ToArray();
            if (minimumGoodMoves > 0)
            {
                var goodMoves = potentialMoves.Where(m => m.BasePower >= Configuration.GoodDamagingMovePower).ToArray();
                while (moveSet.Count < minimumGoodMoves)
                {
                    var newMove = extractedGame.MoveList[random.Next(0, goodMoves.Length)];
                    moveSet.Add((ushort)newMove.MoveIndex);
                }
            }

            while (moveSet.Count < Constants.NumberOfPokemonMoves)
            {
                var newMove = extractedGame.MoveList[random.Next(0, potentialMoves.Length)];
                moveSet.Add((ushort)newMove.MoveIndex);
            }

            return moveSet.ToArray();
        }

        public static ushort[] GetLevelUpMoveset(Random random, ushort pokemon, ushort level, bool forceFourMoves, bool banShadowMoves, ExtractedGame extractedGame)
        {
            var moveSet = new HashSet<ushort>();
            var potentialMoves = banShadowMoves ? extractedGame.MoveList.Where(m => !m.IsShadowMove).ToArray() : extractedGame.MoveList;

            // not randomizing moves? pick level up moves then
            foreach (var levelUpMove in extractedGame.PokemonList[pokemon].CurrentLevelMoves(level))
            {
                moveSet.Add(levelUpMove.Move);
            }

            if (forceFourMoves && moveSet.Count < Constants.NumberOfPokemonMoves)
            {
                var total = moveSet.Count;
                while (moveSet.Count < Constants.NumberOfPokemonMoves)
                {
                    var newMove = potentialMoves[random.Next(0, potentialMoves.Length)];
                    moveSet.Add((ushort)newMove.MoveIndex);
                }
            }

            return moveSet.ToArray();
        }
    }
}
