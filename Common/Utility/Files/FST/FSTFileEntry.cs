using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Utility
{
    public class FSTFileEntry : FSTEntry
    {
        public uint FileDataOffset { get; set; }
        public uint Size { get; set; }

        public FSTFileEntry(int fileEntry, uint nameOffset, uint offset, uint size)
        {
            FileDataOffset = offset;
            NameOffset = nameOffset;
            Size = size;
            FileEntry = fileEntry;
        }

        public override FSTFileEntry SearchForFile(string fileName)
        {
            return Name.ToString() == fileName ? this : null;
        }

        public override FSTFileEntry SearchForFile(int fileEntry)
        {
            return FileEntry == fileEntry ? this : null;
        }
    }
}
