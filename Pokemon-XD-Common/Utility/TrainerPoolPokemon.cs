using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.PokemonDefinitions;

namespace XDCommon.Utility
{
    public enum PokemonFileType
    {
        DPKM,
        DDPK
    }
    public class TrainerPoolPokemon
    {
        public PokemonFileType PokeType;
        TrainerPool pool;
        ISO iso;

        int index;

        public int StartOffset
        {
            get => PokeType == PokemonFileType.DPKM
                    ? pool.DPKMDataOffset + (index * Constants.SizeOfPokemonData)
                    : pool.DDPKDataOffset + (index * Constants.SizeOfShadowData);
        }

        public int Index
        {
            get
            {
                if (PokeType == PokemonFileType.DDPK)
                {
                    return pool.ExtractedFile.GetUShortAtOffset(StartOffset + Constants.ShadowStoryIndexOffset);
                }
                else
                {
                    return index;
                }
            }
        }

        public TrainerPoolPokemon(int index, TrainerPool team, PokemonFileType type, ISO iso)
        {
            pool = team;
            PokeType = type;
            this.index = index;
            this.iso = iso;
            var num = pool.DPKMDataOffset + (index * Constants.SizeOfPokemonData);
            pokemon = new Pokemon(pool.ExtractedFile.GetUShortAtOffset(num), iso);
        }

        Pokemon pokemon;
        public Pokemon Pokemon
        {
            get => pokemon;
        }

        public void SetPokemon(ushort dexNum)
        {
            var num = pool.DPKMDataOffset + (index * Constants.SizeOfPokemonData);
            pool.ExtractedFile.WriteBytesAtOffset(num, dexNum.GetBytes());
            pokemon = new Pokemon(dexNum, iso);
        }
    }
}
