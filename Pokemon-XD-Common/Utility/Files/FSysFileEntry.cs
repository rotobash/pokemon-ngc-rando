using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XDCommon.Utility
{
    public class FSysFileEntry
    {
        const byte kSizeOfLZSSHeader = 0x10;
        public string Path { get; set; }
        public string FileName { get; set; }
        public FileTypes FileType { get; set; }
        public Stream ExtractedFile;
        
        public static FSysFileEntry ExtractFromFSys(FSys fSys, int index)
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

            if (fileName == "common_rel")
            {
                fileName = "common";
            }

            var safeFileName = fileName.GetSafeFileName(extractDir, fileType);
            var extractedFile = $"{extractDir}/{safeFileName}".GetNewStream();
            fSys.fileStream.CopySubStream(extractedFile, offset, size);
            extractedFile.Flush();

            if (isCompressed)
            {
                if (Configuration.Verbose)
                {
                    Console.WriteLine($"Decoding {fileName}");
                }
                extractedFile = LZSSDecoder.Decode(extractedFile);
            }

            return new FSysFileEntry
            {
                FileType = fSys.GetFileTypeForFile(index),
                Path = extractDir,
                FileName = fileName,
                ExtractedFile = extractedFile
            };
        }
    }
}
