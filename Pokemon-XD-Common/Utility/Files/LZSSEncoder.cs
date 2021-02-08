using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace XDCommon.Utility
{
    public static class LZSSEncoder
    {
        const byte EI = 12;
        const byte EJ = 4;
        const byte P = 2;

        public static void Encode(Stream file)
        {

        }

        public static Stream Decode(Stream file)
        {
            uint flags = 0;
            int N, F, rless;
            rless = P;
            N = 1 << EI;
            F = 1 << EJ;

            var slidingWindow = new byte[N];

            Stream outputStream;
            string filename = string.Empty;
            string outputFilename = string.Empty;
            if (file is FileStream fs)
            {
                filename = fs.Name;
                outputFilename = $"{filename}.bak";
            }

            // setup output stream
            outputStream = outputFilename.GetNewStream();

            int r = (N - F) - rless;
            N--;
            F--;

            while (true)
            {
                flags >>= 1;
                if ((flags & 0x100) == 0)
                {
                    var b = file.ReadByte();
                    if (b == -1) break;
                    flags = (uint)(b | 0xFF00);
                }
                
                if ((flags & 0x1) != 0)
                {
                    var b = file.ReadByte();
                    if (b == -1) break;
                    outputStream.WriteByte((byte)b);
                    slidingWindow[r++] = (byte)b;
                    r &= N;
                } 
                else
                {
                    int i = file.ReadByte();
                    if (i == -1) break;
                    int j = file.ReadByte();
                    if (j == -1) break;

                    i |= (j >> EJ) << 8;
                    j = (j & F) + P;
                    for (int k = 0; k <= j; k++)
                    {
                        var b = slidingWindow[((i + k) & N)];
                        outputStream.WriteByte(b);
                        slidingWindow[r++] = b;
                        r &= N;
                    }
                }
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
