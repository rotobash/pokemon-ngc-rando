using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class Pokemon
    {
        const int NumberOfPokemon = 251;
        int dexNum;
        StringTable commonRel;
        public int Index => dexNum;
        public int StartOffset => Stats.kPokemonStatsStartOffset + (dexNum * Stats.kSizeOfPokemonStats);
        public int NameID => commonRel.ExtractedFile.GetIntAtOffset(StartOffset + 0);

        public Abilities Ability1
        {
            get
            {
                var aByte = commonRel.ExtractedFile.GetByteAtOffset(StartOffset + 0);
                return (Abilities)aByte;
            }
            set
            {
                commonRel.ExtractedFile.Seek(StartOffset + 0, SeekOrigin.Begin);
                commonRel.ExtractedFile.WriteByte((byte)value);
            }
        }

        public Abilities Ability2
        {
            get
            {
                var aByte = commonRel.ExtractedFile.GetByteAtOffset(StartOffset + 0);
                return (Abilities)aByte;
            }
            set
            {
                commonRel.ExtractedFile.Seek(StartOffset + 0, SeekOrigin.Begin);
                commonRel.ExtractedFile.WriteByte((byte)value);
            }
        }

        public PokemonTypes Type1
        {
            get
            {
                var aByte = commonRel.ExtractedFile.GetByteAtOffset(StartOffset + 0);
                return (PokemonTypes)aByte;
            }
            set
            {
                commonRel.ExtractedFile.Seek(StartOffset + 0, SeekOrigin.Begin);
                commonRel.ExtractedFile.WriteByte((byte)value);
            }
        }

        public PokemonTypes Type2
        {
            get
            {
                var aByte = commonRel.ExtractedFile.GetByteAtOffset(StartOffset + 0);
                return (PokemonTypes)aByte;
            }
            set
            {
                commonRel.ExtractedFile.Seek(StartOffset + 0, SeekOrigin.Begin);
                commonRel.ExtractedFile.WriteByte((byte)value);
            }
        }

        public byte CatchRate
        {
            get
            {
                return commonRel.ExtractedFile.GetByteAtOffset(StartOffset + 0);
            }
            set
            {
                commonRel.ExtractedFile.Seek(StartOffset + 0, SeekOrigin.Begin);
                commonRel.ExtractedFile.WriteByte(value);
            }
        }

        public Stats Stats
        {
            get;
        }

        public Pokemon(int dexNumber, StringTable commonRel)
        {
            dexNum = dexNumber;
            this.commonRel = commonRel;
            Stats = new Stats(dexNumber);
        }

        
    }
}
