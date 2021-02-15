using System;
using System.Collections.Generic;
using System.IO;

namespace XDCommon.Utility
{
    public class FST : BaseExtractedFile
    {
        const int TOCStartOffsetLocation = 0x424;
        const int TOCFileSizeLocation = 0x428;
        const int TOCMaxFileSizeLocation = 0x42C;
        const int TOCNumberEntriesOffset = 0x8;
        const int TOCEntrySize = 0xC;

        private Dictionary<string, int> fileLocations = new Dictionary<string, int>();
        private Dictionary<string, int> fileSizes = new Dictionary<string, int>();
        private Dictionary<string, int> fileDataOffsets = new Dictionary<string, int>();

        public List<string> AllFileNames { get; private set; }
        public List<string> FilesOrdered { get; private set; }
        public int TOCFirstStringOffset => (int)ExtractedFile.GetUIntAtOffset(TOCNumberEntriesOffset) * TOCEntrySize;
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

        public FST(string pathToExtractDirectory, ISOExtractor extractor)
        {
            var fileName = $"{pathToExtractDirectory}/Game.toc";
            ExtractedFile = fileName.GetNewStream();

            // load toc
            Offset = (int)extractor.ISOStream.GetUIntAtOffset(TOCStartOffsetLocation);
            Size = (int)extractor.ISOStream.GetUIntAtOffset(TOCFileSizeLocation);

            if (Configuration.Verbose)
            {
                Console.WriteLine("Reading TOC from ISO");
                Console.WriteLine($"TOC Start: {Offset:X}");
                Console.WriteLine($"TOC Length: {Size:X}");
            }

            // write to disk
            extractor.ISOStream.CopySubStream(ExtractedFile, Offset, Size);
            ExtractedFile.Flush();
        }
        
        public FST(string pathToExtractDirectory)
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
            Offset = TOCStartOffsetLocation;
            Size = (int)ExtractedFile.Length;
        }

        public void Load(DOL dolFile)
        {

            if (Configuration.Verbose)
            {
                Console.WriteLine($"Extracting TOC.");
            }

            fileLocations.Add("Game.toc", Offset);
            fileSizes.Add("Game.toc", Size);
            fileLocations.Add("Start.dol", dolFile.Offset);
            fileSizes.Add("Start.dol", dolFile.Size);

            AllFileNames = new List<string>
            {
                "Start.dol",
                "Game.toc"
            };

            for (int i = 0; i * 12 < TOCFirstStringOffset; i++)
            {
                var offset = i * 12;
                var folder = ExtractedFile.GetByteAtOffset(offset) == 1;

                if (!folder)
                {
                    var nameOffset = ExtractedFile.GetIntAtOffset(offset);
                    var fileOffset = ExtractedFile.GetIntAtOffset(offset + 4);
                    var fileSize = ExtractedFile.GetIntAtOffset(offset + 8);

                    var fileNameChars = ExtractedFile.GetStringAtOffset(nameOffset + TOCFirstStringOffset);
                    var fileName = string.Join("", fileNameChars);
                    AllFileNames.Add(fileName);
                    fileLocations.Add(fileName, fileOffset);
                    fileSizes.Add(fileName, fileSize);
                    fileDataOffsets.Add(fileName, offset);
                }
            }

            FilesOrdered = new List<string>(AllFileNames);
            FilesOrdered.Sort((s1, s2) => LocationForFile(s1) - LocationForFile(s2));

            if (Configuration.Verbose)
            {
                Console.WriteLine($"Finished reading FST.");
            }
        }

        public int LocationForFile(string fileName)
        {
            if (fileLocations.ContainsKey(fileName))
            {
                return fileLocations[fileName];
            }
            return 0;
        }

        public int SizeForFile(string fileName)
        {
            if (fileSizes.ContainsKey(fileName))
            {
                return fileSizes[fileName];
            }
            return 0;
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
