using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer.Shufflers
{
    public class Settings
    {
        public PokemonTraitShufflerSettings PokemonTraitShufflerSettings { get; set; }
        public TeamShufflerSettings TeamShufflerSettings { get; set; }
        public MoveShufflerSettings MoveShufflerSettings { get; set; }
        public ItemShufflerSettings ItemShufflerSettings { get; set; }
        public StaticPokemonShufflerSettings StaticPokemonShufflerSettings { get; set; }
        public BingoCardShufflerSettings BingoCardShufflerSettings { get; set; }

    }

    public struct PokemonTraitShufflerSettings
    {
        public int RandomizeBaseStats { get; set; }
        public bool StandardizeEXPCurves { get; set; }
        public bool BaseStatsFollowEvolution { get; set; }
        public bool UpdateBaseStats { get; set; }

        public bool RandomizeAbilities { get; set; }
        public bool AllowWonderGuard { get; set; }
        public bool AbilitiesFollowEvolution { get; set; }
        public bool BanNegativeAbilities { get; set; }

        public bool RandomizeTypes { get; set; }
        public bool TypesFollowEvolution { get; set; }

        public bool RandomizeEvolutions { get; set; }
        public bool EvolutionHasSimilarStrength { get; set; }
        public bool EvolutionHasSameType { get; set; }
        public bool ThreeStageEvolution { get; set; }
        public bool EasyEvolutions { get; set; }
        public bool FixImpossibleEvolutions { get; set; }

        public MoveCompatibility TMCompatibility { get; set; }
        public MoveCompatibility TutorCompatibility { get; set; }

        public bool NoEXP { get; set; }
        public bool RandomizeMovesets { get; set; }
        public bool MetronomeOnly { get; set; }
    }

    public struct TeamShufflerSettings
    {
        public bool RandomizePokemon { get; set; }
        public bool DontUseLegendaries { get; set; }

        public bool SetMinimumShadowCatchRate { get; set; }
        public int ShadowCatchRateMinimum { get; set; }
        public bool BoostTrainerLevel { get; set; }
        public float BoostTrainerLevelPercent { get; set; }
        public bool ForceFullyEvolved { get; set; }
        public int ForceFullyEvolvedLevel { get; set; }

        public bool RandomizeHeldItems { get; set; }
        public bool BanBadItems { get; set; }
        public bool RandomizeMovesets { get; set; }
        public bool MetronomeOnly { get; set; }
        public bool ForceFourMoves { get; set; }
        public bool ForceGoodDamagingMoves { get; set; }
        public int ForceGoodDamagingMovesCount { get; set; }
    }

    public struct MoveShufflerSettings
    {
        public bool RandomMovePower { get; set; }
        public bool RandomMoveAcc { get; set; }
        public bool RandomMovePP { get; set; }
        public bool RandomMoveTypes { get; set; }
        public bool RandomMoveCategory { get; set; }
    }

    public struct ItemShufflerSettings
    {
        public bool RandomizeItems { get; set; }
        public bool RandomizeNonEssentialKeyItems { get; set; }
        public bool RandomizeItemQuantity { get; set; }
        public bool RandomizeTMs { get; set; }
        public bool RandomizeTutorMoves { get; set; }
        public bool RandomizeMarts { get; set; }

        public bool BanBadItems { get; set; }
        public bool MakeItemsSparkles { get; set; }
        public bool MakeItemsBoxes { get; set; }

        public bool TMForceGoodDamagingMove { get; set; }
        public float TMGoodDamagingMovePercent { get; set; }
        public bool TutorForceGoodDamagingMove { get; set; }
        public float TutorGoodDamagingMovePercent { get; set; }

        public bool MartsSellXItems { get; set; }
        public bool MartsSellEvoStones { get; set; }
    }

    public struct BingoCardShufflerSettings
    {
        public bool ForceStrongPokemon { get; set; }
        public bool ForceGoodDamagingMove { get; set; }
        public bool ForceSTABMove { get; set; }
        public bool AllowTrolls { get; set; }
    }
    public enum MoveCompatibility
    {
        Unchanged,
        RandomPreferType,
        Random,
        Full
    }

    public enum StarterRandomSetting
    {
        Unchanged,
        Custom,
        Random,
        RandomThreeStage,
        RandomTwoStage,
        RandomSingleStage
    }
    public enum TradeRandomSetting
    {
        Unchanged,
        Given,
        Both,
    }

    public struct StaticPokemonShufflerSettings
    {
        public StarterRandomSetting Starter { get; set; }
        public string Starter1 { get; set; }
        public string Starter2 { get; set; }

        public TradeRandomSetting Trade { get; set; }

        public bool RandomizeMovesets { get; set; }
        public bool ForceFourMoves { get; set; }
    }
}
