using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace XDCommon.Utility
{
    public static class LZSSDecoder
    {
        const int N = 4096;
        const int F = 18;
        const byte P = 2;
        public static Stream Decode(Stream file)
        {
            var slidingWindow = new byte[N];

            Stream outputStream;
            string filename = string.Empty;
            string outputFilename = string.Empty;
            if (file is FileStream fs)
            {
                filename = fs.Name;
                outputFilename = $"{filename}.bak";
            }
            outputStream = outputFilename.GetNewStream();

            // setup output stream
            file.Flush();
            file.Seek(0, SeekOrigin.Begin);

            int r = N - F;
            int flags = 0;

            while (file.Position < file.Length)
            {
                // endianess needs to be converted
                if ((flags & 0x100) == 0)
                {
                    flags = file.ReadByte();
                    flags |= 0xFF00;
                }
                
                if ((flags & 0x1) != 0)
                {
                    var b = (byte)file.ReadByte();
                    outputStream.WriteByte(b);
                    slidingWindow[r++] = b;
                    r &= N - 1;
                } 
                else
                {
                    int i = file.ReadByte();
                    int j = file.ReadByte();

                    i |= (j & 0xF0) << 4;
                    j = (j & 0x0F) + P;
                    var bytes = new byte[j + 1];
                    for (int k = 0; k <= j; k++)
                    {
                        int ind = (i + k) & (N - 1);
                        var b = slidingWindow[ind];
                        bytes[k] = b;
                        slidingWindow[r++] = b;
                        r &= N - 1;
                    }
                    outputStream.Write(bytes);
                }
                flags >>= 1;
            }

            outputStream.Flush();
            file.Dispose();

            if (file is FileStream)
            {
                outputStream.Dispose();
                File.Delete(filename);
                File.Move(outputFilename, filename);
                return filename.GetNewStream();
            }
            return outputStream;
        }
    }
}
