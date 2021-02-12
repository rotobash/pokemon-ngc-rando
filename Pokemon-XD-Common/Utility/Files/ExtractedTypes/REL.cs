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

        public const int kCommonRELDataStartOffsetLocation = 0x6c;
        public const int kRELDataStartOffsetLocation = 0x64;
        public const int kRELPointersStartOffsetLocation = 0x24;
        public const int kRELPointersHeaderPointerOffset = 0x28;
        public const int kRELPointersFirstPointerOffset = 0x8;
        public const int kRELSizeOfPointer = 0x10;
        public const int kRELPointerDataPointer1Offset = 0x4;
        public const int kRELPointerDataPointer2Offset = 0xc;

        uint dataStart;
        uint pointerStart;
        uint firstPointer;
        int numberOfPointers;

        public REL(string fileName, string path, Stream extractedFile)
        {
            FileName = fileName;
            Path = path;
            FileType = FileTypes.REL;
            ExtractedFile = extractedFile;

            if (FileName.Contains("common_rel"))
            {
                dataStart = ExtractedFile.GetUIntAtOffset(kCommonRELDataStartOffsetLocation);
            }
            else
            {
                dataStart = ExtractedFile.GetUIntAtOffset(kRELDataStartOffsetLocation);
            }
            pointerStart = ExtractedFile.GetUIntAtOffset(kRELPointersStartOffsetLocation);
            firstPointer = pointerStart + kRELPointersFirstPointerOffset;
        }

        public uint GetPointerOffset(int index)
        {
            return (uint)(firstPointer + (index * kRELSizeOfPointer) + kRELPointerDataPointer1Offset);
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
            ExtractedFile.Seek(offset, SeekOrigin.Begin);
            ExtractedFile.Write(newValue.GetBytes());
        }
    }
}
