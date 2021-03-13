using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public static class Constants
    {
        // shamelessly copied from Stars because holy fuck have you seen how many there are??
        // kudos to him
        // class specific ones live in their respective files, still a WIP

        public const byte NameIDOffset = 0x18;
        public const byte PokemonCryIndexOffset = 0x0C;
        public const byte SpeciesNameIDOffset = 0x1C;

        public const int PokemonModelIndexOffset = 0x2E; // Same as pokemon's index
        public const int PokemonFaceIndexOffset = 0x10E; // Same as Pokemon's national dex index

        public const byte ModelDictionaryModelOffset = 0x4;

        // evolutions
        public const byte NumberOfEvolutions = 0x5;
        public const byte SizeOfEvolutionData = 0x6;

        // pokemon traits
        public const byte NumberOfPokemonMoves = 0x04;
        public const byte NumberOfEVs = 0x06;
        public const byte NumberOfIVs = 0x06;

        // moves
        public const byte SizeOfMoveData = 0x0038;
        public const int FirstTMItemIndex = 0x121;
        public const byte NumberOfTMsandHMs = 0x3A;
        public const byte NumberOfTutorMoves = 0x0C; // XD only

        public const byte NumberOfLevelUpMoves = 0x13;
        public const byte SizeOfLevelUpData = 0x4;
        public const byte LevelUpMoveLevelOffset = 0x0;
        public const byte LevelUpMoveIndexOffset = 0x2;

        public const byte MoveDisplaysTypeMatchupInSummaryScreenFlagOffset = 0x15; // XD only

        // status effects
        public const uint FirstStatusEffectOffset = 0x3f93e0;
        public const byte SizeOfStatusEffect = 0x14;
        public const byte NumberOfStatusEffects = 87;

        public const byte StatusEffectDurationOffset = 0x4;
        public const byte StatusEffectNameIDOffset = 0x10;

        // items
        public const byte SizeOfItemData = 0x28;
        public const byte NumberOfTMsAndHMs = 0x3A;
        public const byte NumberOfTMs = 0x32;

        // overworld items

        public const byte SizeOfTreasure = 0x1c;

        // xd
        public const byte NumberOfBingoCards = 0x0B;
        public const int NumberOfPokespots = 11;

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

        // Marts
        public const int MartStartIndexes = 0;
        public const int NumberOfMarts = 1;
        public const int MartGreetings = 2;
        public const int NumberOfMartGreetingSections = 3; // 0x4c bytes each
        public const int MartItems = 4;
        public const int NumberOfMartItems = 5;

        // todo: turn these into functions that take the game enum and returns the correct offset

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

        public static int AbilityStartOffset(ISO iso)
        {
            if (iso.Game == Game.XD)
            {
                switch (iso.Region)
                {
                    case Region.US:
                        return 0x3FCC50;
                    case Region.Europe:
                        return 0x437530;
                    case Region.Japan:
                        return 0x3DA310;
                }
            }
            else
            {
                switch (iso.Region)
                {
                    case Region.US:
                        return 0x35C5E0;
                    case Region.Europe:
                        return 0x3A9688;
                    case Region.Japan:
                        return 0x348D20;
                }
            }
            return 0;
        }

        public static bool AbilityListUpdated(ISO iso)
        {
            return iso.Game != Game.Colosseum && iso.DOL.ExtractedFile.GetIntAtOffset(AbilityStartOffset(iso) + 8) != 0;
        }

        public static int NumberOfAbilities(ISO iso)
        {
            return AbilityListUpdated(iso) ? 0x75 : 0x4E;
        }

        public static int AbilityNameIDOffset(ISO iso)
        {
            return AbilityListUpdated(iso) ? 0 : 4;
        }

        public static int AbilityDescriptionIDOffset(ISO iso)
        {
            return AbilityListUpdated(iso) ? 4 : 8;
        }

        public static int SizeOfAbilityEntry(ISO iso)
        {
            return AbilityListUpdated(iso) ? 8 : 12;
        }
    }
}
