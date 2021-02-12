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
                rel = fSysFile.ExtractEntryByFileName($"{Path}/{FileName.RemoveFileExtensions()}.rel") as REL;
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
