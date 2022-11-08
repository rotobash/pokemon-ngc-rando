using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class BattleBingoPokemon
    {
        const byte BattleBingoPokemonPanelTypeOffset = 0x00;
        const byte BattleBingoPokemonAbilityOffset = 0x01;
        const byte BattleBingoPokemonNatureOffset = 0x02;
        const byte BattleBingoPokemonGenderOffset = 0x03;
        const byte BattleBingoPokemonSpeciesOffset = 0x04;
        const byte BattleBingoPokemonMoveOffset = 0x06;

        ISO iso;
        public BattleBingoPokemon(int index, ISO iso)
        {
            StartOffset = index;
            this.iso = iso;
        }

        public int StartOffset { get; }

        public ushort Pokemon
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + BattleBingoPokemonSpeciesOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + BattleBingoPokemonSpeciesOffset, value.GetBytes());
        }
        public ushort Move
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + BattleBingoPokemonMoveOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + BattleBingoPokemonMoveOffset, value.GetBytes());
        }
        public Natures Nature
        {
            get => (Natures)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + BattleBingoPokemonNatureOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + BattleBingoPokemonNatureOffset, (byte)value);
        }
        public Genders Gender
        {
            get => (Genders)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + BattleBingoPokemonGenderOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + BattleBingoPokemonGenderOffset, (byte)value);
        }

        // always 0, why?
        public byte Ability
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + BattleBingoPokemonAbilityOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + BattleBingoPokemonAbilityOffset, value);
        }
        public byte TypeOnCard
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + BattleBingoPokemonPanelTypeOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + BattleBingoPokemonPanelTypeOffset, (byte)value);
        }
    }
}
