using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace XDCommon.Utility
{
    public struct BinaryTreeMatch
    {
        public int Length;
        public int Value;
    }

    public static class LZSSEncoder
    {
        const byte EI = 12; // offset bits
        const byte EJ = 4; // length bits
        const byte P = 2; // threshold
        public const int N = 1 << EI; // window size
        const int F = 1 << EJ; // buffer size
        const int Uncoded = 1;
        const int Encoded = 0;

        private static BinaryTreeMatch FindMatch(
            BinaryTree root, int bufferPos, byte[] buffer, byte[] slidingWindow
        )
        {
            int length, compare;
            length = 0;

            var match = new BinaryTreeMatch
            {
                Length = 0,
                Value = 0
            };

            var offset = root;
            while (offset != null)
            {
                compare = slidingWindow[offset.Value] - buffer[bufferPos];
                if (compare == 0)
                {
                    while (compare == 0 && length < buffer.Length)
                    {
                        length++;
                        compare = slidingWindow[(offset.Value + length) % slidingWindow.Length] - buffer[(bufferPos + length) % buffer.Length];

                    }

                    if (length > match.Length)
                    {
                        match.Length = length;
                        match.Value = offset.Value;
                    }
                }

                if (length >= buffer.Length)
                {
                    match.Length = buffer.Length;
                }

                offset = compare > 0 ? offset.Left : offset.Right;
            }

            return match;
        }

        private static void ReplaceByte(this BinaryTree root, byte[] slidingWindow, int index, byte replacement)
        {
            var firstIndex = index < F + P
                ? (slidingWindow.Length + index) - (F + P)
                : index - (F + P);

            for (int i = 0; i <= F + P; i++)
            {
                var windowIndex = (firstIndex + i) % slidingWindow.Length;
                if (root?.Search(windowIndex) != null)
                {
                    root = root.Delete(windowIndex);
                }
            }

            slidingWindow[index] = replacement;

            for (int i = 0; i <= F + P; i++)
            {
                var windowIndex = (index + i) % slidingWindow.Length;
                root.Insert(windowIndex);
            }
        }

        public static Stream Encode(Stream file)
        {
            int i, len, windowPos, bufferPos, lastMatchLen;
            byte[] slidingWindow = new byte[N];
            byte[] buffer = new byte[F + P];
            bufferPos = windowPos = 0;

            // setup output stream
            Stream outputStream;
            string filename = string.Empty;
            string outputFilename = string.Empty;
            if (file is FileStream fs)
            {
                filename = fs.Name;
                outputFilename = $"{filename}.bak";
            }
            outputStream = outputFilename.GetNewStream();

            for (len = 0; len < F + P; len++)
            {
                var b = file.ReadByte();
                if (b < 0) break;
                buffer[len] = (byte)b;
            }

            if (len == 0) return outputStream;

            var bTree = new BinaryTree();
            do
            {
                var match = FindMatch(bTree, bufferPos, buffer, slidingWindow);
                if (match.Length > len)
                {
                    match.Length = len;
                }

                if (match.Length <= P)
                {
                    match.Length = 1;
                }
                else
                {
                }


                lastMatchLen = match.Length;
                for (i = 0; i < lastMatchLen; i++)
                {
                    var b = file.ReadByte();
                    if (b < 0) break;
                    ReplaceByte(bTree, slidingWindow, windowPos, buffer[bufferPos]);
                    slidingWindow[windowPos] = (byte)b;
                    windowPos = (windowPos + 1) % slidingWindow.Length;
                    bufferPos = (bufferPos + 1) % buffer.Length;
                }

                while (i < lastMatchLen)
                {
                    ReplaceByte(bTree, slidingWindow, windowPos, buffer[bufferPos]);

                    windowPos = (windowPos + 1) % slidingWindow.Length;
                    bufferPos = (bufferPos + 1) % buffer.Length;
                    len--;
                    i++;
                }

            } while (len > 0);
            return outputStream;
        }

        public static Stream Decode(Stream file)
        {
            uint flags = 0;
            int n, f, rless;
            n = N;
            f = F;
            rless = P;

            var slidingWindow = new byte[n];

            // setup output stream
            Stream outputStream;
            string filename = string.Empty;
            string outputFilename = string.Empty;
            if (file is FileStream fs)
            {
                filename = fs.Name;
                outputFilename = $"{filename}.bak";
            }
            outputStream = outputFilename.GetNewStream();

            int r = (n - f) - rless;
            n--;
            f--;

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
                    r &= n;
                }
                else
                {
                    int i = file.ReadByte();
                    if (i == -1) break;
                    int j = file.ReadByte();
                    if (j == -1) break;

                    i |= (j >> EJ) << 8;
                    j = (j & f) + P;
                    for (int k = 0; k <= j; k++)
                    {
                        var b = slidingWindow[((i + k) & n)];
                        outputStream.WriteByte(b);
                        slidingWindow[r++] = b;
                        r &= n;
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
                // bypass GetNewStream because it'll overwrite files
                return File.Open(filename, FileMode.Open, FileAccess.ReadWrite);
            }
            return outputStream;
        }
    }
}
