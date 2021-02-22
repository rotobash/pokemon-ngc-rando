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
            var fileDetails = fSys.GetDetailsForFile(index);
            var noExtFsysName = fSys.FileName.RemoveFileExtensions();
            var extractDir = $"{fSys.Path}/{noExtFsysName}";

            var fileName = fileDetails.FileName.ToString();
            if (!fSys.UsesFileExtensions || fileName == fileName.RemoveFileExtensions())
            {
                fileName = $"{fileName.RemoveFileExtensions()}{fileDetails.Filetype.FileTypeName()}";
            }

            if (fSys.ExtractedEntries.ContainsKey(fileName))
            {
                return fSys.ExtractedEntries[fileName];
            }

            if (Configuration.Verbose)
            {
                Console.WriteLine($"Extracting {fileName}");
            }

            var extractedFile = $"{extractDir}/{fileName}".GetNewStream();
            fSys.ExtractedFile.CopySubStream(extractedFile, fileDetails.StartOffset, fileDetails.IsCompressed ? fileDetails.CompressedSize : fileDetails.UncompressedSize);
            extractedFile.Flush();

            if (fileDetails.IsCompressed)
            {
                if (Configuration.Verbose)
                {
                    Console.WriteLine($"Decoding {fileName}");
                }
                extractedFile.Seek(kSizeOfLZSSHeader, SeekOrigin.Begin);
                extractedFile = LZSSEncoder.Decode(extractedFile);
            }

            return CreateExtractedFile(extractDir, fileName, fileDetails.Filetype, extractedFile);
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
            Stream entryStream;
            ExtractedFile.Seek(0, SeekOrigin.Begin);

            if (isCompressed)
            {
                var tempStream = $"{Path}/{FileName}.lzss".GetNewStream();
                ExtractedFile.CopyTo(tempStream);
                tempStream.Flush();
                tempStream.Seek(0, SeekOrigin.Begin);

                var encoder = new LZSSEncoder();
                entryStream = encoder.Encode(tempStream);
            }
            else
            {
                entryStream =  $"{Path}/{FileName}.lzss".GetNewStream();
                ExtractedFile.CopyTo(entryStream);
            }

            entryStream.AlignStream(0x10);
            entryStream.Flush();
            entryStream.Seek(0, SeekOrigin.Begin);
            return entryStream;
        }
    }
}
