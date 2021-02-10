using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class BattleBingoPokemon
    {
        ISO iso;
        public BattleBingoPokemon(int index, ISO iso)
        {
            StartOffset = index;
            this.iso = iso;
        }

        public int StartOffset { get; }

        public ushort Pokemon
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.BattleBingoPokemonSpeciesOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.BattleBingoPokemonSpeciesOffset, value.GetBytes());
        }
        public ushort Move
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.BattleBingoPokemonMoveOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.BattleBingoPokemonMoveOffset, value.GetBytes());
        }
        public Natures Nature
        {
            get => (Natures)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.BattleBingoPokemonNatureOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.BattleBingoPokemonNatureOffset, (byte)value);
        }
        public Genders Gender
        {
            get => (Genders)iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.BattleBingoPokemonGenderOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.BattleBingoPokemonGenderOffset, (byte)value);
        }

        // always 0, why?
        public byte Ability
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.BattleBingoPokemonAbilityOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.BattleBingoPokemonAbilityOffset, value);
        }
        public byte TypeOnCard
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.BattleBingoPokemonPanelTypeOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.BattleBingoPokemonPanelTypeOffset, (byte)value);
        }
    }
}
