﻿using XDCommon.PokemonDefinitions;

namespace Randomizer.Shufflers
{
    public class ShuffleSettings
    {
        public Settings RandomizerSettings { get; set; }
        public AbstractRNG RNG { get; set; }
        public ExtractedGame ExtractedGame { get; set; }
    }


    public class Settings
    {
        public PokemonTraitShufflerSettings PokemonTraitShufflerSettings { get; set; }
        public TeamShufflerSettings TeamShufflerSettings { get; set; }
        public MoveShufflerSettings MoveShufflerSettings { get; set; }
        public ItemShufflerSettings ItemShufflerSettings { get; set; }
        public StaticPokemonShufflerSettings StaticPokemonShufflerSettings { get; set; }
        public BingoCardShufflerSettings BingoCardShufflerSettings { get; set; }
        public PokeSpotShufflerSettings PokeSpotShufflerSettings { get; set; }
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
        public bool EvolutionLinesEndRandomly { get; set; }
        public bool EvolutionHasSimilarStrength { get; set; }
        public bool EvolutionHasSameType { get; set; }
        public bool ThreeStageEvolution { get; set; }
        public bool EasyEvolutions { get; set; }
        public bool FixImpossibleEvolutions { get; set; }

        public MoveCompatibility TMCompatibility { get; set; }
        public MoveCompatibility TutorCompatibility { get; set; }
        public RandomMoveSetOptions MoveSetOptions { get; set; }

        public bool NoEXP { get; set; }
    }

    public struct TeamShufflerSettings
    {
        public bool RandomizePokemon { get; set; }
        public bool UseSimilarBSTs { get; set; }
        public bool DontUseLegendaries { get; set; }
        public bool NoDuplicateShadows { get; set; }
        public bool RandomizeLegendaryIntoLegendary { get; set; }

        public CatchRateAdjustmentType CatchRateAdjustment { get; set; }
        public int ShadowCatchRateMinimum { get; set; }
        public bool BoostTrainerLevel { get; set; }
        public float BoostTrainerLevelPercent { get; set; }
        public bool ForceFullyEvolved { get; set; }
        public int ForceFullyEvolvedLevel { get; set; }

        public bool RandomizeHeldItems { get; set; }
        public bool BanBadItems { get; set; }
        public bool BanBattleCDs { get; set; }
        public RandomMoveSetOptions MoveSetOptions { get; set; }
    }

    public struct PokeSpotShufflerSettings
    {
        public bool RandomizePokeSpotPokemon { get; set; }
        public bool SetMinimumCatchRate { get; set; }
        public int MinimumCatchRate { get; set; }
        public bool BoostPokeSpotLevel { get; set; }
        public float BoostPokeSpotLevelPercent { get; set; }

        public bool UseSimilarBSTs { get; set; }
        public bool RandomizeHeldItems { get; set; }
        public bool BanBadHeldItems { get; set; }
        public bool EasyBonsly { get; set; }
    }

    public struct MoveShufflerSettings
    {
        public bool RandomMovePower { get; set; }
        public bool RandomMoveAcc { get; set; }
        public bool IgnoreOHKOMoveAcc { get; set; }
        public bool RandomMovePP { get; set; }
        public bool RandomMoveTypes { get; set; }
        public bool RandomMoveCategory { get; set; }
        public MoveAnimationType MoveAnimationType { get; set; } // use enum for now, but 
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
        public bool BanBattleCDs { get; set; }
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
        public bool RandomizeBattleBingoPokemon { get; set; }
        public bool RandomizeBattleBingoMoveSets { get; set; }
        public bool ForceStrongPokemon { get; set; }
        public bool ForceGoodDamagingMove { get; set; }
        public bool ForceSTABMove { get; set; }
        public bool AllowTrolls { get; set; }
        public bool BanShadowMoves { get; set; }
    }

    public struct StaticPokemonShufflerSettings
    {
        public StarterRandomSetting Starter { get; set; }
        public string Starter1 { get; set; }
        public string Starter2 { get; set; }

        public TradeRandomSetting Trade { get; set; }
        public RandomMoveSetOptions MoveSetOptions { get; set; }
        public bool UsePokeSpotPokemonInTrade { get; set; }
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
        Requested,
        Both,
    }

    public enum CatchRateAdjustmentType 
    {
        Unchanged, 
        Minimum, 
        True, 
        Adjusted
    }
    public enum MoveAnimationType
    {
        Unchanged,
        None,
        Random
    }
}
