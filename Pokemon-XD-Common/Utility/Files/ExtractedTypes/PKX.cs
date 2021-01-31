using System;
using System.Collections.Generic;
using System.Text;
using XDCommon.Contracts;

namespace XDCommon.Utility
{
    public class PKX: FSysFileEntry
    {
        const int Start = 0xE60;
        const int GPT1MagicNumber = 0x47505431;

        public PKX()
        {
            FileType = FileTypes.PKX;
        }

        public IExtractedFile ExtractDat()
        {
            var length = ExtractedFile.GetIntAtOffset(0);
            var gpt1Length = ExtractedFile.GetIntAtOffset(8);

            var startOffset = Start;
            if (gpt1Length > 0 && ExtractedFile.GetIntAtOffset(Start) == GPT1MagicNumber) /*GPT1 magic*/
            {
                startOffset += gpt1Length + 4;
            }
            var fileName = FileName.RemoveFileExtensions() + ".dat";
            var datFile = $"{Path}/{fileName}".GetNewStream();
            ExtractedFile.CopySubStream(datFile, startOffset, length);
            datFile.Flush();

            return new DAT
            {
                FileName = fileName,
                Path = Path,
                FileType = FileTypes.DAT,
                ExtractedFile = datFile
            };
        }
    }
}
