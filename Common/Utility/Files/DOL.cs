using System;
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

        private uint DolToRAMOffset(Game game)
        {
            return game switch
            {
                Game.Colosseum => 0x3000,
                Game.XD => 0x30a0,
                _ => throw new Exception("Unsupported game!")
            };
        }

        private uint DOLFreeSpaceStart(Game game, Region region)
        {
            return game switch
            {
                Game.Colosseum => region switch
                {
                    Region.US => 0xbe348 - DolToRAMOffset(game),
                    Region.Europe => 0xc1948 - DolToRAMOffset(game),
                    Region.Japan => 0xbb4a8 - DolToRAMOffset(game),
                    _ => throw new Exception("Unknown region!")
                },
                Game.XD => region switch
                {
                    Region.US => 0xd39d0 - DolToRAMOffset(game),
                    Region.Europe => 0xd4fec - DolToRAMOffset(game),
                    Region.Japan => 0xcfef4 - DolToRAMOffset(game),
                    _ => throw new Exception("Unsupported game!")
                },
            };
        }

        private uint DOLFreeSpaceEnd(Game game, Region region)
        {
            return game switch
            {
                Game.Colosseum => region switch
                {
                    Region.US => 0xc459c - DolToRAMOffset(game),
                    Region.Europe => 0xc7b9c - DolToRAMOffset(game),
                    Region.Japan => 0xc16f8 - DolToRAMOffset(game),
                    _ => throw new Exception("Unknown region!")
                },
                Game.XD => region switch
                {
                    Region.US => 0xd9c2c - DolToRAMOffset(game),
                    Region.Europe => 0xdb23c - DolToRAMOffset(game),
                    Region.Japan => 0x30a0 - DolToRAMOffset(game),
                    _ => throw new Exception("Unsupported game!")
                },
            };
        }

        public uint FindFreeSpace(Game game, Region region)
        {
            var startPointer = DOLFreeSpaceStart(game, region);
            var endPointer = DOLFreeSpaceEnd(game, region);

            var offset = startPointer;
            while (offset < endPointer)
            {
                if (ExtractedFile.GetUIntAtOffset(offset) != 0)
                {
                    offset += 4;
                    continue;
                }
                else
                {
                    return offset;
                }
            }

            return 0;
        }
    }
}
