using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;

namespace APCommon.Memory
{
    public class APGetItems : APMemoryObject
    {
        public const int SizeOfGetItems = 2 + APPokemonItem.SizeOfGetPokemon * 5;

        ushort pokemonCounter;
        APPokemonItem[] GivePokemon = new APPokemonItem[5];

        public APGetItems()
        {
            for (int i = 0; i < GivePokemon.Length; i++)
            {
                GivePokemon[i] = new APPokemonItem();
            }
        }

        public override byte[] GetBytes()
        {
            var ushortBuffer = new byte[2];
            var uintBuffer = new byte[4];
            var bytes = new List<byte>(SizeOfGetItems);

            BinaryPrimitives.WriteUInt16BigEndian(ushortBuffer, pokemonCounter);
            bytes.AddRange(ushortBuffer);

            for (int i = 0; i < GivePokemon.Length; i++)
            {
                bytes.AddRange(GivePokemon[i].GetBytes());
            }

            return bytes.ToArray();
        }

        public override void ReadFromBytes(Span<byte> bytes)
        {
            var offset = 0;

            pokemonCounter = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(0, 2));
            offset += 2;

            for (int i = 0; i < GivePokemon.Length; i++)
            {
                GivePokemon[i].ReadFromBytes(bytes.Slice(offset, APPokemonItem.SizeOfGetPokemon));
                offset += APPokemonItem.SizeOfGetPokemon;
            }
        }
    }
}
