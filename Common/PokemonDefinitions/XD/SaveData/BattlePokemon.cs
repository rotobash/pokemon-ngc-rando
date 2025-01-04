using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using XDCommon.Utility;

namespace XDCommon.PokemonDefinitions.XD.SaveData
{
    public class BattlePokemon : PokemonInstance
    {
        public const int Size = SizeOfData + 0x248;

        const int PartyPokemonPtrOffset = 0x0;

        readonly MemoryStream Data;

        public uint PartyPokemonPtr
        {
            get => Data.GetUIntAtOffset(PartyPokemonPtrOffset);
        }

        public BattlePokemon(byte[] bytes) : base(bytes.Skip(4).Take(SizeOfData).ToArray())
        {
            Data = new MemoryStream(bytes);
        }
    }
}
