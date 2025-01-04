using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions.XD.SaveData
{
    public class PCBox
    {
        const byte FirstPCPokemonOffset = 0x14;
        public UnicodeString BoxName
        {
            get;
        }

        public PokemonInstance[] Pokemon
        {
            get;
        } = new PokemonInstance[30];

        public PCBox(byte[] boxData)
        {
            BoxName = new UnicodeString(boxData.Take(0xF));
            var offset = FirstPCPokemonOffset;
            for (int i = 0; i < 30; i++)
            {
                byte[] pokemonData = new byte[0xC4];
                Array.Copy(boxData, offset + (i * 0xC4), pokemonData, 0, 0xC4);
                Pokemon[i] = new PokemonInstance(pokemonData);
            }
        }
    }
}
