using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XDCommon.Contracts;

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

    public class StringTable: FSysFileEntry
    {
        const int kNumberOfStringsOffset = 0x04;
        const int kEndOfHeader = 0x10;

        const int kMaxStringID = 0xFFFFF;

        int startOffset = 0;
        Dictionary<int, int> stringOffsets = new Dictionary<int, int>();

        public int NumberOfEntries => ExtractedFile.GetUShortAtOffset(kNumberOfStringsOffset);

        public StringTable()
        {
            FileType = FileTypes.MSG;
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
                stringOffsets.Add(id, offset);
                currentOffset += 8;
            }
        }

        public UnicodeString GetStringAtOffset(int offset)
        {
            var currentOffset = offset;
            var currChar = 0x0;
            var nextChar = 0x1;
            var str = new UnicodeString();

            while (nextChar != 0)
            {
                if (currentOffset + 2 > ExtractedFile.Length)
                {
                    break;
                }
                currChar = ExtractedFile.GetUShortAtOffset(currentOffset);
                currentOffset += 2;

                if (currChar == 0xFFFF)
                {
                    var specChar = (SpecialCharacters)ExtractedFile.GetByteAtOffset(currentOffset);
                    currentOffset += 1;

                    var extra = 0;
                    var bytes = new byte[extra];
                    ExtractedFile.Read(bytes, offset, extra);

                    str.Add(new SpecialUnicodeCharacters(specChar, bytes));
                }
                else
                {
                    str.Add(new UnicodeCharacters(currChar));
                }
                nextChar = ExtractedFile.GetUShortAtOffset(currentOffset);
            }
            return str;
        }

        public UnicodeString GetStringWithId(int id)
        {
            UnicodeString str;
            if (!stringOffsets.ContainsKey(id))
            {
                str = null;
            }
            else
            {
                var offset = stringOffsets[id];
                if (offset + 2 > ExtractedFile.Length)
                    str = null;
                else
                    str = GetStringAtOffset(offset);

            }
            return str ?? new UnicodeString();
        }

        public IEnumerable<UnicodeString> GetAllStrings()
        {
            var strings = new UnicodeString[stringOffsets.Count];
            var currentIndex = 0;

            foreach (var offset in stringOffsets.Values)
            {
                strings[currentIndex++] = GetStringAtOffset(offset);
            }
            return strings;
        }
    }
}
