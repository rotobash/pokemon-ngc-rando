using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using XDCommon.Contracts;
using XDCommon.PokemonDefinitions;

namespace XDCommon.Utility
{
    public class REL: FSysFileEntry
    {

        public const int CommonRELDataStartOffsetLocation = 0x6c;
        public const int RELDataStartOffsetLocation = 0x64;
        public const int RELPointersStartOffsetLocation = 0x24;
        public const int RELPointersHeaderPointer1Offset = 0x28;
        public const int RELPointersHeaderPointer2Offset = 0x48;
        public const int RELPointersFirstPointerOffset = 0x8;
        public const int RELSizeOfPointer = 0x10;
        public const int RELPointerDataPointer1Offset = 0x4;
        public const int RELPointerDataPointer2Offset = 0xc;

        uint dataStart;
        uint pointerStart;
        uint firstPointer;
        
        public int NumberOfPointers { get; }

        public REL(string fileName, string path, Stream extractedFile)
        {
            FileName = fileName;
            Path = path;
            FileType = FileTypes.REL;
            ExtractedFile = extractedFile;

            if (FileName.Contains("common_rel"))
            {
                dataStart = ExtractedFile.GetUIntAtOffset(CommonRELDataStartOffsetLocation);
            }
            else
            {
                dataStart = ExtractedFile.GetUIntAtOffset(RELDataStartOffsetLocation);
            }
            pointerStart = ExtractedFile.GetUIntAtOffset(RELPointersStartOffsetLocation);
            firstPointer = pointerStart + RELPointersFirstPointerOffset;

            var pointerHeader = ExtractedFile.GetIntAtOffset(RELPointersHeaderPointer1Offset);
            var pointerEnd = ExtractedFile.GetIntAtOffset(pointerHeader + 0xC);
            var currentOffset = firstPointer;
            var numberOfPointers = 0;
            var end = false;
            while (currentOffset < pointerEnd && !end)
            {
                var val = ExtractedFile.GetIntAtOffset(currentOffset);
                end = val >= 0xCA01 && val <= 0xCAFF;
                if (!end) numberOfPointers++;
                currentOffset += RELSizeOfPointer;
            }

            NumberOfPointers = numberOfPointers;
        }

        public uint GetPointerOffset(int index)
        {
            return (uint)(firstPointer + (index * RELSizeOfPointer) + RELPointerDataPointer1Offset);
        }
        public uint GetPointer(int index)
        {
            var offset = GetPointerOffset(index);
            return ExtractedFile.GetUIntAtOffset(offset) + dataStart;
        }

        public uint GetValueAtPointer(int index)
        {
            var offset = GetPointer(index);
            return ExtractedFile.GetUIntAtOffset(offset);
        }

        public void SetValueAtPointer(int index, int newValue)
        {
            var offset = GetPointer(index);
            ExtractedFile.WriteBytesAtOffset(offset, newValue.GetBytes());
        }

        public void ReplacePointer(int index, int newValue)
        {
            var pointerStart1 = GetPointerOffset(index);
            var pointerStart2 = (uint)(firstPointer + (index * RELSizeOfPointer) + RELPointerDataPointer2Offset);
            ExtractedFile.WriteBytesAtOffset(pointerStart1, ((uint)(newValue - dataStart)).GetBytes());
            ExtractedFile.WriteBytesAtOffset(pointerStart2, ((uint)(newValue - dataStart)).GetBytes());
            ExtractedFile.Flush();
        }

        public void AdjustPointerStart(int byAmount)
        {
            var newPointerStart = (uint)(ExtractedFile.GetUIntAtOffset(RELPointersStartOffsetLocation) + byAmount);
            var pointerHeaderStart = ExtractedFile.GetUIntAtOffset(RELPointersHeaderPointer1Offset);
            var newPointerHeaderStart = (uint)(pointerHeaderStart + byAmount);
            var newPointerHeaderEnd = (uint)(ExtractedFile.GetUIntAtOffset(pointerHeaderStart + 0xC) + byAmount);

            ExtractedFile.WriteBytesAtOffset(RELPointersStartOffsetLocation, newPointerStart.GetBytes());
            ExtractedFile.WriteBytesAtOffset(RELPointersHeaderPointer1Offset, newPointerHeaderStart.GetBytes());
            ExtractedFile.WriteBytesAtOffset(RELPointersHeaderPointer2Offset, newPointerStart.GetBytes());
            ExtractedFile.WriteBytesAtOffset(pointerHeaderStart + 0x4, newPointerStart.GetBytes());
            ExtractedFile.WriteBytesAtOffset(pointerHeaderStart + 0xC, newPointerHeaderEnd.GetBytes());
            firstPointer = newPointerStart + RELPointersFirstPointerOffset;
        }

        public static uint RELtoRAMOffset(Region region, Game game)
        {
            return region switch
            {
                Region.US => game switch
                {
                    Game.XD => 0xb18dc0, // add this value to a common_rel offset to get it's offset in RAM,  XD US
                    Game.Colosseum => 0x7628A0,
                },
                _ => 0,
            };
        }
    }
}
