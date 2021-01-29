using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Utility.Files.ExtractedTypes
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

    public class StringTable
    {
        const int kNumberOfStringsOffset = 0x04;
        const int kEndOfHeader = 0x10;

        const int kMaxStringID = 0xFFFFF;

        int startOffset = 0;
        FSysFileEntry fileData;
        Dictionary<int, int> stringOffsets = new Dictionary<int, int>();
        List<int> stringIDs = new List<int>();

        public int NumberOfEntries => fileData.ExtractedFile.GetUShortAtOffset(kNumberOfStringsOffset);

        public int ExtraChracters()
        {
            var currentChar = 0;
            var currentOffset = (int)fileData.ExtractedFile.Length - 3;
            var length = 0;

            while (currentChar == 0)
            {
                currentChar = fileData.ExtractedFile.GetByteAtOffset(currentOffset);
                if (currentChar == 0)
                    length++;
                currentOffset--;
            }
            return length;
        }
    }
}
