using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XDCommon.Utility
{
    public class FST : BaseExtractedFile
    {
        const int TOCStartOffsetLocation = 0x424;
        const int TOCFileSizeLocation = 0x428;
        const int TOCMaxFileSizeLocation = 0x42C;
        const int TOCNumberEntriesOffset = 0x8;
        const int TOCEntrySize = 0xC;

        FSTDirectory rootDirectory;

        public List<string> AllFileNames { get; private set; }
        public List<string> FilesOrdered { get; private set; }
        public uint TOCFirstStringOffset => ExtractedFile.GetUIntAtOffset(TOCNumberEntriesOffset) * TOCEntrySize;
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

        public uint StartDataOffset { get; private set; } = uint.MaxValue;
        public uint LastNameOffset { get; private set; } = uint.MinValue;
        public FST(string pathToExtractDirectory, ISOExtractor extractor)
        {
            Path = pathToExtractDirectory;
            FileName = "Game.toc";
            ExtractedFile = $"{Path}/{FileName}".GetNewStream();

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
            Load();
        }
        
        public FST(string pathToExtractDirectory)
        {
            Path = pathToExtractDirectory;
            FileName = "Game.toc";
            if (!File.Exists($"{Path}/{FileName}"))
            {
                throw new ArgumentException("ISOExtractor must be provided if file doesn't exist.");
            }

            var file = File.Open($"{Path}/{FileName}", FileMode.Open, FileAccess.ReadWrite);
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
            Load();
        }

        public void Load()
        {
            if (Configuration.Verbose)
            {
                Console.WriteLine($"Extracting TOC.");
            }

            rootDirectory = new FSTDirectory(0, 0, 0);
            var numberOfEntries = ExtractedFile.GetIntAtOffset(TOCNumberEntriesOffset);
            LoadFST(rootDirectory, numberOfEntries);

            if (Configuration.Verbose)
            {
                Console.WriteLine($"Finished reading FST.");
            }
        }

        void LoadFST(FSTDirectory root, int endEntryIndex, int firstEntryIndex = 1)
        {
            int index = firstEntryIndex;
            while (index < endEntryIndex)
            {
                var entryOffset = index * 12;
                var folder = ExtractedFile.GetByteAtOffset(entryOffset) == 1;

                FSTEntry fileEntry;
                uint nameOffset;
                if (folder)
                {
                    nameOffset = (ExtractedFile.GetUIntAtOffset(entryOffset) & 0x00FFFFFF);
                    var parentEntry = ExtractedFile.GetIntAtOffset(entryOffset + 4);
                    var endDirEntry = ExtractedFile.GetIntAtOffset(entryOffset + 8);
                    var directory = new FSTDirectory(index, nameOffset, parentEntry);

                    LoadFST(directory, endDirEntry, index + 1);
                    fileEntry = directory;
                    index += endDirEntry - index;
                }
                else
                {
                    nameOffset = ExtractedFile.GetUIntAtOffset(entryOffset);
                    var fileOffset = ExtractedFile.GetUIntAtOffset(entryOffset + 4);
                    var fileSize = ExtractedFile.GetUIntAtOffset(entryOffset + 8);
                    fileEntry = new FSTFileEntry(index, nameOffset, fileOffset, fileSize);
                    index++;
                    if (fileOffset < StartDataOffset)
                        StartDataOffset = fileOffset;
                }

                if (nameOffset > LastNameOffset)
                    LastNameOffset = nameOffset;

                var fileName = ExtractedFile.GetStringAtOffset(nameOffset + TOCFirstStringOffset);
                fileEntry.Name = fileName;
                root.Children.Add(fileEntry);
            }
        }

        public void AddFileToFST(string fileName, uint size, string inDirectory = "")
        {
            var addDir = rootDirectory;
            if (inDirectory != string.Empty)
            {
                addDir = rootDirectory.SearchForDirectory(inDirectory);
            }

            if (addDir == null)
                return;

            var lastEntry = GetFlattenedFST()
                .OrderBy(f =>
                {
                    return f is FSTFileEntry fe ? fe.FileDataOffset : 0;
                })
                .Last() as FSTFileEntry;

            var nameBytes = new UnicodeString(fileName.ToArray());
            var entryIndex = addDir.EndEntry + 1;
            var entryOffset = lastEntry.FileDataOffset + lastEntry.Size;
            var newEntry = new FSTFileEntry(entryIndex, LastNameOffset, entryOffset, size)
            {
                Name = nameBytes
            };
            LastNameOffset += (uint)nameBytes.Count;
        }

        /// <summary>
        /// Rearrange the FST layout to pack all files at the beginning and leave junk towards the end.
        /// For easier file entry adjusting/adding. Note: You must call RebuildFST to update the stream.
        /// </summary>
        public List<FSTEntry> ReorderFST()
        {
            var entries = GetFlattenedFST()
                .OrderBy(f =>
                {
                    return f is FSTFileEntry fe ? fe.FileDataOffset : 0;
                })
                .ToList();

            // we put directories at the start, so skip them
            int i = entries.FindIndex(f => f is FSTFileEntry) + 1;
            for (; i < entries.Count; i++)
            {
                var prevEntry = entries[i - 1] as FSTFileEntry;
                var currEntry = entries[i] as FSTFileEntry;

                if (prevEntry.FileDataOffset + prevEntry.Size < currEntry.FileDataOffset)
                {
                    // come a little closer
                    var m = (prevEntry.FileDataOffset + prevEntry.Size) % 2048;
                    currEntry.FileDataOffset = prevEntry.FileDataOffset + prevEntry.Size;
                    if (m != 0)
                    {
                        // align by 2048 byte sectors
                        // probably not necessary unless you intend to burn the ISO to disk...
                        m = 2048 - m;
                        prevEntry.Size += m;
                        currEntry.FileDataOffset += m;
                    }
                }
            }

            return entries;
        }

        /// <summary>
        /// Recreates FST.bin from the current directory state. I.e. recreate the underlying stream.
        /// </summary>
        public void RebuildFST()
        {
            ExtractedFile.Dispose();
            ExtractedFile = $"{Path}/{FileName}".GetNewStream();

            var entries = GetFlattenedFST();

            // write root dir
            ExtractedFile.WriteByte(1);
            ExtractedFile.WriteByte(0);
            ExtractedFile.WriteByte(0);
            ExtractedFile.WriteByte(0);
            ExtractedFile.Write(0.GetBytes());
            ExtractedFile.Write((entries.Count + 1).GetBytes());

            // instead of seeking back and forth, store name bytes as they come and write it at the end.
            var nameBytes = new List<byte>();
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                if (entry is FSTFileEntry fileEntry)
                {
                    ExtractedFile.Write(fileEntry.NameOffset.GetBytes());
                    ExtractedFile.Write(fileEntry.FileDataOffset.GetBytes());
                    ExtractedFile.Write(fileEntry.Size.GetBytes());
                }
                else if (entry is FSTDirectory dir)
                {
                    var nameOffsetBytes = dir.NameOffset.GetBytes();
                    nameOffsetBytes[0] = 1;

                    ExtractedFile.Write(nameOffsetBytes);
                    ExtractedFile.Write(dir.ParentEntry.GetBytes());
                    ExtractedFile.Write(dir.EndEntry.GetBytes());
                }

                var nameByteArray = entry.Name.ToByteArray();
                if (nameByteArray.Length > 0) 
                {
                    nameBytes.AddRange(nameByteArray);
                    nameBytes.Add(0);
                }
            }
            ExtractedFile.Write(nameBytes.ToArray());

            // fst.bin is aligned to the nearest 4 bytes
            var m = ExtractedFile.Length % 4;
            if (m > 0)
                ExtractedFile.Write(new byte[4 - m]);

            ExtractedFile.Flush();
        }

        public FSTFileEntry GetFileEntry(string fileName)
        {
            return rootDirectory.SearchForFile(fileName);
        }

        /// <summary>
        /// Get an in order list of FST entries (directories and files) as they would appear in fst.bin. Excludes the root directory.
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public List<FSTEntry> GetFlattenedFST(FSTDirectory root = null) 
        {
            if (root == null)
                root = rootDirectory;

            var fileEntries = new List<FSTEntry>();
            foreach (var fe in root.Children)
            {
                if (fe is FSTFileEntry file)
                {
                    fileEntries.Add(file);
                }
                else if (fe is FSTDirectory dir)
                {
                    fileEntries.Add(dir);
                    fileEntries.AddRange(GetFlattenedFST(dir));
                }
            }
            return fileEntries.OrderBy(f => f.FileEntry).ToList();
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
