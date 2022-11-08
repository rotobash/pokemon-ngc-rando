using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Utility
{
    public enum SpecialCharacters
    {
        NewLine = 0x00,
        Special01 = 0x01,
        DialogueEnd = 0x02,
        ClearWindow = 0x03,
        KanjiStart = 0x04,
        FuriganaStart = 0x05,
        FuriganaEnd = 0x06,
        ChangeFont = 0x07, // 2bytes // 1=bold
        ChangeColourExtended = 0x08, // 5bytes (rgba)
        Pause = 0x09, // 2 bytes (second byte determines duration, likely in tenths of a second)
        Unused0A = 0x0A,
        Special0B = 0x0B,
        Special0C = 0x0C,
        Var0D = 0x0D,
        Var0E = 0x0E,
        VarPokemon0F = 0x0F,
        VarPokemon10 = 0x10,
        VarPokemon11 = 0x11,
        VarPokemon12 = 0x12,
        Player13 = 0x13, // Used in battle
        SentOutPokemon1 = 0x14,
        SentOutPokemon2 = 0x15,
        VarPokemon16 = 0x16,
        VarPokemon17 = 0x17,
        VarPokemon18 = 0x18,
        VarPokemon19 = 0x19,
        VarAbility1A = 0x1A,
        VarAbility1B = 0x1B,
        VarAbility1C = 0x1C,
        VarAbility1D = 0x1D,
        VarPokemon1E = 0x1E,
        Special1F = 0x1F,
        VarPokemon20 = 0x20,
        VarPokemon21 = 0x21,
        FoeTrainerClass = 0x22,
        FoeTrainerName = 0x23,
        Special24 = 0x24,
        VarFoe25 = 0x25,
        VarFoe26 = 0x26,
        VarFoe27 = 0x27,
        VarMove28 = 0x28,
        VarItem29 = 0x29,
        Unused2A = 0x2A,
        PlayerInField = 0x2B,
        Rui = 0x2C, // Colosseum only
        VarItem2D = 0x2D,
        VarItem2E = 0x2E,
        VarQuantity = 0x2F,
        Var30 = 0x30,
        Var31 = 0x31,
        Var32 = 0x32,
        Var33 = 0x33,
        Var34 = 0x34,
        Var35 = 0x35,
        Var36 = 0x36,
        Var37 = 0x37,
        ChangeColour = 0x38, // 2 bytes (0x0 = white, 0x1 = yellow, 0x2 = green, 0x3 = blue  0x4 = yellow, 0x5 = black)
        Var39 = 0x39,
        Unused3A = 0x3A,
        Unused3B = 0x3B,
        Unused3C = 0x3C,
        Special3D = 0x3D,
        Unused3E = 0x3E,
        Unused3F = 0x3F,
        Unused40 = 0x40,
        Special41 = 0x41,
        Special42 = 0x42,
        Special43 = 0x43,
        Special44 = 0x44,
        Special45 = 0x45,
        Special46 = 0x46,
        Special47 = 0x47,
        Unused48 = 0x48,
        Special49 = 0x49,
        Unused4A = 0x4A,
        Special4B = 0x4B,
        Special4C = 0x4C,
        SpecialMSG = 0x4D, // loads another .msg string as the variable
        VarPokemon4E = 0x4E,
        Unused4F = 0x4F,
        PokemonSpeciesCry = 0x50, // set var to a species to play it's cry as audio
        Unused51 = 0x51,
        Unused52 = 0x52, // Apparently 2 bytes but not used in any US tables afaik)
        Special53 = 0x53, // 2 bytes.
        Unused54 = 0x54,
        Special55 = 0x55,
        Special56 = 0x56,
        Special57 = 0x57,
        special58 = 0x58,
        Speaker = 0x59,
        Unused5A = 0x5A,
        Special5B = 0x5B, // 2 bytes
        Special5C = 0x5C, // 2 bytes
        Special5D = 0x5D,
        Special5E = 0x5E,
        Special5F = 0x5F,
        Unused60 = 0x60,
        Special61 = 0x61,
        Special62 = 0x62,
        Unused63 = 0x63,
        Special64 = 0x64,
        Special65 = 0x65,
        Unused66 = 0x66,
        Special67 = 0x67,
        Unused68 = 0x68,
        Special69 = 0x69,
        SetSpeaker = 0x6A,
        Unused6B = 0x6B,
        Unused6C = 0x6C,
        WaitKeyPress = 0x6D, // Dialog box won't disappear until a key is pressed. Typically used in battle.
        Special6E = 0x6E,
    }

    public static class SpecialCharacterExtensions
    {
        public static readonly byte[] k2ByteChars = new byte[] { 0x07, 0x09, 0x38, 0x52, 0x53, 0x5B, 0x5C };
        public static readonly byte[] k5ByteChars = new byte[] { 0x08 };

        const string kBold = "Bold";
        const string kNewLine = "New Line";
        const string kDialogueEnd = "Dialogue End";
        const string kClearWindow = "Clear Window";
        const string kKanjiStart = "Kanji";
        const string kFuriganaStart = "Furigana";
        const string kFuriganaEnd = "Furigana End";
        const string kChangeColourP = "Predef Colour";
        const string kChangeColourS = "Spec Colour";
        const string kPause = "Pause";
        const string kBattlePlayer = "Player B";
        const string kFieldPlayer = "Player F";
        const string kSpeaker = "Speaker";
        const string kSetSpeaker = "Set Speaker";
        const string kFoeTrainerClass = "Foe Tr Class";
        const string kFoeTrainerName = "Foe Tr Name";
        const string kWaitKeyPress = "Wait Input";
        const string kSpeciesCry = "Pokemon Cry";
        const string kspecialMSG = "MsgID";
        const string kvarPokemon4E = "Pokemon 0x4E";
        const string kvarItem2D = "Item 0x2D";
        const string kvarItem2E = "Item 0x2E";
        const string kvarQuantity = "Quantity 0x2F";
        const string kvarMove28 = "Move 0x28";
        const string kvarItem29 = "Item 0x29";
        const string kvarPokemon20 = "Pokemon 0x20";
        const string kvarPokemon21 = "Pokemon 0x21";
        const string kvarPokemon0F = "Pokemon 0x0F";
        const string kvarPokemon10 = "Pokemon 0x10";
        const string kvarPokemon11 = "Pokemon 0x11";
        const string kvarPokemon12 = "Pokemon 0x12";
        const string ksentOutPokemon1 = "Switch Pokemon 0x14";
        const string ksentOutPokemon2 = "Switch Pokemon 0x15";
        const string kvarPokemon16 = "Pokemon 0x16";
        const string kvarPokemon17 = "Pokemon 0x17";
        const string kvarPokemon18 = "Pokemon 0x18";
        const string kvarPokemon19 = "Pokemon 0x19";
        const string kvarAbility1A = "Ability 0x1A";
        const string kvarAbility1B = "Ability 0x1B";
        const string kvarAbility1C = "Ability 0x1C";
        const string kvarAbility1D = "Ability 0x1D";
        const string kvarPokemon1E = "Pokemon 0x1E";

        public static string AsString(this SpecialCharacters specialCharacters)
        {
            return specialCharacters switch
            {
                SpecialCharacters.DialogueEnd => kDialogueEnd,
                SpecialCharacters.ChangeColour => kChangeColourP,
                SpecialCharacters.ChangeColourExtended => kChangeColourS,
                SpecialCharacters.ClearWindow => kClearWindow,
                SpecialCharacters.NewLine => kNewLine,
                SpecialCharacters.Pause => kPause,
                SpecialCharacters.KanjiStart => kKanjiStart,
                SpecialCharacters.FuriganaStart => kFuriganaStart,
                SpecialCharacters.FuriganaEnd => kFuriganaEnd,
                SpecialCharacters.Player13 => kBattlePlayer,
                SpecialCharacters.PlayerInField => kFieldPlayer,
                SpecialCharacters.Speaker => kSpeaker,
                SpecialCharacters.SetSpeaker => kSetSpeaker,
                SpecialCharacters.FoeTrainerClass => kFoeTrainerClass,
                SpecialCharacters.FoeTrainerName => kFoeTrainerName,
                SpecialCharacters.WaitKeyPress => kWaitKeyPress,
                SpecialCharacters.PokemonSpeciesCry => kSpeciesCry,
                SpecialCharacters.SpecialMSG => kspecialMSG,
                SpecialCharacters.VarPokemon4E => kvarPokemon4E,
                SpecialCharacters.VarItem2D => kvarItem2D,
                SpecialCharacters.VarItem2E => kvarItem2E,
                SpecialCharacters.VarQuantity => kvarQuantity,
                SpecialCharacters.VarMove28 => kvarMove28,
                SpecialCharacters.VarItem29 => kvarItem29,
                SpecialCharacters.VarPokemon20 => kvarPokemon20,
                SpecialCharacters.VarPokemon21 => kvarPokemon21,
                SpecialCharacters.VarPokemon0F => kvarPokemon0F,
                SpecialCharacters.VarPokemon10 => kvarPokemon10,
                SpecialCharacters.VarPokemon11 => kvarPokemon11,
                SpecialCharacters.VarPokemon12 => kvarPokemon12,
                SpecialCharacters.SentOutPokemon1 => ksentOutPokemon1,
                SpecialCharacters.SentOutPokemon2 => ksentOutPokemon2,
                SpecialCharacters.VarPokemon16 => kvarPokemon16,
                SpecialCharacters.VarPokemon17 => kvarPokemon17,
                SpecialCharacters.VarPokemon18 => kvarPokemon18,
                SpecialCharacters.VarPokemon19 => kvarPokemon19,
                SpecialCharacters.VarAbility1A => kvarAbility1A,
                SpecialCharacters.VarAbility1B => kvarAbility1B,
                SpecialCharacters.VarAbility1C => kvarAbility1C,
                SpecialCharacters.VarAbility1D => kvarAbility1D,
                SpecialCharacters.VarPokemon1E => kvarPokemon1E,
                SpecialCharacters.ChangeFont => kBold,
                _ => string.Format("{0:X}", (byte)specialCharacters)
            };
        }

        public static SpecialCharacters AsSpecialCharacter(this string text)
        {
            return text switch
            {
                kDialogueEnd => SpecialCharacters.DialogueEnd,
                kChangeColourP => SpecialCharacters.ChangeColour,
                kChangeColourS => SpecialCharacters.ChangeColourExtended,
                kClearWindow => SpecialCharacters.ClearWindow,
                kNewLine => SpecialCharacters.NewLine,
                kPause => SpecialCharacters.Pause,
                kKanjiStart => SpecialCharacters.KanjiStart,
                kFuriganaStart => SpecialCharacters.FuriganaStart,
                kFuriganaEnd => SpecialCharacters.FuriganaEnd,
                kBattlePlayer => SpecialCharacters.Player13,
                kFieldPlayer => SpecialCharacters.PlayerInField,
                kSpeaker => SpecialCharacters.Speaker,
                kSetSpeaker => SpecialCharacters.SetSpeaker,
                kFoeTrainerClass => SpecialCharacters.FoeTrainerClass,
                kFoeTrainerName => SpecialCharacters.FoeTrainerName,
                kWaitKeyPress => SpecialCharacters.WaitKeyPress,
                kSpeciesCry => SpecialCharacters.PokemonSpeciesCry,
                kspecialMSG => SpecialCharacters.SpecialMSG,
                kvarPokemon4E => SpecialCharacters.VarPokemon4E,
                kvarItem2D => SpecialCharacters.VarItem2D,
                kvarItem2E => SpecialCharacters.VarItem2E,
                kvarQuantity => SpecialCharacters.VarQuantity,
                kvarMove28 => SpecialCharacters.VarMove28,
                kvarItem29 => SpecialCharacters.VarItem29,
                kvarPokemon20 => SpecialCharacters.VarPokemon20,
                kvarPokemon21 => SpecialCharacters.VarPokemon21,
                kvarPokemon0F => SpecialCharacters.VarPokemon0F,
                kvarPokemon10 => SpecialCharacters.VarPokemon10,
                kvarPokemon11 => SpecialCharacters.VarPokemon11,
                kvarPokemon12 => SpecialCharacters.VarPokemon12,
                ksentOutPokemon1 => SpecialCharacters.SentOutPokemon1,
                ksentOutPokemon2 => SpecialCharacters.SentOutPokemon2,
                kvarPokemon16 => SpecialCharacters.VarPokemon16,
                kvarPokemon17 => SpecialCharacters.VarPokemon17,
                kvarPokemon18 => SpecialCharacters.VarPokemon18,
                kvarPokemon19 => SpecialCharacters.VarPokemon19,
                kvarAbility1A => SpecialCharacters.VarAbility1A,
                kvarAbility1B => SpecialCharacters.VarAbility1B,
                kvarAbility1C => SpecialCharacters.VarAbility1C,
                kvarAbility1D => SpecialCharacters.VarAbility1D,
                kvarPokemon1E => SpecialCharacters.VarPokemon1E,
                kBold => SpecialCharacters.ChangeFont,
                _ => (SpecialCharacters)text.HexStringToInt()
            };
        }

        public static int ExtraBytes(this SpecialCharacters specialCharacters)
        {
            foreach (var byteChar in k2ByteChars)
            {
                if ((byte)specialCharacters == byteChar)
                {
                    return 1;
                }
            }
            foreach (var byteChar in k5ByteChars)
            {
                if ((byte)specialCharacters == byteChar)
                {
                    return 4;
                }
            }
            return 0;
        }

        public static int HexStringToInt(this string hex)
        {
            return Convert.ToInt32(hex);
        }
    }
}
