﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XDCommon.Utility
{
    public static class StreamExtensions
    {
        public static byte[] GetBytesAtOffset(this Stream stream, int offset, int length)
        {
            byte[] bytes = new byte[length];
            if (offset >= 0 && length + offset < stream.Length)
            {
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Read(bytes);
            }

            if (BitConverter.IsLittleEndian && bytes.Length > 1)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        public static byte[] GetNibbleAtOffset(this Stream stream, int offset, int length)
        {
            byte[] bytes = new byte[length];
            if (offset > 0 && length + offset < stream.Length)
            {
                stream.Read(bytes, offset, length);
            }
            return bytes;
        }
        
        public static byte GetByteAtOffset(this Stream stream, int offset)
        {
            byte[] bytes = GetBytesAtOffset(stream, offset, 1);
            return bytes[0];
        }
        
        public static ushort GetUShortAtOffset(this Stream stream, int offset)
        {
            byte[] bytes = GetBytesAtOffset(stream, offset, 4);
            return BitConverter.ToUInt16(bytes);
        }
        
        public static uint GetUIntAtOffset(this Stream stream, int offset)
        {
            byte[] bytes = GetBytesAtOffset(stream, offset, 4);
            return BitConverter.ToUInt32(bytes);
        }
        
        public static ulong GetULongAtOffset(this Stream stream, int offset)
        {
            byte[] bytes = GetBytesAtOffset(stream, offset, 8);
            return BitConverter.ToUInt64(bytes);
        }
        
        public static sbyte GetSByteAtOffset(this Stream stream, int offset)
        {
            return (sbyte)GetByteAtOffset(stream, offset);
        }

        public static short GetShortAtOffset(this Stream stream, int offset)
        {
            byte[] bytes = GetBytesAtOffset(stream, offset, 2);
            return BitConverter.ToInt16(bytes);
        }
        
        public static int GetIntAtOffset(this Stream stream, int offset)
        {
            byte[] bytes = GetBytesAtOffset(stream, offset, 4);
            return BitConverter.ToInt32(bytes);
        }
        
        public static long GetLongAtOffset(this Stream stream, int offset)
        {
            byte[] bytes = GetBytesAtOffset(stream, offset, 8);
            return BitConverter.ToInt64(bytes);
        }
        
        public static char GetCharAtOffset(this Stream stream, int offset)
        {
            return (char)GetByteAtOffset(stream, offset);
        }

        public static void CopySubStream(this Stream input, Stream output, int start, int length)
        {
            var bytesReadTotal = 0;
            var bufferSize = 64 * 1024;
            input.Seek(start, SeekOrigin.Begin);
            output.Seek(0, SeekOrigin.Begin);
            do
            {
                var bytesToRead = Math.Min(length - bytesReadTotal, bufferSize);
                var buffer = new byte[bytesToRead];
                var bytesRead = input.Read(buffer);
                if (bytesRead == 0)
                    break;
                output.Write(buffer);
                bytesReadTotal += bytesRead;
            } while (bytesReadTotal < length);
        }

        public static Stream InsertIntoStream(this FileStream stream, int offset, byte[] data)
        {
            // you can't really insert into a stream without pulling it entirely into memory, so cheat a bit
            var streamFileName = stream.Name;
            var newStream = File.Create($"{streamFileName}.bak");

            // write any pending changes
            // copy old stream into new stream up to offset
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopySubStream(newStream, 0, offset);

            // copy our new data
            newStream.Write(data, offset, data.Length);

            // copy rest of oldstream
            stream.CopySubStream(newStream, offset, (int)stream.Length - offset);

            // dispose of old stream
            stream.Dispose();
            // flush new stream and close it
            newStream.Flush();
            newStream.Dispose();

            File.Delete(streamFileName);
            File.Move($"{streamFileName}.bak", streamFileName);

            return File.Open(streamFileName, FileMode.Append);
        }
        
        public static Stream DeleteFromStream(this FileStream stream, int offset, int length)
        {
            // you can't really insert into a stream without pulling it entirely into memory, so cheat a bit
            var streamFileName = stream.Name;
            var newStream = File.Create($"{streamFileName}.bak");

            // write any pending changes
            // copy old stream into new stream up to offset
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopySubStream(newStream, 0, offset);
            stream.CopySubStream(newStream, offset + length, (int)stream.Length - (offset + length));

            // dispose of old stream
            stream.Dispose();
            // flush new stream and close it
            newStream.Flush();
            newStream.Dispose();

            File.Delete(streamFileName);
            File.Move($"{streamFileName}.bak", streamFileName);

            return File.Open(streamFileName, FileMode.Open, FileAccess.ReadWrite);
        }

        public static List<IUnicodeCharacters> GetStringAtOffset(this Stream stream, int offset)
        {
            byte currentByte;
            var currentOffset = offset;
            List<IUnicodeCharacters> chars = new List<IUnicodeCharacters>();

            while ((currentByte = GetByteAtOffset(stream, currentOffset)) != 0)
            {
                chars.Add(new UnicodeCharacters(currentByte));
                currentOffset += 1;
            }
            return chars;
        }

        public static IEnumerable<int> OccurencesOfBytes(this Stream stream, int marker)
        {
            var offsets = new List<int>();
            stream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < stream.Length; i++)
            {
                var checkBytes = stream.GetIntAtOffset(i);
                if (checkBytes == marker)
                {
                    offsets.Add(i);
                    i += 4;
                }
            }

            return offsets;
        }
    }
}