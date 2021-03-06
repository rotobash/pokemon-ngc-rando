﻿using System;
using System.Collections.Generic;
using System.IO;
using XDCommon.Contracts;

namespace XDCommon.Utility
{
    public class DOL : BaseExtractedFile
    {
        public const int DOLStartOffsetLocation = 0x420;
        const int DolSectionSizesStart = 0x90;
        const int DolSectionSizesCount = 18;
        const int DolHeaderSize = 0x100;

        public override FileTypes FileType => FileTypes.DOL;
        public override string FileName => "Start.dol"; 

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
            Path = pathToExtractDirectory;
            var fileName = $"{Path}/{FileName}";
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
            Path = pathToExtractDirectory;
            var fileName = $"{Path}/{FileName}";
            ExtractedFile = fileName.GetNewStream();
            Offset = (int)extractor.ISOStream.GetUIntAtOffset(DOLStartOffsetLocation);

            var size = DolHeaderSize;
            for (int i = 0; i < DolSectionSizesCount; i++)
            {
                var offset = Offset + (i * 4) + DolSectionSizesStart;
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

        public override Stream Encode(bool _ = false)
        {
            var streamCopy = new MemoryStream();
            ExtractedFile.Seek(0, SeekOrigin.Begin);
            ExtractedFile.CopyTo(streamCopy);
            streamCopy.Seek(0, SeekOrigin.Begin);
            return streamCopy;
        }
    }
}
