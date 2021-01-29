using System;
using System.Collections.Generic;
using System.IO;

namespace XDCommon.Utility
{
    public class Texture
    {
        public Texture(FSysFileEntry data)
        {

        }

        public void WritePNGData()
        {

        }
    }

    struct GSWMetadata
    {
        public int Id;
        public int StartOffset;
        public int DataSize;
    }

    public class GSWTexture
    {
        FSysFileEntry fileData;
        List<GSWMetadata> metadata;
        public GSWTexture(FSysFileEntry data)
        {
            var numberOfIds = data.ExtractedFile.GetUShortAtOffset(0x18);
            fileData = data;
            metadata = new List<GSWMetadata>();

            for (int i = 1; i < numberOfIds && i <= 0xFF; i++)
            {
                var search = SearchForID(i);
                if (search.HasValue)
                {
                    metadata.Add(search.Value);
                }
            }
        }

        GSWMetadata? SearchForID(int id)
        {
            var headerMarker = new byte[] { 3, 0, 0, (byte)id };
            var offsets = fileData.ExtractedFile.OccurencesOfBytes(BitConverter.ToInt32(headerMarker));
            foreach (var offset in offsets)
            {
                if (fileData.ExtractedFile.GetUShortAtOffset(offset + 0x24) == 0x0801)
                {
                    var formatID = fileData.ExtractedFile.GetIntAtOffset(offset + 0x28);

                    var palletteSize = fileData.ExtractedFile.GetIntAtOffset(offset + 0x68);
                    var textureSize = palletteSize + 0x200;
                    var textureStart = offset + 0x20;
                    return new GSWMetadata
                    {
                        Id = id,
                        StartOffset = textureSize,
                        DataSize = textureStart
                    };
                }
            }
            return null;
        }

        public IEnumerable<Texture> ExtractTextureData()
        {
            var textures = new List<Texture>();
            foreach (var info in metadata)
            {
                var filename = $"{fileData.Path}/{fileData.FileName.RemoveFileExtensions()}{info.Id}.gtx";
                var extractedFile = File.Open(filename, FileMode.Create, FileAccess.ReadWrite);
                fileData.ExtractedFile.CopySubStream(extractedFile, info.StartOffset, info.DataSize);
                extractedFile.Flush();
                
                var entry = new FSysFileEntry
                {
                    Path = fileData.Path,
                    FileName = filename,
                    FileType = FileTypes.GTX,
                    ExtractedFile = extractedFile
                };
                textures.Add(new Texture(entry));
            }
            return textures;
        }
    }
}
