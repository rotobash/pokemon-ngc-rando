using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions
{
    public class Pokemon
    {
        int dexNum;
        REL commonRel;
        StringTable commonRelStrTbl;
        Game game;
        public int Index => dexNum;
        public int NameID => commonRelStrTbl.ExtractedFile.GetIntAtOffset(StartOffset + Constants.PokemonNameIDOFfset);
        public uint StartOffset
        {
            get
            {
                var stats = game == Game.XD ? Constants.XDPokemonStats : Constants.ColPokemonStats;
                return (uint)(commonRel.GetPointer(stats) + (dexNum * Constants.SizeOfPokemonStats));
            }
        }

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

        public Pokemon(int dexNumber, ISO iso)
        {
            dexNum = dexNumber;
            commonRel = iso.CommonRel();
            commonRelStrTbl = iso.CommonRelStringTable();
            game = iso.Game;
            Stats = new Stats(dexNumber);
        }

        
    }
}
