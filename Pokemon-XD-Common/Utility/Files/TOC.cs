using System;
using System.Collections.Generic;
using System.IO;

namespace XDCommon.Utility
{
    public class TOC
    {
        const int kTOCStartOffsetLocation = 0x424;
        const int kTOCFileSizeLocation = 0x428;
        const int kTOCMaxFileSizeLocation = 0x42C;
        const int kTOCNumberEntriesOffset = 0x8;
        const int kTOCEntrySize = 0xC;

        private Dictionary<string, int> fileLocations = new Dictionary<string, int>();
        private Dictionary<string, int> fileSizes = new Dictionary<string, int>();
        private Dictionary<string, int> fileDataOffsets = new Dictionary<string, int>();

        public List<string> AllFileNames { get; private set; }
        public List<string> FilesOrdered { get; private set; }
        public Stream TOCStream { get; }
        public int TOCFirstStringOffset => (int)TOCStream.GetUIntAtOffset(kTOCNumberEntriesOffset) * kTOCEntrySize;
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

        public TOC(string pathToExtractDirectory, ISOExtractor extractor)
        {
            var fileName = $"{pathToExtractDirectory}/Game.toc";
            TOCStream = fileName.GetNewStream();

            // load toc
            Offset = (int)extractor.ISOStream.GetUIntAtOffset(kTOCStartOffsetLocation);
            Size = (int)extractor.ISOStream.GetUIntAtOffset(kTOCFileSizeLocation);

            if (Configuration.Verbose)
            {
                Console.WriteLine("Reading TOC from ISO");
                Console.WriteLine($"TOC Start: {Offset:X}");
                Console.WriteLine($"TOC Length: {Size:X}");
            }

            // write to disk
            extractor.ISOStream.CopySubStream(TOCStream, Offset, Size);
            TOCStream.Flush();
        }
        
        public TOC(string pathToExtractDirectory, int offset)
        {
            var fileName = $"{pathToExtractDirectory}/Game.toc";
            if (!File.Exists(fileName))
            {
                throw new ArgumentException("ISOExtractor must be provided if file doesn't exist.");
            }

            var file = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite);
            if (Configuration.UseMemoryStreams)
            {
                TOCStream = new MemoryStream();
                file.CopyTo(TOCStream);
            }
            else
            {
                TOCStream = file;
            }
            Offset = offset;
            Size = (int)TOCStream.Length;
        }

        ~TOC()
        {
            TOCStream.Flush();
            TOCStream.Dispose();
        }

        public void Load(DOL dolFile, bool verbose)
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
                var folder = TOCStream.GetByteAtOffset(offset) == 1;

                if (!folder)
                {
                    var nameOffset = TOCStream.GetIntAtOffset(offset);
                    var fileOffset = TOCStream.GetIntAtOffset(offset + 4);
                    var fileSize = TOCStream.GetIntAtOffset(offset + 8);

                    var fileNameChars = TOCStream.GetStringAtOffset(nameOffset + TOCFirstStringOffset);
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
    }
}
