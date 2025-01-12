using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APCommon.Memory
{
    public class APItemCheck : APMemoryObject
    {
        public const int SizeOfAPItemCheck = 8;

        public ushort Type { get; set; }
        public ushort ItemIndex { get; set; }
        public ushort Quantity { get; set; }
        public ushort RoomId { get; set; }

        public override byte[] GetBytes()
        {
            var ushortBuffer = new byte[2];
            var bytes = new List<byte>(SizeOfAPItemCheck);

            BinaryPrimitives.WriteUInt16BigEndian(ushortBuffer, Type);
            BinaryPrimitives.WriteUInt16BigEndian(ushortBuffer, ItemIndex);
            BinaryPrimitives.WriteUInt16BigEndian(ushortBuffer, Quantity);
            BinaryPrimitives.WriteUInt16BigEndian(ushortBuffer, RoomId);

            return bytes.ToArray();
        }

        public override void ReadFromBytes(Span<byte> bytes)
        {
            var offset = 0;

            Type = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(offset, 2));
            offset += 2;

            ItemIndex = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(offset, 2));
            offset += 2;

            Quantity = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(offset, 2));
            offset += 2;

            RoomId = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(offset, 2));
        }
    }
}
