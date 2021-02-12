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

    [Serializable]
    public struct PokemonTraitShufflerSettings
    {
        public int RandomizeBaseStats;
        public bool StandardizeEXPCurves;
        public bool BaseStatsFollowEvolution;
        public bool UpdateBaseStats;

        public bool RandomizeAbilities;
        public bool AllowWonderGuard;
        public bool AbilitiesFollowEvolution;
        public bool BanNegativeAbilities;

        public bool RandomizeTypes;
        public bool TypesFollowEvolution;

        public bool RandomizeEvolutions;
        public bool EvolutionHasSimilarStrength;
        public bool EvolutionHasSameType;
        public bool ThreeStageEvolution;
        public bool EasyEvolutions;
        public bool FixImpossibleEvolutions;

        public MoveCompatibility TMCompatibility;
        public MoveCompatibility TutorCompatibility;

        public bool NoEXP;
        public bool RandomizeMovesets;
        public bool MetronomeOnly;
    }

    [Serializable]
    public struct TeamShufflerSettings
    {
        public bool RandomizePokemon;
        public bool DontUseLegendaries;

        public bool SetMinimumShadowCatchRate;
        public float ShadowCatchRateMinimum;
        public bool BoostTrainerLevel;
        public float BoostTrainerLevelPercent;
        public bool ForceFullyEvolved;
        public int ForceFullyEvolvedLevel;

        public bool RandomizeHeldItems;
        public bool BanBadItems;
        public bool RandomizeMovesets;
        public bool MetronomeOnly;
        public bool ForceFourMoves;
        public bool ForceGoodDamagingMoves;
        public int ForceGoodDamagingMovesCount;
    }

    [Serializable]
    public struct MoveShufflerSettings
    {
        public bool RandomMovePower;
        public bool RandomMoveAcc;
        public bool RandomMovePP;
        public bool RandomMoveTypes;
        public bool RandomMoveCategory;
    }

    [Serializable]
    public struct ItemShufflerSettings
    {
        public bool RandomizeItems;
        public bool RandomizeNonEssentialKeyItems;
        public bool RandomizeItemQuantity;
        public bool RandomizeTMs;
        public bool RandomizeTutorMoves;
        public bool RandomizeMarts;

        public bool BanBadItems;
        public bool MakeItemsSparkles;
        public bool MakeItemsBoxes;

        public bool TMForceGoodDamagingMove;
        public float TMGoodDamagingMovePercent;
        public bool TutorForceGoodDamagingMove;
        public float TutorGoodDamagingMovePercent;

        public bool MartsSellXItems;
        public bool MartsSellEvoStones;
    }

    [Serializable]
    public struct BingoCardShufflerSettings
    {
        public bool ForceStrongPokemon;
        public bool ForceGoodDamagingMove;
        public bool ForceSTABMove;
        public bool AllowTrolls;
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

    [Serializable]
    public struct StaticPokemonShufflerSettings
    {
        public StarterRandomSetting Starter;
        public string Starter1;
        public string Starter2;

        public TradeRandomSetting Trade;

        public bool RandomizeMovesets;
        public bool ForceFourMoves;
    }
}
