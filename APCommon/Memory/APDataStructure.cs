using DolphinMemoryAccess;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APCommon.Memory
{
    public class APDataStructure : APMemoryObject
    {
        public const uint APMemoryAddress = 0x81500000;

        public const int SizeOfAPData = 4 + APGetItems.SizeOfGetItems + APCheckedItems.SizeOfAPCheckedItems;

        public ushort Flags;
        byte DeathLink { get;  set; }
        byte Escape { get; set; }

        APCheckedItems checks = new APCheckedItems();
        APGetItems give = new APGetItems();

        public override byte[] GetBytes()
        {
            var ushortBuffer = new byte[2];
            var bytes = new List<byte>(SizeOfAPData);

            BinaryPrimitives.WriteUInt16BigEndian(ushortBuffer, Flags);
            bytes.AddRange(ushortBuffer);
            bytes.Add(DeathLink);
            bytes.Add(Escape);

            bytes.AddRange(checks.GetBytes());
            bytes.AddRange(give.GetBytes());

            return bytes.ToArray();
        }

        public override void ReadFromBytes(Span<byte> bytes)
        {
            var offset = 2;
            Flags = BinaryPrimitives.ReadUInt16BigEndian(bytes.Slice(0, 2));
            DeathLink = bytes[offset++];
            Escape = bytes[offset++];

            var checkedBytes = bytes.Slice(offset, APCheckedItems.SizeOfAPCheckedItems);
            checks.ReadFromBytes(checkedBytes);
            offset += APCheckedItems.SizeOfAPCheckedItems;

            var giveBytes = bytes.Slice(offset, APGetItems.SizeOfGetItems);
            give.ReadFromBytes(giveBytes);
            offset += APGetItems.SizeOfGetItems;
        }

        public void WriteDeathLink(Dolphin dolphin)
        {
            dolphin.WriteData(APMemoryAddress + 2, new byte[] { 1 });
        }
        public void GiveItem(Dolphin dolphin)
        {
            dolphin.WriteData(APMemoryAddress + 2, new byte[] { 1 });
        }
        public APCheckedItems GetCheckedItems()
        {
            return checks;
        }
    }
}
