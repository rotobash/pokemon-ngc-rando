using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
