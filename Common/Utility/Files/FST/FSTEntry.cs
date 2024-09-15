using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Utility
{
    public abstract class FSTEntry
    {
        public int FileEntry { get; protected set; }
        public uint NameOffset { get; protected set; }
        public string Name { get; set; }
        public abstract FSTFileEntry SearchForFile(string fileName);
        public abstract FSTFileEntry SearchForFile(int fileEntry);
    }
}
