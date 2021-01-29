using System;
using System.Collections.Generic;
using System.IO;

namespace XDCommon.Utility
{
    public static class FSysExtractor
    {
        public static void ExtractFSys(FSys sys, bool decode)
        {
            var extractDir = $"{sys.Path}/{sys.Filename.RemoveFileExtensions()}";
            if (!Directory.Exists(extractDir))
            {
                Directory.CreateDirectory(extractDir);
            }
            else
            {
                Directory.Delete(extractDir, true);
                Directory.CreateDirectory(extractDir);
            }

            for (int i = 0; i < sys.NumberOfEntries; i++)
            {
                sys.extractedEntries.Add(FSysFileEntry.ExtractFromFSys(sys, i));
            }

            if (decode)
            {
                var textures = new List<Texture>();
                foreach (var entry in sys.extractedEntries)
                {
                    switch (entry.FileType)
                    {
                        case FileTypes.GTX:
                        case FileTypes.ATX:
                        case FileTypes.GSW:
                        {
                            if (entry.FileType == FileTypes.GSW)
                            {
                                textures.AddRange(
                                    new GSWTexture(entry).ExtractTextureData()
                                );
                            }
                            else
                            {
                                textures.Add(new Texture(entry));
                            }
                            break;
                        }

                        case FileTypes.PKX:
                            // pkxextractor
                            break;

                        case FileTypes.MSG:
                            // string table
                            break;

                        case FileTypes.SCD:
                            new SCD(entry).WriteScriptData(sys);
                            break;

                        case FileTypes.THH:
                        {
                            var thdData = sys.extractedEntries.Find(f =>
                            {
                                var entryFileName = f.FileName.Split(".")[0];
                                return f.FileType == FileTypes.THD && f.FileName.Contains(entryFileName);
                            });
                            var thp = new THP();
                            break;
                        }
                    }
                }

                if (textures.Count > 0)
                {
                    foreach (var tex in textures)
                    {
                        tex.WritePNGData();
                    }
                }
            }
        }
    }
}
