using System;
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

    public class MoveShuffler
    {
        Random random;
        Move[] moves;

        public MoveShuffler(Random random, Move[] moveList)
        {
            this.random = random;
            moves = moveList;
        }

        public void RandomizeMoves(MoveShufflerSettings settings)
        {
            foreach (var move in moves)
            {
                if (settings.RandomMovePower && move.BasePower > 0)
                {
                    move.BasePower = (byte)random.Sample(80, 40);
                }

                if (settings.RandomMoveAcc && move.EffectType != MoveEffectTypes.OHKO)
                {
                    move.Accuracy = (byte)random.Next(0, 256);
                }

                if (settings.RandomMovePP)
                {
                    move.PP = (byte)(random.Next(1, 8) * 5);
                }

                if (settings.RandomMoveTypes && move.Type != PokemonTypes.None)
                {
                    var typesCount = Enum.GetValues<PokemonTypes>();
                    PokemonTypes newType;
                    do
                    {
                        newType = typesCount[random.Next(0, typesCount.Length)];
                    }
                    while (newType != PokemonTypes.None);
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
