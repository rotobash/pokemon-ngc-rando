using System;
using System.Collections.Generic;
using System.IO;
using XDCommon.Contracts;

namespace XDCommon.Utility
{
    public class FSysFileEntry : BaseExtractedFile
    {
        const byte kSizeOfLZSSHeader = 0x10;
        
        public static IExtractedFile ExtractFromFSys(FSys fSys, int index)
        {
            var offset = fSys.GetStartOffsetForFile(index);
            var size = fSys.GetSizeForFile(index);
            var noExtFsysName = fSys.FileName.RemoveFileExtensions();
            var extractDir = $"{fSys.Path}/{noExtFsysName}";
            var fileName = string.Join("", fSys.GetFilenameForFile(index));
            var isCompressed = fSys.IsCompressed(index);
            var fileType = fSys.GetFileTypeForFile(index);

            if (!fSys.UsesFileExtensions || fileName == fileName.Split(".")[0])
            {
                fileName = $"{fileName.RemoveFileExtensions()}{fileType.FileTypeName()}";
            }

            if (Configuration.Verbose)
            {
                Console.WriteLine($"Extracting {fileName}");
            }

            var extractedFile = $"{extractDir}/{fileName}".GetNewStream();
            fSys.ExtractedFile.CopySubStream(extractedFile, offset, size);
            extractedFile.Flush();

            if (isCompressed)
            {
                if (Configuration.Verbose)
                {
                    Console.WriteLine($"Decoding {fileName}");
                }
                extractedFile.Seek(kSizeOfLZSSHeader, SeekOrigin.Begin);
                extractedFile = LZSSEncoder.Decode(extractedFile);
            }

            return CreateExtractedFile(extractDir, fileName, fileType, extractedFile);
        }

        public static IExtractedFile CreateExtractedFile(string extractDir, string fileName, FileTypes fileType, Stream extractedFile)
        {
            switch (fileType)
            {
                case FileTypes.GTX:
                case FileTypes.ATX:
                case FileTypes.GSW:
                {
                    if (fileType == FileTypes.GSW)
                    {
                        return new GSWTexture
                        {
                            Path = extractDir,
                            FileName = fileName,
                            ExtractedFile = extractedFile
                        };
                    }
                    else
                    {
                        return new Texture
                        {
                            FileType = fileType,
                            Path = extractDir,
                            FileName = fileName,
                            ExtractedFile = extractedFile
                        };
                    }
                }

                case FileTypes.PKX:
                    return new PKX
                    {
                        Path = extractDir,
                        FileName = fileName,
                        ExtractedFile = extractedFile
                    };

                case FileTypes.MSG:
                    return new StringTable(extractedFile)
                    {
                        Path = extractDir,
                        FileName = fileName
                    };

                case FileTypes.REL:
                    return new REL(fileName, extractDir, extractedFile);
                case FileTypes.SCD:
                    return new SCD
                    {
                        Path = extractDir,
                        FileName = fileName,
                        ExtractedFile = extractedFile
                    };
                default:
                    return new FSysFileEntry
                    {
                        FileType = fileType,
                        Path = extractDir,
                        FileName = fileName,
                        ExtractedFile = extractedFile
                    };
            }
        }

        public override Stream Encode(bool isCompressed)
        {
            Stream entryStream = new MemoryStream();
            ExtractedFile.Seek(0, SeekOrigin.Begin);
            ExtractedFile.CopyTo(entryStream);
            entryStream.Flush();
            entryStream.Seek(0, SeekOrigin.Begin);

            if (isCompressed)
            {
                var encoder = new LZSSEncoder();
                entryStream = encoder.Encode(entryStream);
                entryStream.Seek(0, SeekOrigin.Begin);
                using var newFile = File.Open(Configuration.ExtractDirectory + "/" + FileName, FileMode.Create, FileAccess.ReadWrite);
                entryStream.CopyTo(newFile);
                entryStream.Seek(0, SeekOrigin.Begin);
            }
            return entryStream;
        }
    }
}
