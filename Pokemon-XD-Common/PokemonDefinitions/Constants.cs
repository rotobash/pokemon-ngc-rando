using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.PokemonDefinitions
{
    public static class Constants
    {
        // pokemon
        public const byte PokemonNameIDOFfset = 0x18;
        public const int SizeOfPokemonStats = 0x124;
        //public const byte FirstPokemonOffset		= 0x29DA8;

        public const byte EXPRateOffset = 0x00;
        public const byte CatchRateOffset = 0x01;
        public const byte GenderRatioOffset = 0x02;
        public const byte BaseEXPOffset = 0x05;
        public const byte BaseHappinessOffset = 0x07;

        public const byte HeightOffset = 0x08;
        public const byte WeightOffset = 0x0A;

        public const byte NationalIndexOffset = 0x0E;

        public const byte Type1Offset = 0x30;
        public const byte Type2Offset = 0x31;

        public const byte Ability1Offset = 0x32;
        public const byte Ability2Offset = 0x33;

        public const byte FirstTMOffset = 0x34;
        public const byte FirstEVYieldOffset = 0x9A; // 1 byte between each one.
        public const byte FirstEvolutionOffset = 0xA6;
        public const byte FirstTutorMoveOffset = 0x6E;
        public const byte FirstLevelUpMoveOffset = 0xC4;
        public const byte FirstEggMoveOffset = 0x7E;

        public const byte HeldItem1Offset = 0x7A;
        public const byte HeldItem2Offset = 0x7C;

        public const byte HPOffset = 0x8F;
        public const byte AttackOffset = 0x91;
        public const byte DefenseOffset = 0x93;
        public const byte SpecialAttackOffset = 0x95;
        public const byte SpecialDefenseOffset = 0x97;
        public const byte SpeedOffset = 0x99;

        public const byte NameIDOffset = 0x18;
        public const byte PokemonCryIndexOffset = 0x0C;
        public const byte SpeciesNameIDOffset = 0x1C;

        public const int PokemonModelIndexOffset = 0x2E; // Same as pokemon's index
        public const int PokemonFaceIndexOffset = 0x10E; // Same as Pokemon's national dex index

        public const byte ModelDictionaryModelOffset = 0x4;

        // evolutions
        public const byte NumberOfEvolutions = 0x5;
        public const byte SizeOfEvolutionData = 0x6;
        public const byte EvolutionMethodOffset = 0x0;
        public const byte EvolutionConditionOffset = 0x2;
        public const byte EvovledFormOffset = 0x4;

        //trainer poke
        public const byte SizeOfPokemonData = 0x20;
        public const byte NumberOfPokemonMoves = 0x04;
        public const byte NumberOfEVs = 0x06;
        public const byte NumberOfIVs = 0x06;

        public const byte PokemonIndexOffset = 0x00;
        public const byte PokemonLevelOffset = 0x02;
        public const byte PokemonHappinessOffset = 0x03;
        public const byte PokemonItemOffset = 0x04;
        public const byte FirstPokemonIVOffset = 0x08;
        public const byte FirstPokemonEVOffset = 0x0E;
        public const byte FirstPokemonMoveOffset = 0x14;
        public const byte PokemonPriority1Offset = 0x1D; // priority? in vanilla but moved by stars in xg
        public const byte PokemonPIDOffset = 0x1E;
        public const byte PokemonGenRandomOffset = 0x1F; // If value is set to 1 then the pokemon is generated with a random nature and gender. Always 0 in vanilla and unused in XG

        // added by stars by modifying dol behaviour
        public const byte PokemonshinynessOffset = 0x1C;
        public const byte PokemonPriority2Offset = 0x1F;

        public const byte SizeOfShadowData = 0x18;

        public const byte FleeAfterBattleOffset = 0x00; // 0 = no flee. Other values probably chances of finding with mirorb. Higher value = more common encounter.
        public const byte ShadowCatchRateOFfset = 0x01; // this catch rate overrides the species' catch rate
        public const byte ShadowLevelOffset = 0x02; // the pokemon's level after it's caught. Regular level can be increased so AI shadows are stronger
        public const byte ShadowInUseFlagOffset = 0x03; // flags for whether pokemon is seen/caught/purified etc. default 0x80 and updated in save file
        public const byte ShadowStoryIndexOffset = 0x06; // dpkm index of pokemon data in deck story
        public const byte ShadowCounterOffset = 0x08; // the starting value of the heart gauge
        public const byte FirstShadowMoveOFfset = 0x0C;
        public const byte ShadowAggressionOffset = 0x14; // determines how often it enters reverse mode
        public const byte ShadowAlwaysFleeOffset = 0x15; // the shadow pokemon is sent to miror b. even if you lose the battle

        public const byte PurificationExperienceOffset = 0xA; // Should always be 0. The value gets increased as the pokemon gains exp and it is all gained at once upon purification.


        // moves
        public const byte SizeOfMoveData = 0x0038;

        public const byte NumberOfTMsandHMs = 0x3A;

        public const byte NumberOfTutorMoves = 0x0C; // XD only

        public const byte NumberOfLevelUpMoves = 0x13;
        public const byte SizeOfLevelUpData = 0x4;
        public const byte LevelUpMoveLevelOffset = 0x0;
        public const byte LevelUpMoveIndexOffset = 0x2;

        public const byte PriorityOffset = 0x00;
        public const byte PPOffset = 0x01;
        public const byte MoveTypeOffset = 0x02;
        public const byte TargetsOffset = 0x03;
        public const byte AccuracyOffset = 0x04;
        public const byte EffectAccuracyOffset = 0x05;

        public const byte ContactFlagOffset = 0x06;
        public const byte ProtectFlagOffset = 0x07;
        public const byte MagicCoatFlagOffset = 0x08;
        public const byte SnatchFlagOffset = 0x09;
        public const byte MirrorMoveFlagOffset = 0x0A;
        public const byte KingsRockFlagOffset = 0x0B;
        public const byte SoundBasedFlagOffset = 0x10;
        public const byte HMFlagOffset = 0x12;

        public const byte MoveNameIDOffset = 0x20;
        public const byte MoveDescriptionIDOffset = 0x2C;
        public const byte Animation2IndexOffset = 0x32;

        public const byte MoveEffectTypeOffset = 0x34; // used by AI

        public const byte MoveDisplaysTypeMatchupInSummaryScreenFlagOffset = 0x15; // XD only

        // status effects
        public const uint FirstStatusEffectOffset = 0x3f93e0;
        public const byte SizeOfStatusEffect = 0x14;
        public const byte NumberOfStatusEffects = 87;

        public const byte StatusEffectDurationOffset = 0x4;
        public const byte StatusEffectNameIDOffset = 0x10;

        // xd
        public const int BattleBingo = 0;
        public const int NumberOfBingoCards = 1;
        public const int numberOfPokespots = 11;
        public const int PokespotRock = 12;
        public const int PokespotRockEntries = 13;
        public const int PokespotOasis = 15;
        public const int PokespotOasisEntries = 16;
        public const int PokespotCave = 18;
        public const int PokespotCaveEntries = 19;
        public const int PokespotAll = 21;
        public const int PokespotAllEntries = 22;
        public const int BattleCDs = 24;
        public const int NumberBattleCDs = 25;
        public const int BattleFields = 28;
        public const int NumberOfBattleFields = 29;
        public const int BattleLayouts = 42; // eg 2 trainers per side, 1 active pokemon per trainer, 6 pokemon per trainer
        public const int NumberOfBattleLayouts = 43;
        public const int Flags = 44;
        public const int NumberOfFlags = 45;
        public const int RoomBGM = 64; // byte 1 = volume, short 2 = room id, short 4 = bgm id
        public const int NumberOfRoomBGMs = 65;

        public const int ValidItems = 68; // list of items which are actually available in XD
        public const int TotalNumberOfItems = 69;
        public const int Items = 70;
        public const int NumberOfItems = 71;
        public const int SoundsMetaData = 102;
        public const int NumberOfSounds = 103;
        public const int BGM = 104;
        public const int NumberBGM = 105;


        public const int stringTable1 = 116;
        public const int TutorMoves = 126;
        public const int Types = 130;
        public const int NumberOfTypes = 13;

        public const int NumberOfBGMIDs = -4;


        public const int LegendaryPokemon = 2;
        public const int NumberOfLegendaryPokemon = 3;

        public const int PokefaceTextures = 4;

        public const int Trainers = 44;
        public const int NumberOfTrainers = 45;
        public const int TrainerAIData = 46;
        public const int NumberOfTrainerAIData = 47;
        public const int TrainerPokemonData = 48;
        public const int NumberOfTrainerPokemonData = 49;

        public const int MusicSamples = 52; // 8bytes each. 0-3 file identifier, 6-7 unknown identifier
        public const int NumberOfMusicSamples = 53;

        public const int BattleDebugScenarios = 56;
        public const int NumberOfBattleDebugScenarios = 57;
        public const int AIDebugScenarios = 58;
        public const int NumberOfAIDebugScenarios = 59;

        public const int StoryDebugOptions = 32;
        public const int NumberOfStoryDebugOptions = 33;

        public const int KeyboardCharacters = 36;
        public const int NumberOfKeyboardCharacters = 37;
        public const int Keyboard2Characters = 38;
        public const int NumberOfKeyboard2Characters = 39;
        public const int Keyboard3Characters = 40; // main keyboard
        public const int NumberOfKeyboard3Characters = 41;

        public const int BattleStyles = 42;
        public const int NumberOfBattleStyles = 43;

        public const int RoomData = 28;
        public const int NumberOfRoomData = 29;

        public const int ShadowData = 80;
        public const int NumberOfShadowPokemon = 81;

        public const int PokemonMetLocations = 82;
        public const int NumberOfMetLocations = 83;

        public const int StringTableB = 101;

        // XD
        public const int XDNumberRelPointers = 0x84;
        public const int XDPeopleIDs = 2; // 2 bytes at offset 0 person id 4 bytes at offset 4 string id for character name;
        public const int XDNumberOfPeopleIDs = 3;
        public const int XDTrainerClasses = 38;
        public const int XDNumberOfTrainerClasses = 39;

        public const int XDBattles = 26;
        public const int XDNumberOfBattles = 27;
        public const int XDRooms = 58; // same as maps;
        public const int XDNumberOfRooms = 59;
        public const int XDDoors = 60; // doors that open when player is near
        public const int XDNumberOfDoors = 61;
        public const int XDTreasureBoxData = 66; // 0x1c bytes each;
        public const int XDNumberTreasureBoxes = 67;

        public const int XDInteractionPoints = 62; // warps and inanimate objects
        public const int XDNumberOfInteractionPoints = 63;
        public const int XDCharacterModels = 84;
        public const int XDNumberOfCharacterModels = 85;
        public const int XDPokemonStats = 88;
        public const int XDNumberOfPokemon = 89;
        public const int XDNatures = 94;
        public const int XDNumberOfNatures = 95;

        public const int XDMoves = 124;
        public const int XDNumberOfMoves = 125;

        // COLO
        public const int ColNumberRelPointers = 0x6C;
        public const int ColPeopleIDs = 6; // 2 bytes at offset 0 person id 4 bytes at offset 4 string id for character name
        public const int ColNumberOfPeopleIDs = 7;

        public const int ColTrainerClasses = 24;
        public const int ColNumberOfTrainerClasses = 25;

        public const int ColDoors = 30;
        public const int ColNumberOfDoors = 31;
        public const int ColBattles = 50;
        public const int ColNumberOfBattles = 51;
        public const int ColRooms = 14;
        public const int ColNumberOfRooms = 15;
        public const int ColTreasureBoxData = 60;
        public const int ColNumberTreasureBoxes = 61;
        public const int ColCharacterModels = 72;
        public const int ColNumberOfCharacterModels = 73;
        public const int ColInteractionPoints = 86; // warps and inanimate objects
        public const int ColNumberOfInteractionPoints = 87;
        public const int ColPokemonStats = 68;
        public const int ColNumberOfPokemon = 69;
        public const int ColNatures = 64;
        public const int ColNumberOfNatures = 65;
        public const int ColMoves = 62;
        public const int ColNumberOfMoves = 6;
    }
}
