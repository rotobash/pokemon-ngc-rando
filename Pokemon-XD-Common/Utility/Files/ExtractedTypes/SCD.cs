using System;
using System.Collections.Generic;
using System.IO;

namespace XDCommon.Utility
{
    public class SCD
    {
        FSysFileEntry entry;
        public SCD(FSysFileEntry data)
        {
            entry = data;
        }

        public void WriteScriptData(FSys fSysFile)
        {
            FSysFileEntry rel = null;
            if (entry.FileType != FileTypes.REL)
            {
                rel = fSysFile.extractedEntries.Find(f => f.FileType == FileTypes.REL && entry.FileName.RemoveFileExtensions() == f.FileName.RemoveFileExtensions());
                if (rel == null)
                {
                    var extractDir = $"{fSysFile.Path}/{fSysFile.Filename.RemoveFileExtensions()}";
                    var fileName = entry.FileName.GetSafeFileName(extractDir, FileTypes.REL);
                    rel = new FSysFileEntry
                    {
                        Path = extractDir,
                        FileName = fileName,
                        FileType = FileTypes.REL,
                        ExtractedFile = File.Open($"{extractDir}/{fileName}", FileMode.Create, FileAccess.ReadWrite)
                    };
                }
            }

            var scriptData = new Script(entry);
            if (rel != null)
            {
                // maprel
            }
            
        }
    }
}
