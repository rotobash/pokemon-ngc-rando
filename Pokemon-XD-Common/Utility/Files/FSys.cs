using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDCommon.Contracts;

namespace XDCommon.Utility
{
    public class FSys : BaseExtractedFile
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

        public Dictionary<string, IExtractedFile> ExtractedEntries = new Dictionary<string, IExtractedFile>();


        public int GroupID
        {
            get
            {
                return ExtractedFile.GetIntAtOffset(kFSYSGroupIDOffset);
            }
            set
            {
                ExtractedFile.Seek(kFSYSGroupIDOffset, SeekOrigin.Begin);
                ExtractedFile.Write(value.GetBytes());
            }
        }

        public int NumberOfEntries
        {
            get
            {
                return ExtractedFile.GetIntAtOffset(kNumberOfEntriesOffset);
            }
            set
            {
                ExtractedFile.Seek(kNumberOfEntriesOffset, SeekOrigin.Begin);
                ExtractedFile.Write(value.GetBytes());
            }
        }

        public override FileTypes FileType => FileTypes.FSYS;

        public int Offset { get; private set; }
        public int Size { get; private set; }

        public bool UsesFileExtensions => ExtractedFile.GetByteAtOffset(0x13) == 1;

        public FSys(string pathToFile)
        {
            ExtractedFile = File.Open(pathToFile, FileMode.Open, FileAccess.ReadWrite);
            var fileParts = pathToFile.Split("/");
            FileName = fileParts.Last();
            Path = string.Join("/", fileParts.Take(fileParts.Length - 1));
        }

        public FSys(string path, string fileName, ISO iso)
        {

            FileName = fileName;
            Path = iso.Path;
            Offset = iso.TOC.LocationForFile(fileName);
            Size = iso.TOC.SizeForFile(fileName);

            if (Configuration.Verbose)
            {
                Console.WriteLine($"Extracting {fileName}");
            }

            ExtractedFile = $"{Path}/{FileName}".GetNewStream();
            iso.ExtractedFile.CopySubStream(ExtractedFile, Offset, Size);
        }

        public bool IsCompressed(int index)
        {
            var flag = ExtractedFile.GetUIntAtOffset(GetStartOffsetForFile(index));
            return flag == kLZSSbytes;
        }

        public int GetStartOffsetForFileDetails(int index)
        {
            return ExtractedFile.GetIntAtOffset(kFirstFileDetailsPointerOffset + (index * 4));
        }

        public int GetStartOffsetForFile(int index)
        {
            var start = GetStartOffsetForFileDetails(index) + kFileStartPointerOffset;
            return ExtractedFile.GetIntAtOffset(start);
        }

        public void SetStartOffsetForFile(int index, int newStart)
        {
            var start = GetStartOffsetForFile(index);
            ExtractedFile.Seek(start, SeekOrigin.Begin);
            ExtractedFile.Write(newStart.GetBytes());
        }
        
        public int GetSizeForFile(int index)
        {
            var offset = IsCompressed(index) ? kCompressedSizeOffset : kUncompressedSizeOffset;
            var start = GetStartOffsetForFileDetails(index) + offset;
            return ExtractedFile.GetIntAtOffset(start);
        }

        public void SetSizeForFile(int index, int newSize)
        {
            var offset = IsCompressed(index) ? kCompressedSizeOffset : kUncompressedSizeOffset;
            var start = GetStartOffsetForFileDetails(index) + offset;
            var originalSize = GetSizeForFile(index);

            ExtractedFile.Seek(start, SeekOrigin.Begin);
            ExtractedFile.Write(newSize.GetBytes());
            Size += newSize - originalSize;
        }
        
        public string GetFilenameForFile(int index, bool clean = true)
        {
            var offset = UsesFileExtensions ? kFileDetailsFullFilenameOffset : kFileDetailsFilenameOffset;
            var start = ExtractedFile.GetIntAtOffset(GetStartOffsetForFileDetails(index) + offset);
            return string.Join("", ExtractedFile.GetStringAtOffset(start));
        }

        public int GetIDForFile(int index)
        {
            var start = GetStartOffsetForFileDetails(index) + kFileIdentifierOffset;
            return ExtractedFile.GetIntAtOffset(start);
        }

        public int GetIndexForFileName(string fileName)
        {
            for (int i = 0; i < NumberOfEntries; i++)
            {
                var nameAtIndex = GetFilenameForFile(i);
                var fileType = GetFileTypeForFile(i);
                if (fileName.RemoveFileExtensions() == nameAtIndex.RemoveFileExtensions() && fileName.EndsWith(fileType.FileTypeName()))
                {
                    return i;
                }
            }
            return -1;
        }

        public FileTypes GetFileTypeForFile(int index)
        {
            var id = (GetIDForFile(index) & 0xFF00) >> 8;
            return (FileTypes)Enum.ToObject(typeof(FileTypes), id);
        }

        public IExtractedFile ExtractEntryByFileName(string filename)
        {
            if (ExtractedEntries.ContainsKey(filename))
            {
                return ExtractedEntries[filename];
            }
            else
            {
                var index = GetIndexForFileName(filename);
                var entry = FSysFileEntry.ExtractFromFSys(this, index);
                ExtractedEntries.Add(entry.FileName, entry);
                return entry;
            }
        }

        public IExtractedFile ExtractEntryByIndex(int index)
        {
            if (index < 0 || index > NumberOfEntries)
                return null;

            return ExtractEntryByFileName(GetFilenameForFile(index));
        }

        public override Stream Encode(bool _ = false)
        {
            Stream fSysStream = new MemoryStream();
            // copy the existing stream back, 
            ExtractedFile.CopyTo(fSysStream);
            if (ExtractedEntries.Count > 0)
            {
                for (int i = 0; i < ExtractedEntries.Count; i++)
                {
                    // write element, update its properties
                    var entry = ExtractedEntries.Values.ElementAt(i);
                    SetStartOffsetForFile(i, (int)fSysStream.Position);
                    SetSizeForFile(i, (int)entry.ExtractedFile.Length);
                    using var stream = entry.Encode(IsCompressed(i));
                    stream.CopyTo(fSysStream);
                }  
            }
            return fSysStream;
        }

        protected override void Dispose(bool isDisposing)
        {
            if (!disposedValue)
            {
                foreach (var file in ExtractedEntries.Values)
                {
                    file.Dispose();
                }
                ExtractedEntries.Clear();
                base.Dispose(isDisposing);
            }
        }
    }
}
