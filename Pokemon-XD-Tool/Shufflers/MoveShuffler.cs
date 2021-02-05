﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;

namespace Randomizer.Shufflers
{
    public struct MoveShufflerSettings
    {
        public bool RandomMovePower;
        public bool RandomMoveAcc;
        public bool RandomMovePP;
        public bool RandomMoveTypes;
        public bool RandomMoveCategory;
    }

    public static class MoveShuffler
    {
        public static void RandomizeMoves(Random random, Move[] moves, MoveShufflerSettings settings)
        {
            foreach (var move in moves)
            {
                if (settings.RandomMovePower && move.BasePower > 0)
                {
                    move.BasePower = (byte)random.Sample(90, 70);
                }

                if (settings.RandomMoveAcc && move.Accuracy > 0)
                {
                    byte acc;
                    do 
                    {
                        acc = (byte)Math.Min(100, random.Sample(90, 30));
                    } while (move.Accuracy == 100 && move.EffectType == MoveEffectTypes.OHKO);
                    move.Accuracy = acc;
                }

                if (settings.RandomMovePP)
                {
                    move.PP = (byte)Math.Max(5, random.Sample(4, 4) * 5);
                }

                if (settings.RandomMoveTypes && move.Type != PokemonTypes.None)
                {
                    var typesCount = Enum.GetValues<PokemonTypes>();
                    PokemonTypes newType;
                    do
                    {
                        newType = typesCount[random.Next(0, typesCount.Length)];
                    }
                    while (newType == PokemonTypes.None);
                    move.Type = newType;
                }

                if (settings.RandomMoveCategory && move.Category != MoveCategories.None)
                {
                    var newCategory = (MoveCategories)random.Next(1, 3);
                    move.Category = newCategory;
                }
            }
        }
    }
}