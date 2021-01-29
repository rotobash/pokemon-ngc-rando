using System;
using System.Collections.Generic;
using System.IO;

namespace XDCommon.Utility
{
    public class DOL
    {
        const int kDOLStartOffsetLocation = 0x420;
        const int kDolSectionSizesStart = 0x90;
        const int kDolSectionSizesCount = 18;
        const int kDolHeaderSize = 0x100;

        Stream dolStream;

        public int Offset
        {
            get;
            private set;
        }
        
        public int Size
        {
            get;
            private set;
        }

        public DOL(string pathToExtractDirectory, int offset)
        {
            var fileName = $"{pathToExtractDirectory}/Game.toc";
            if (!File.Exists(fileName))
            {
                throw new ArgumentException("ISOExtractor must be provided if file doesn't exist.");
            }

            dolStream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite);
            Offset = offset;
            Size = (int)dolStream.Length;
        }

        public DOL(string pathToExtractDirectory, ISOExtractor extractor)
        {
            var fileName = $"{pathToExtractDirectory}/Start.dol";
            dolStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Offset = (int)extractor.ISOStream.GetUIntAtOffset(kDOLStartOffsetLocation);

            var size = kDolHeaderSize;
            for (int i = 0; i < kDolSectionSizesCount; i++)
            {
                var offset = Offset + (i * 4) + kDolSectionSizesStart;
                size += (int)extractor.ISOStream.GetUIntAtOffset(offset);
            }
            Size = size;

            if (true)
            {
                Console.WriteLine($"DOL Size: {Size:X}");
            }

            extractor.ISOStream.CopySubStream(dolStream, Offset, Size);
            dolStream.Flush();
        }
    }
}
