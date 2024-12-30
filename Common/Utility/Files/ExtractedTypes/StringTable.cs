using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace XDCommon.Utility
{
    enum Languages
    {
        Japanese,
        EnglishUS,
        EnglishUK,
        German,
        French,
        Italian,
        Spanish
    }

    public class StringTable : FSysFileEntry
    {
        const int kNumberOfStringsOffset = 0x04;
        const int kEndOfHeader = 0x10;

        const int kMaxStringID = 0xFFFFF;

        Dictionary<int, int> stringOffsets = new Dictionary<int, int>();

        Dictionary<int, UnicodeString> strings = new Dictionary<int, UnicodeString>();

        public int NumberOfEntries => ExtractedFile.GetUShortAtOffset(kNumberOfStringsOffset);
        public IEnumerable<int> StringIds => strings.Keys;

        public StringTable(Stream stream)
        {
            ExtractedFile = stream;
            GetOffsets();
        }

        public int ExtraChracters()
        {
            var currentChar = 0;
            var currentOffset = (int)ExtractedFile.Length - 3;
            var length = 0;

            while (currentChar == 0)
            {
                currentChar = ExtractedFile.GetByteAtOffset(currentOffset);
                if (currentChar == 0)
                    length++;
                currentOffset--;
            }
            return length;
        }

        void GetOffsets()
        {
            var currentOffset = kEndOfHeader;
            for (int i = 0; i < NumberOfEntries; i++)
            {
                var id = ExtractedFile.GetIntAtOffset(currentOffset) & 0xFFFFF;
                var offset = ExtractedFile.GetIntAtOffset(currentOffset + 4);
                if (!stringOffsets.ContainsKey(id))
                {
                    stringOffsets.Add(id, offset);
                    strings.Add(id, GetStringAtOffset(offset));
                }
                currentOffset += 8;
            }
        }

        public UnicodeString GetStringAtOffset(int offset)
        {
            return ExtractedFile.GetStringAtOffset(offset);
        }

        public UnicodeString GetStringWithId(int id)
        {
            UnicodeString str;
            if (stringOffsets.TryGetValue(id, out var offset) && offset + 2 <= ExtractedFile.Length)
            {
                if (!strings.TryGetValue(id, out str))
                {
                    str = ExtractedFile.GetStringAtOffset(offset);
                    strings[id] = str;
                }
            }
            else
            {
                str = null;
            }
            return str ?? new UnicodeString();
        }

        public void ReplaceString(int id, UnicodeString newString)
        {
            strings[id] = newString;
        }

        public override Stream Encode(bool isCompressed)
        {
            Stream stringFile = string.Empty.GetNewStream();
            ExtractedFile.CopySubStream(stringFile, 0, kEndOfHeader);

            var firstOffset = strings.Count * 8;
            var newOffsetDict = new Dictionary<int, int>();

            var strOffset = firstOffset;
            foreach (var strKvp in strings)
            {
                var strBytes = strKvp.Value.ToByteArray();
                stringFile.WriteBytesAtOffset(strOffset, strBytes);
                stringFile.WriteBytesAtOffset(strOffset + strBytes.Length, new byte[] { 0, 0 });
                newOffsetDict.Add(strKvp.Key, strOffset);

                strOffset += strBytes.Length + 2;
            }

            var pointerOffset = kEndOfHeader;
            foreach (var offsetKvp in newOffsetDict)
            {
                stringFile.WriteBytesAtOffset(pointerOffset, offsetKvp.Key.GetBytes());
                stringFile.WriteBytesAtOffset(pointerOffset + 4, offsetKvp.Value.GetBytes());
                pointerOffset += 8;
            }

            stringFile.Seek(0, SeekOrigin.Begin);
            ExtractedFile.Dispose();
            ExtractedFile = stringFile;
            stringOffsets = newOffsetDict;

            return base.Encode(isCompressed);
        }
    }
}
