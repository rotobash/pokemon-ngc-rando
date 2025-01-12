using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDCommon.PokemonDefinitions;

namespace APCommon.Memory
{
    public class APPokemonItem : APMemoryObject
    {
        public const int SizeOfGetPokemon = 0x11;

        public byte CurrentLevel { get; set; }
        public ushort Species { get; set; }
        public ushort ShadowIndex { get; set; }
        public ushort Friendship { get; set; }
        public ushort[] MoveIds { get; } = new ushort[4];
        public ushort BattleRoom { get; set; }

        public override byte[] GetBytes()
        {
            var ushortBuffer = new byte[2];
            var uintBuffer = new byte[4];
            var bytes = new List<byte>(SizeOfGetPokemon);

            bytes.Add(CurrentLevel);

            BinaryPrimitives.WriteUInt16BigEndian(ushortBuffer, Species);
            bytes.AddRange(ushortBuffer);

            BinaryPrimitives.WriteUInt16BigEndian(ushortBuffer, ShadowIndex);
            bytes.AddRange(ushortBuffer);

            BinaryPrimitives.WriteUInt16BigEndian(ushortBuffer, Friendship);
            bytes.AddRange(ushortBuffer);

            for (int i = 0; i < MoveIds.Length; i++)
            {
                BinaryPrimitives.WriteUInt16BigEndian(ushortBuffer, MoveIds[i]);
                bytes.AddRange(ushortBuffer);
            }

            BinaryPrimitives.WriteUInt16BigEndian(ushortBuffer, BattleRoom);
            bytes.AddRange(ushortBuffer);

            return bytes.ToArray();
        }

        public override void ReadFromBytes(Span<byte> bytes)
        {
            var offset = 0;
            CurrentLevel = bytes[offset++];

            Species = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(offset, 2));
            offset += 2;

            ShadowIndex = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(offset, 2));
            offset += 2;

            Friendship = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(offset, 2));
            offset += 2;

            for (int i = 0; i < MoveIds.Length; i++)
            {
                MoveIds[i] = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(offset, 2));
                offset += 2;
            }

            BattleRoom = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(offset, 2));
            offset += 2;
        }
    }
}
