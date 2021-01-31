using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDCommon.Contracts;

namespace XDCommon.Utility
{
    public class FSys
    {
        const byte kFSYSGroupIDOffset = 0x08;
        const byte kNumberOfEntriesOffset = 0x0C;
        const byte kFSYSFileSizeOffset = 0x20;
        const byte kFirstFileNamePointerOffset = 0x44;
        const byte kFirstFileOffset = 0x48;
        const byte kFirstFileDetailsPointerOffset = 0x60;
        const byte kFirstFileNameOffset = 0x70;

        const byte kFileIdentifierOffset = 0x00; // 3rd byte is the file format, 1st half is an arbitrary identifier
        const byte kFileFormatOffset = 0x02;
        const byte kFileStartPointerOffset = 0x04;
        const byte kUncompressedSizeOffset = 0x08;
        const byte kCompressedSizeOffset = 0x14;
        const byte kFileDetailsFullFilenameOffset = 0x1C; // includes file extension. Not always used.
        const byte kFileFormatIndexOffset = 0x20; // half of value in byte 3
        const byte kFileDetailsFilenameOffset = 0x24;

        const byte kLZSSUncompressedSizeOffset = 0x04;
        const byte kLZSSCompressedSizeOffset = 0x08;
        const byte kLZSSUnkownOffset = 0x0C;// PBR only, unused in Colo/XD

        const uint kLZSSbytes = 0x4C5A5353;
        const uint kTCODbytes = 0x54434F44;
        const uint kFSYSbytes = 0x46535953;
        const ushort kUSbytes = 0x5553;
        const ushort kJPbytes = 0x4A50;

        public Stream fileStream;

        public List<IExtractedFile> ExtractedEntries = new List<IExtractedFile>();

        public int GroupID
        {
            get
            {
                return fileStream.GetIntAtOffset(kFSYSGroupIDOffset);
            }
            set
            {
                fileStream.Seek(kFSYSGroupIDOffset, SeekOrigin.Begin);
                fileStream.Write(BitConverter.GetBytes(value));
            }
        }

        public int NumberOfEntries
        {
            get
            {
                return fileStream.GetIntAtOffset(kNumberOfEntriesOffset);
            }
            set
            {
                fileStream.Seek(kNumberOfEntriesOffset, SeekOrigin.Begin);
                fileStream.Write(BitConverter.GetBytes(value));
            }
        }

        public string Filename { get; }
        public string Path { get; }
        public Region Region { get; }
        public Game Game { get; }

        public bool UsesFileExtensions => fileStream.GetByteAtOffset(0x13) == 1;

        public FSys(string pathToFile, Region region, Game game)
        {
            fileStream = File.Open(pathToFile, FileMode.Open, FileAccess.ReadWrite);
            var fileParts = pathToFile.Split("/");
            Filename = fileParts.Last();
            Path = string.Join("/", fileParts.Take(fileParts.Length - 1));
            Region = region;
            Game = game;
        }

        public FSys(string fileName, ISOExtractor extractor)
        {
            var offset = extractor.TOC.LocationForFile(fileName);
            var size = extractor.TOC.SizeForFile(fileName);

            Filename = fileName;
            Path = extractor.ExtractPath;
            Region = extractor.Region;
            Game = extractor.Game;

            if (Configuration.Verbose)
            {
                Console.WriteLine($"Extracting {fileName}");
            }

            fileStream = File.Open($"{Path}/{Filename}", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            extractor.ISOStream.CopySubStream(fileStream, offset, size);
        }
        public bool IsCompressed(int index)
        {
            var flag = fileStream.GetUIntAtOffset(GetStartOffsetForFile(index));
            return flag == kLZSSbytes;
        }


        public int GetStartOffsetForFileDetails(int index)
        {
            return fileStream.GetIntAtOffset(kFirstFileDetailsPointerOffset + (index * 4));
        }

        public int GetStartOffsetForFile(int index)
        {
            var start = GetStartOffsetForFileDetails(index) + kFileStartPointerOffset;
            return fileStream.GetIntAtOffset(start);
        }

        public void SetStartOffsetForFile(int index, int newStart)
        {
            var start = GetStartOffsetForFile(index);
            fileStream.Seek(start, SeekOrigin.Begin);
            fileStream.Write(BitConverter.GetBytes(newStart));
        }
        
        public int GetSizeForFile(int index)
        {
            var start = GetStartOffsetForFileDetails(index) + kCompressedSizeOffset;
            return fileStream.GetIntAtOffset(start);
        }

        public void SetSizeForFile(int index, int newSize)
        {
            var start = GetSizeForFile(index);
            fileStream.Seek(start, SeekOrigin.Begin);
            fileStream.Write(BitConverter.GetBytes(newSize));
        }
        
        public string GetFilenameForFile(int index, bool clean = true)
        {
            var offset = UsesFileExtensions ? kFileDetailsFullFilenameOffset : kFileDetailsFilenameOffset;
            var start = fileStream.GetIntAtOffset(GetStartOffsetForFileDetails(index) + offset);
            return string.Join("", fileStream.GetStringAtOffset(start));
        }

        public int GetIDForFile(int index)
        {
            var start = GetStartOffsetForFileDetails(index) + kFileIdentifierOffset;
            return fileStream.GetIntAtOffset(start);
        }

        public FileTypes GetFileTypeForFile(int index)
        {
            var id = (GetIDForFile(index) & 0xFF00) >> 8;
            return (FileTypes)Enum.ToObject(typeof(FileTypes), id);
        }
    }
}
