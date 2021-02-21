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
        const byte FSYSGroupIDOffset = 0x08;
        const byte NumberOfEntriesOffset = 0x0C;
        const byte FSYSFileSizeOffset = 0x20;
        const byte FirstFileNamePointerOffset = 0x44;
        const byte FirstFileOffset = 0x48;
        const byte FirstFileDetailsPointerOffset = 0x60;
        const byte FirstFileNameOffset = 0x70;

        const byte FileIdentifierOffset = 0x00; // 3rd byte is the file format, 1st half is an arbitrary identifier
        const byte FileFormatOffset = 0x02;
        const byte FileStartPointerOffset = 0x04;
        const byte UncompressedSizeOffset = 0x08;
        const byte CompressedSizeOffset = 0x14;
        const byte FileDetailsFullFilenameOffset = 0x1C; // includes file extension. Not always used.
        const byte FileFormatIndexOffset = 0x20; // half of value in byte 3
        const byte FileDetailsFilenameOffset = 0x24;

        const uint TCODbytes = 0x54434F44;
        const uint FSYSbytes = 0x46535953;
        const ushort USbytes = 0x5553;
        const ushort JPbytes = 0x4A50;

        public Dictionary<string, IExtractedFile> ExtractedEntries = new Dictionary<string, IExtractedFile>();


        public int GroupID
        {
            get
            {
                return ExtractedFile.GetIntAtOffset(FSYSGroupIDOffset);
            }
            set
            {
                ExtractedFile.Seek(FSYSGroupIDOffset, SeekOrigin.Begin);
                ExtractedFile.Write(value.GetBytes());
            }
        }

        public int NumberOfEntries
        {
            get
            {
                return ExtractedFile.GetIntAtOffset(NumberOfEntriesOffset);
            }
            set
            {
                ExtractedFile.Seek(NumberOfEntriesOffset, SeekOrigin.Begin);
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

        public FSys(string fileName, ISO iso)
        {

            FileName = fileName;
            Path = iso.Path;

            var fileEntry = iso.TOC.GetFileEntry(fileName);
            Offset = (int)fileEntry.FileDataOffset;
            Size = (int)fileEntry.Size;

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
            return flag == LZSSEncoder.LZSSbytes;
        }

        public int GetStartOffsetForFileDetails(int index)
        {
            return ExtractedFile.GetIntAtOffset(FirstFileDetailsPointerOffset + (index * 4));
        }

        public int GetStartOffsetForFile(int index)
        {
            var start = GetStartOffsetForFileDetails(index) + FileStartPointerOffset;
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
            var offset = IsCompressed(index) ? CompressedSizeOffset : UncompressedSizeOffset;
            var start = GetStartOffsetForFileDetails(index) + offset;
            return ExtractedFile.GetIntAtOffset(start);
        }

        public void SetSizeForFile(int index, int newSize)
        {
            var offset = IsCompressed(index) ? CompressedSizeOffset : UncompressedSizeOffset;
            var start = GetStartOffsetForFileDetails(index) + offset;
            var originalSize = GetSizeForFile(index);

            ExtractedFile.Seek(start, SeekOrigin.Begin);
            ExtractedFile.Write(newSize.GetBytes());
            Size += newSize - originalSize;
        }
        
        public string GetFilenameForFile(int index, bool clean = true)
        {
            var offset = UsesFileExtensions ? FileDetailsFullFilenameOffset : FileDetailsFilenameOffset;
            var start = ExtractedFile.GetIntAtOffset(GetStartOffsetForFileDetails(index) + offset);
            return string.Join("", ExtractedFile.GetStringAtOffset(start));
        }

        public int GetIDForFile(int index)
        {
            var start = GetStartOffsetForFileDetails(index) + FileIdentifierOffset;
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

        public IExtractedFile GetEntryByFileName(string filename)
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

        public IExtractedFile GetEntryByIndex(int index)
        {
            if (index < 0 || index > NumberOfEntries)
                return null;

            return GetEntryByFileName(GetFilenameForFile(index));
        }

        public override Stream Encode(bool _ = false)
        {
            Stream fSysStream = new MemoryStream();
            // copy the existing stream back,
            ExtractedFile.Seek(0, SeekOrigin.Begin);
            ExtractedFile.CopyTo(fSysStream);
            if (ExtractedEntries.Count > 0)
            {
                for (int i = 0; i < ExtractedEntries.Count; i++)
                {
                    // write element, update its properties
                    var entry = ExtractedEntries.Values.ElementAt(i);
                    var index = GetIndexForFileName(entry.FileName);
                    var offset = GetStartOffsetForFile(index);
                    var size = GetSizeForFile(index);

                    using var encodeStream = entry.Encode(IsCompressed(index));
                    fSysStream.Seek(offset, SeekOrigin.Begin);
                    encodeStream.CopyTo(fSysStream);

                    // don't mess up our offsets, not sure if this is a good idea or not...
                    int padBytes = (int)encodeStream.Position;
                    while (padBytes < size)
                    {
                        fSysStream.WriteByte(0);
                        padBytes++;
                    }
                }  
            }
            fSysStream.Seek(0, SeekOrigin.Begin);
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
