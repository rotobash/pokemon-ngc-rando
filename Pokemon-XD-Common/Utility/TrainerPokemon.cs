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
    public class TrainerPokemon
    {
        public PokemonFileType PokeType;
        TrainerPool pool;

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

        public TrainerPokemon(int index, TrainerPool team, PokemonFileType type)
        {
            pool = team;
            PokeType = type;
            this.index = index;
            var num = pool.DPKMDataOffset + (index * Constants.SizeOfPokemonData);
            pokemon = team.PokemonList[pool.ExtractedFile.GetUShortAtOffset(num)]; // new Pokemon(, iso);
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
            pokemon = pool.PokemonList[dexNum];
        }
    }
}
