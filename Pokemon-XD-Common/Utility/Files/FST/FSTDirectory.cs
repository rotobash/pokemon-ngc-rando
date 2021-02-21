using System;
using System.Collections.Generic;
using System.Text;

namespace XDCommon.Utility
{
    public class FSTDirectory : FSTEntry
    {
        public int ParentEntry { get; set; }
        public List<FSTEntry> Children { get; }

        public int EndEntry => FileEntry + 1 + Children.Count;

        public FSTDirectory(int fileEntry, uint nameOffset, int parentEntry)
        {
            NameOffset = nameOffset;
            FileEntry = fileEntry;
            ParentEntry = parentEntry;
            Children = new List<FSTEntry>();
        }

        public FSTDirectory SearchForDirectory(string fileName)
        {
            if (Name.ToString() == fileName)
            {
                return this;
            }
            else if (Children.Count > 0)
            {
                foreach (var child in Children)
                {
                    if (child is FSTDirectory dir)
                    {
                        var candidate = dir.SearchForDirectory(fileName);
                        if (candidate != null)
                            return candidate;
                    }
                }
            }
            return null;
        }

        public override FSTFileEntry SearchForFile(string fileName)
        {
            if (Children.Count > 0)
            {
                foreach (var child in Children)
                {
                    var candidate = child.SearchForFile(fileName);
                    if (candidate != null)
                        return candidate;
                }
            }
            return null;
        }

        public override FSTFileEntry SearchForFile(int fileEntry)
        {
            if (Children.Count > 0)
            {
                foreach (var child in Children)
                {
                    var candidate = child.SearchForFile(fileEntry);
                    if (candidate != null)
                        return candidate;
                }
            }
            return null;
        }
    }
}
