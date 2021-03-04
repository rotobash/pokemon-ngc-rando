using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.PokemonDefinitions;

namespace Randomizer.Shufflers
{
    public struct RandomMoveSetOptions
    {
        public bool RandomizeMovesets { get; set; }
        public bool MetronomeOnly { get; set; }
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
        public static void RandomizeMoves(Random random, MoveShufflerSettings settings, ExtractedGame extractedGame)
        {
            Logger.Log("=============================== Moves ===============================\n\n");
            foreach (var move in extractedGame.ValidMoves)
            {

                Logger.Log($"{move.Name}: \n");
                if (settings.RandomMovePower && move.BasePower > 0)
                {
                    // sample move power with the power of math!!
                    // basically moves will average at 80 power but can be from 10 to 150
                    move.BasePower = (byte)random.Sample(80, 70);
                }
                Logger.Log($"Power: {move.BasePower}\n");

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
                Logger.Log($"Accuracy: {move.Accuracy}\n");

                if (settings.RandomMovePP)
                {
                    // pick from 1, 8, multiply by 5 to get the 5 - 40 range for PP
                    move.PP = (byte)Math.Max(5, Math.Round(random.Sample(4, 4)) * 5);
                }
                Logger.Log($"PP: {move.PP}\n");

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
                Logger.Log($"Type: {move.Type}\n");

                // I don't know if this will work for colo, probably not
                if (settings.RandomMoveCategory && move.Category != MoveCategories.None)
                {
                    var newCategory = (MoveCategories)random.Next(1, 3);
                    move.Category = newCategory;
                }
                Logger.Log($"Category: {move.Category}\n\n");
            }
        }

        public static ushort[] GetNewMoveset(Random random, RandomMoveSetOptions options, ushort pokemon, ushort level, ExtractedGame extractedGame)
        {
            var poke = extractedGame.PokemonList[pokemon];
            var moveSet = new HashSet<ushort>();
            var moveFilter = options.BanShadowMoves ? extractedGame.ValidMoves.Where(m => !m.IsShadowMove) : extractedGame.ValidMoves;
            if (options.BanEarlyDragonRage)
            {
                moveFilter = moveFilter.Where(m => !(m.MoveIndex == RandomizerConstants.DragonRageIndex && level < RandomizerConstants.BanDragonRageUnderLevel));
            }

            var typeFilter = moveFilter;
            if (options.PreferType)
            {
                // allow 20% chance for move to not be same type
                typeFilter = typeFilter.Where(m => m.Type == poke.Type1 || m.Type == poke.Type2 || random.Next(0, 10) >= 8).ToArray();
                if (!moveFilter.Any())
                    typeFilter = moveFilter;
            }

            var potentialMoves = typeFilter.ToArray();

            if (options.UseLevelUpMoves || !options.RandomizeMovesets)
            {
                // not randomizing moves? pick level up moves then
                var levelUpMoves = extractedGame.PokemonList[pokemon].CurrentLevelMoves(level++);

                // this *could* happen so increase the level until there's at least one move for them
                while (!levelUpMoves.Any() && level <= 100)
                    levelUpMoves = extractedGame.PokemonList[pokemon].CurrentLevelMoves(level++);

                // still nothing, add a random move
                if (!levelUpMoves.Any())
                {
                    var newMove = potentialMoves[random.Next(0, potentialMoves.Length)];
                    moveSet.Add((ushort)newMove.MoveIndex);
                }
                else
                {
                    foreach (var levelUpMove in levelUpMoves)
                    {
                        moveSet.Add(levelUpMove.Move);
                    }
                }
            }
            else if (options.MinimumGoodMoves > 0)
            {
                var goodMoves = potentialMoves.Where(m => m.BasePower >= Configuration.GoodDamagingMovePower).ToArray();
                while (moveSet.Count < options.MinimumGoodMoves)
                {
                    var newMove = goodMoves[random.Next(0, goodMoves.Length)];
                    moveSet.Add((ushort)newMove.MoveIndex);
                }
            }

            if (options.RandomizeMovesets || options.ForceFourMoves)
            {
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
