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

        public Stream ExtractedFile;

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

            var file = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite);
            if (Configuration.UseMemoryStreams)
            {
                ExtractedFile = new MemoryStream();
                file.CopyTo(ExtractedFile);
            }
            else
            {
                ExtractedFile = file;
            }
            Offset = offset;
            Size = (int)ExtractedFile.Length;
        }

        public DOL(string pathToExtractDirectory, ISOExtractor extractor)
        {
            var fileName = $"{pathToExtractDirectory}/Start.dol";
            ExtractedFile = fileName.GetNewStream();
            Offset = (int)extractor.ISOStream.GetUIntAtOffset(kDOLStartOffsetLocation);

            var size = kDolHeaderSize;
            for (int i = 0; i < kDolSectionSizesCount; i++)
            {
                var offset = Offset + (i * 4) + kDolSectionSizesStart;
                size += (int)extractor.ISOStream.GetUIntAtOffset(offset);
            }
            Size = size;

            if (Configuration.Verbose)
            {
                Console.WriteLine($"DOL Size: {Size:X}");
            }

            extractor.ISOStream.CopySubStream(ExtractedFile, Offset, Size);
            ExtractedFile.Flush();
        }
    }
}
