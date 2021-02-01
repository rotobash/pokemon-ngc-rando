using System;
using System.Collections.Generic;
using System.Linq;
using XDCommon.Contracts;

namespace XDCommon.Utility
{
    public class SCD : FSysFileEntry
    {
        public SCD()
        {
            FileType = FileTypes.SCD;
        }
        public void WriteScriptData(FSys fSysFile)
        {
            IExtractedFile rel = null;
            if (FileType != FileTypes.REL)
            {
                rel = fSysFile.ExtractedEntries.Values.FirstOrDefault(f => f.FileType == FileTypes.REL && FileName.RemoveFileExtensions() == f.FileName.RemoveFileExtensions());
                if (rel == null)
                {
                    var fileName = FileName.GetSafeFileName(Path, FileTypes.REL);
                    var relFile = $"{Path}/{fileName}".GetNewStream();
                    rel = new REL
                    {
                        Path = Path,
                        FileName = fileName,
                        FileType = FileTypes.REL,
                        ExtractedFile = relFile
                    };
                    fSysFile.ExtractedEntries.Add(fileName, rel);
                }
            }


            var scriptData = new Script
            {
                
            };

            if (rel != null)
            {
                // maprel
            }
            
        }
    }
}
