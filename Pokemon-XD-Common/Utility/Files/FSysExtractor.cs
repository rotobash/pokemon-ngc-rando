using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XDCommon.Contracts;

namespace XDCommon.Utility
{
    public static class FSysExtractor
    {
        public static void ExtractFSys(FSys sys, bool decode)
        {
            if (sys.NumberOfEntries <= 0)
                return;

            var extractDir = $"{sys.Path}/{sys.Filename.RemoveFileExtensions()}";
            if (!Configuration.UseMemoryStreams)
            {
                if (!Directory.Exists(extractDir))
                {
                    Directory.CreateDirectory(extractDir);
                }
                else
                {
                    Directory.Delete(extractDir, true);
                    Directory.CreateDirectory(extractDir);
                }
            }

            for (int i = 0; i < sys.NumberOfEntries; i++)
            {
                var entry = FSysFileEntry.ExtractFromFSys(sys, i);
                sys.ExtractedEntries.Add(entry.FileName, entry);
                switch (entry.FileType)
                {
                    case FileTypes.GSW:
                    {
                        var gswTex = (GSWTexture)entry;
                        var texData = gswTex.ExtractTextureData();
                        foreach (var tex in texData)
                        {
                            sys.ExtractedEntries.Add(tex.FileName, tex);
                        }
                    }
                    break;

                }
            }

            if (decode)
            {
                var extraData = new List<IExtractedFile>();
                for (int i = 0; i < sys.ExtractedEntries.Count; i++)
                {
                    var entry = sys.ExtractedEntries.Values.ElementAt(i);
                    if (entry is Texture tex)
                    {
                        tex.WritePNGData();
                    }
                    else if (entry is PKX pk)
                    {
                        sys.ExtractedEntries.Add(pk.FileName.GetSafeFileName(pk.Path, FileTypes.DAT), pk.ExtractDat());
                    }
                    else if (entry is StringTable tbl)
                    {

                    }
                    else if (entry is SCD scr)
                    {
                        scr.WriteScriptData(sys);
                    }
                    else if (entry.FileType == FileTypes.THH)
                    {
                        var thdData = sys.ExtractedEntries.Values.First(f =>
                        {
                            var entryFileName = f.FileName.Split(".")[0];
                            return f.FileType == FileTypes.THD && f.FileName.Contains(entryFileName);
                        });
                        var thp = new THP();
                        sys.ExtractedEntries.Add(entry.FileName.GetSafeFileName(entry.Path, FileTypes.THP), thp);
                    }
                }
            }
        }
    }
}
