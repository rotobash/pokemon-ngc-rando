using DolphinMemoryAccess;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using XDCommon.Contracts;
using XDCommon.Patches;
using XDCommon.PokemonDefinitions;

namespace XDCommon.Utility
{
    public class DOLSection
    {
        public uint SectionNumber { get; set; }
        public uint LoadAddress { get; set; }
        public uint DOLOffset { get; set; }
        public uint Size { get; set; }
    }

    public class DOLDataSection : DOLSection
    {
    }

    public class DOLTextSection : DOLSection
    {

        public static DOLTextSection Create(uint sectionNumber, uint loadAddress, uint atOffset, byte[] contents)
        {
            return new DOLTextSection
            {
                SectionNumber = sectionNumber,
                LoadAddress = loadAddress,
                DOLOffset = atOffset,
                Size = (uint)contents.Length
            };
        }
    }

    public class DOL : BaseExtractedFile
    {
        public const int DOLStartOffsetLocation = 0x420;

        const int TextSectionStart = 0x00;
        const int DataSectionStart = 0x1C;
        const int TextAddressStart = 0x48;
        const int DataAddressStart = 0x64;
        const int TextSectionSizeStart = 0x90;
        const int DataSectionSizeStart = 0xAC;
        const int BSSAddressStart = 0xD8;
        const int BSSSizeStart = 0xDC;
        const int EntryPoint = 0xE0;

        const int DolHeaderSize = 0x100;

        public override FileTypes FileType => FileTypes.DOL;
        public override string FileName => "Start.dol"; 

        public int Offset
        {
            get;
            private set;
        }
        
        public uint Size
        {
            get;
            private set;
        }

        public uint BSSAddress
        {
            get => ExtractedFile.GetUIntAtOffset(BSSAddressStart);
        }
        
        public uint BSSSize
        {
            get => ExtractedFile.GetUIntAtOffset(BSSSizeStart);
        }

        public uint Entrypoint
        {
            get => ExtractedFile.GetUIntAtOffset(EntryPoint);
        }

        public uint RAMOffset => (uint)(Entrypoint - Dolphin.EmulatedMemoryBase - 0xB4);

        DOLDataSection[] dataSections = new DOLDataSection[11];
        DOLTextSection[] textSections = new DOLTextSection[7];

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

            ReadDOLFile(ExtractedFile.GetBytesAtOffset(0, DolHeaderSize));
        }

        public DOL(string pathToExtractDirectory, ISOExtractor extractor)
        {
            Path = pathToExtractDirectory;
            var fileName = $"{Path}/{FileName}";
            ExtractedFile = fileName.GetNewStream();
            Offset = (int)extractor.ISOStream.GetUIntAtOffset(DOLStartOffsetLocation);


            ReadDOLFile(extractor.ISOStream.GetBytesAtOffset(Offset, DolHeaderSize));

            if (Configuration.Verbose)
            {
                Console.WriteLine($"DOL Size: {Size:X}");
            }

            extractor.ISOStream.CopySubStream(ExtractedFile, Offset, Size);
            ExtractedFile.Flush();
        }

        void ReadDOLFile(byte[] header)
        {
            using var dolHeader = new MemoryStream(header);
            uint size = DolHeaderSize;

            for (int i = 0; i < textSections.Length; i++)
            {
                var offset = dolHeader.GetUIntAtOffset(TextSectionStart + (i * 4));
                var load_address = dolHeader.GetUIntAtOffset(TextAddressStart + (i * 4));
                var section_size = dolHeader.GetUIntAtOffset(TextSectionSizeStart + (i * 4));
                textSections[i] = new DOLTextSection()
                {
                    SectionNumber = (uint)i,
                    LoadAddress = load_address,
                    DOLOffset = offset,
                    Size = section_size
                };

                size += section_size;
            }

            for (int i = 0; i < dataSections.Length; i++)
            {
                var offset = dolHeader.GetUIntAtOffset(DataSectionStart + (i * 4));
                var load_address = dolHeader.GetUIntAtOffset(DataAddressStart + (i * 4));
                var section_size = dolHeader.GetUIntAtOffset(DataSectionSizeStart + (i * 4));
                dataSections[i] = new DOLDataSection()
                {
                    SectionNumber = (uint)i,
                    LoadAddress = load_address,
                    DOLOffset = offset,
                    Size = section_size
                };

                size += section_size;
            }

            Size = size;
        }

        public override Stream Encode(bool _ = false)
        {
            var streamCopy = new MemoryStream();
            ExtractedFile.Seek(0, SeekOrigin.Begin);
            ExtractedFile.CopyTo(streamCopy);
            streamCopy.Seek(0, SeekOrigin.Begin);
            return streamCopy;
        }

        public uint CreateASMSection(byte[] asmContent)
        {
            return 0;
        }
    }
}
