using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.PokemonDefinitions;
using XDCommon.Utility.Extensions;

namespace XDCommon.Shufflers
{
    public struct RandomMoveSetOptions
    {
        public bool RandomizeMovesets { get; set; }
        public bool MetronomeOnly { get; set; }
        public bool LegalMovesOnly { get; set; }
        public bool UseLevelUpMoves { get; set; }
        public bool BanShadowMoves { get; set; }
        public bool BanEarlyDragonRage { get; set; }
        public bool PreferType { get; set; }
        public bool ForceFourMoves { get; set; }
        public bool ForceGoodMoves { get; set; }
        public int MinimumGoodMoves { get; set; }
    }

    public static class MoveShuffler
    {
        public static void RandomizeMoves(ShuffleSettings shuffleSettings)
        {
            var settings = shuffleSettings.RandomizerSettings.MoveShufflerSettings;
            var extractedGame = shuffleSettings.ExtractedGame;
            var random = shuffleSettings.RNG;

            Logger.Log("=============================== Moves ===============================\n\n");

            if (settings.MoveAnimationType != MoveAnimationType.Unchanged)
            {
                Logger.Log("Turning off move animations.\n");
            }

            foreach (var move in extractedGame.ValidMoves)
            {
                switch (settings.MoveAnimationType)
                {
                    case MoveAnimationType.None:
                        move.AnimationID = 0;
                        break;
                    default:
                        break;
                }

                Logger.Log($"{move.Name}\n");
                if (settings.RandomMovePower && move.BasePower > 0)
                {
                    // sample move power with the power of math!!
                    // basically moves will average at 80 power but can be from 10 to 150
                    move.BasePower = (byte)random.Sample(80, 70);
                    Logger.Log($"Power: {move.BasePower}\n");
                }

                if (settings.RandomMoveAcc && move.Accuracy > 0)
                {
                    if (settings.IgnoreOHKOMoveAcc && move.EffectType == MoveEffectTypes.OHKO)
                        continue;

                    // randomize accuracy, repick if the moves is OHKO and 100% accurate cause that's a yikes
                    byte acc;
                    do
                    {
                        acc = (byte)Math.Min(100, random.Sample(80, 30));
                    } while (acc == 100 && move.EffectType == MoveEffectTypes.OHKO);
                    move.Accuracy = acc;
                    Logger.Log($"Accuracy: {move.Accuracy}\n");
                }

                if (settings.RandomMovePP)
                {
                    // pick from 1, 8, multiply by 5 to get the 5 - 40 range for PP
                    move.PP = (byte)Math.Max(5, Math.Round(random.Sample(4, 4)) * 5);
                    Logger.Log($"PP: {move.PP}\n");
                }

                if (settings.RandomMoveTypes && move.Type != PokemonTypes.None)
                {
                    var typesCount = EnumExtensions.GetValues<PokemonTypes>();
                    PokemonTypes newType;
                    // pick new move type but not the none type
                    do
                    {
                        newType = typesCount[random.Next(0, typesCount.Length)];
                    }
                    while (newType == PokemonTypes.None);
                    move.Type = newType;
                    Logger.Log($"Type: {move.Type}\n");
                }

                // I don't know if this will work for colo, probably not
                if (settings.RandomMoveCategory && move.Category != MoveCategories.None)
                {
                    var newCategory = (MoveCategories)random.Next(1, 3);
                    move.Category = newCategory;
                    Logger.Log($"Category: {move.Category}\n\n");
                }
            }
        }
    }
}
