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
        public static FileStream Decode(FileStream file)
        {
            var slidingWindowSize = N - F;
            var slidingWindow = new byte[N];

            // setup output stream
            file.Flush();
            file.Seek(0, SeekOrigin.Begin);
            var outputStream = File.Open(file.Name + ".bak", FileMode.OpenOrCreate, FileAccess.Write);

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
            outputStream.Dispose();
            file.Dispose();

            File.Delete(file.Name);
            File.Move(outputStream.Name, file.Name);
            
            return File.Open(file.Name, FileMode.Open, FileAccess.ReadWrite);
        }
    }
}
