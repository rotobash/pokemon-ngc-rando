namespace XDCommon.PokemonDefinitions.XD.SaveData
{
	public enum PokeSpotFlagTypes
	{
		Rock = 1,
        Oasis = 2,
        Cave = 3
    }

    public enum FlagsTypes
    {
		BerryMasterVisited = 4,
		StepsWalkedSinceLastBerryMasterVisit = 5,

		Story = 964,

		DayCareVisited = 1124,

		MirorBHasBeenEncountered = 1191,

		CurrentPokespotSnacksRock = 1248,
		CurrentPokespotSnacksOasis = 1249,
		CurrentPokespotSnacksCave = 1250,

		CurrentPokespotSpawnRock = 1407,
		CurrentPokespotSpawnOasis = 1408,
		CurrentPokespotSpawnCave = 1409,

		MirorbNoDespawn = 1415, // miror radar won't lost signal while this is set

		MtBattleHighestClearedZone = 1433,

		MirorBStepsWalked = 1449,

		MirorbLocation = 1452,

		MtBattleCurrentZone = 1478,

		MtBattleCanBeChallenged = 1487,

		PyriteColosseumWins = 1754
    }
}
