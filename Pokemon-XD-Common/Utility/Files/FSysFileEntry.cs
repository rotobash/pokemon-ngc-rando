using System;
using System.Collections.Generic;
using System.IO;
using XDCommon.Contracts;

namespace XDCommon.Utility
{
    public class FSysFileEntry: IExtractedFile
    {
        const byte kSizeOfLZSSHeader = 0x10;
        public string Path { get; set; }
        public string FileName { get; set; }
        public FileTypes FileType { get; set; }
        public Stream ExtractedFile { get; internal set; }
        
        public static IExtractedFile ExtractFromFSys(FSys fSys, int index)
        {
            var offset = fSys.GetStartOffsetForFile(index);
            var size = fSys.GetSizeForFile(index);
            var noExtFsysName = fSys.Filename.RemoveFileExtensions();
            var extractDir = $"{fSys.Path}/{noExtFsysName}";
            var fileName = string.Join("", fSys.GetFilenameForFile(index));
            var isCompressed = fSys.IsCompressed(index);
            var fileType = fSys.GetFileTypeForFile(index);

            if (!fSys.UsesFileExtensions || fileName == fileName.Split(".")[0])
            {
                fileName = $"{fileName.Split(".")[0]}{fileType.FileTypeName()}";
            }

            if (Configuration.Verbose)
            {
                Console.WriteLine($"Extracting {fileName}");
            }

            var safeFileName = fileName.GetSafeFileName(extractDir, fileType);
            var extractedFile = $"{extractDir}/{safeFileName}".GetNewStream();
            fSys.ExtractedFile.CopySubStream(extractedFile, offset, size);
            extractedFile.Flush();

            if (isCompressed)
            {
                if (Configuration.Verbose)
                {
                    Console.WriteLine($"Decoding {fileName}");
                }
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
                    return new REL
                    {
                        Path = extractDir,
                        FileName = fileName,
                        ExtractedFile = extractedFile
                    };

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

        public Stream Encode(bool isCompressed)
        {
            var entryStream = new MemoryStream();
            ExtractedFile.CopyTo(entryStream);
            if (isCompressed)
            {
                LZSSEncoder.Encode(entryStream);
            }
            return entryStream;
        }
    }
}
