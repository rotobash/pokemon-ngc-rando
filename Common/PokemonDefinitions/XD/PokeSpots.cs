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
        const int PokespotRock = 12;
        const int PokespotRockEntries = 13;
        const int PokespotOasis = 15;
        const int PokespotOasisEntries = 16;
        const int PokespotCave = 18;
        const int PokespotCaveEntries = 19;
        const int PokespotAll = 21;
        const int PokespotAllEntries = 22;

        ISO iso;
        public PokeSpotType PokeSpotType { get; }
        public PokeSpot(PokeSpotType pokeSpot, ISO iso)
        {
            PokeSpotType = pokeSpot;
            this.iso = iso;
        }

        public int Index => PokeSpotType switch
        {
            PokeSpotType.Rock => (int)iso.CommonRel.GetPointer(PokespotRock),
            PokeSpotType.Oasis => (int)iso.CommonRel.GetPointer(PokespotOasis),
            PokeSpotType.Cave => (int)iso.CommonRel.GetPointer(PokespotCave),
            _ => (int)iso.CommonRel.GetPointer(PokespotAll),
        };

        public int EntriesIndex => PokeSpotType switch
        {
            PokeSpotType.Rock => (int)iso.CommonRel.GetPointer(PokespotRockEntries),
            PokeSpotType.Oasis => (int)iso.CommonRel.GetPointer(PokespotOasisEntries),
            PokeSpotType.Cave => (int)iso.CommonRel.GetPointer(PokespotCaveEntries),
            _ => (int)iso.CommonRel.GetPointer(PokespotAllEntries),
        };

        public int NumberOfEntries
        {
            get => iso.CommonRel.ExtractedFile.GetIntAtOffset(EntriesIndex);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(EntriesIndex, value.GetBytes());
        }
    }

    public class PokeSpotPokemon
    {
        const int FirstPokeSpotOffset = 0x2FAC;
        const byte SizeOfPokeSpotData = 0x0C;
        const byte NumberOfPokeSpotEntries = 0x0B;
        const byte MinLevelOffset = 0x00;
        const byte MaxLevelOffset = 0x01;
        const byte PokeSpotSpeciesOffset = 0x02;
        const byte EncounterPercentageOffset = 0x07;
        const byte StepsPerPokeSnackOffset = 0x0A;

        int index;
        ISO iso;

        public PokeSpot PokeSpot { get; }
        public int StartOffset => PokeSpot.Index + (index * SizeOfPokeSpotData);

        public PokeSpotPokemon(int index, PokeSpot pokeSpot, ISO iso)
        {
            PokeSpot = pokeSpot;
            this.index = index;
            this.iso = iso;
        }

        public PokeSpotPokemon(int index, PokeSpotType pokeSpotType, ISO iso)
        {
            PokeSpot = new PokeSpot(pokeSpotType, iso);
            this.index = index;
            this.iso = iso;
        }

        public byte MinLevel
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + MinLevelOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + MinLevelOffset, value);
        }

        public byte MaxLevel
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + MaxLevelOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + MaxLevelOffset, value);
        }

        public byte EncounterPercentage
        {
            get => iso.CommonRel.ExtractedFile.GetByteAtOffset(StartOffset + EncounterPercentageOffset);
            set => iso.CommonRel.ExtractedFile.WriteByteAtOffset(StartOffset + EncounterPercentageOffset, value);
        }
        
        public ushort Pokemon
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + PokeSpotSpeciesOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + PokeSpotSpeciesOffset, value.GetBytes());
        }
        public ushort StepsPerSnack
        {
            get => iso.CommonRel.ExtractedFile.GetUShortAtOffset(StartOffset + StepsPerPokeSnackOffset);
            set => iso.CommonRel.ExtractedFile.WriteBytesAtOffset(StartOffset + StepsPerPokeSnackOffset, value.GetBytes());
        }
    }
}
