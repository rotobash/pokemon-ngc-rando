using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public enum PokeSpotType
    {
        Rock,
        Oasis,
        Cave,
        All
    }

    public class PokeSpot
    {
        ISO iso;
        public PokeSpotType PokeSpotType { get; }
        public PokeSpot(PokeSpotType pokeSpot, ISO iso)
        {
            PokeSpotType = pokeSpot;
            this.iso = iso;
        }

        public int Index => PokeSpotType switch
        {
            PokeSpotType.Rock => (int)iso.CommonRel.GetPointer(Constants.PokespotRock),
            PokeSpotType.Oasis => (int)iso.CommonRel.GetPointer(Constants.PokespotOasis),
            PokeSpotType.Cave => (int)iso.CommonRel.GetPointer(Constants.PokespotCave),
            _ => (int)iso.CommonRel.GetPointer(Constants.PokespotAll),
        };

        public int EntriesIndex => PokeSpotType switch
        {
            PokeSpotType.Rock => (int)iso.CommonRel.GetPointer(Constants.PokespotRockEntries),
            PokeSpotType.Oasis => (int)iso.CommonRel.GetPointer(Constants.PokespotOasisEntries),
            PokeSpotType.Cave => (int)iso.CommonRel.GetPointer(Constants.PokespotCaveEntries),
            _ => (int)iso.CommonRel.GetPointer(Constants.PokespotAllEntries),
        };

        public int NumberOfEntries
        {
            get => iso.CommonRel.ExtractedFile.GetIntAtOffset(EntriesIndex);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(EntriesIndex, value.GetBytes());
        }
    }

    public class PokeSpotPokemon
    {
        int index;
        ISO iso;

        public PokeSpot PokeSpot { get; }
        public int StartOffset => PokeSpot.Index + (index * Constants.SizeOfPokeSpotData);

        public PokeSpotPokemon(int index, PokeSpot pokeSpot, ISO iso)
        {
            PokeSpot = pokeSpot;
            this.index = index;
            this.iso = iso;
        }

        public byte MinLevel
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.MinLevelOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.MinLevelOffset, value);
        }

        public byte MaxLevel
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.MaxLevelOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.MaxLevelOffset, value);
        }

        public byte EncounterPercentage
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + Constants.EncounterPercentageOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + Constants.EncounterPercentageOffset, value);
        }
        
        public ushort Pokemon
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.PokeSpotSpeciesOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.PokeSpotSpeciesOffset, value.GetBytes());
        }
        public ushort StepsPerSnack
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.StepsPerPokeSnackOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + Constants.StepsPerPokeSnackOffset, value.GetBytes());
        }
    }
}
