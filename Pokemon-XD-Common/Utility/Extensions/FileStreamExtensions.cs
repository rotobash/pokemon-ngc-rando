using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XDCommon.Utility
{
    public static class StreamExtensions
    {
        public static byte[] GetBytes(this int value)
        {
            var valBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(valBytes);
            }
            return valBytes;
        }
        public static byte[] GetBytes(this uint value)
        {
            var valBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(valBytes);
            }
            return valBytes;
        }
        public static byte[] GetBytes(this ushort value)
        {
            var valBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(valBytes);
            }
            return valBytes;
        }

        public static Stream GetNewStream(this string fullPath)
        {
            if (Configuration.UseMemoryStreams)
            {
                return new MemoryStream();
            }
            else
            {
                return File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
        }

        public static byte[] GetBytesAtOffset(this Stream stream, long offset, int length)
        {
            byte[] bytes = new byte[length];
            if (offset >= 0 && length + offset <= stream.Length)
            {
                stream.Seek(offset, SeekOrigin.Begin);
                var bytesRead = 0;
                while(bytesRead < length)
                {
                    bytesRead = stream.Read(bytes, bytesRead, length - bytesRead);
                }
            }

            if (BitConverter.IsLittleEndian && bytes.Length > 1)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        public static byte[] GetNibbleAtOffset(this Stream stream, long offset, int length)
        {
            byte[] bytes = new byte[length];
            if (offset > 0 && length + offset < stream.Length)
            {
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Read(bytes);
            }
            return bytes;
        }
        
        public static byte GetByteAtOffset(this Stream stream, long offset)
        {
            byte[] bytes = GetBytesAtOffset(stream, offset, 1);
            return bytes[0];
        }
        
        public static ushort GetUShortAtOffset(this Stream stream, long offset)
        {
            byte[] bytes = GetBytesAtOffset(stream, offset, 2);
            return BitConverter.ToUInt16(bytes);
        }
        
        public static uint GetUIntAtOffset(this Stream stream, long offset)
        {
            byte[] bytes = GetBytesAtOffset(stream, offset, 4);
            return BitConverter.ToUInt32(bytes);
        }
        
        public static ulong GetULongAtOffset(this Stream stream, long offset)
        {
            byte[] bytes = GetBytesAtOffset(stream, offset, 8);
            return BitConverter.ToUInt64(bytes);
        }
        
        public static sbyte GetSByteAtOffset(this Stream stream, long offset)
        {
            return (sbyte)GetByteAtOffset(stream, offset);
        }

        public static short GetShortAtOffset(this Stream stream, long offset)
        {
            byte[] bytes = GetBytesAtOffset(stream, offset, 2);
            return BitConverter.ToInt16(bytes);
        }
        
        public static int GetIntAtOffset(this Stream stream, long offset)
        {
            byte[] bytes = GetBytesAtOffset(stream, offset, 4);
            return BitConverter.ToInt32(bytes);
        }
        
        public static long GetLongAtOffset(this Stream stream, long offset)
        {
            byte[] bytes = GetBytesAtOffset(stream, offset, 8);
            return BitConverter.ToInt64(bytes);
        }
        
        public static char GetCharAtOffset(this Stream stream, long offset)
        {
            return (char)GetByteAtOffset(stream, offset);
        }
        
        public static void WriteByteAtOffset(this Stream stream, long offset, byte writeByte)
        {
            stream.Seek(offset, SeekOrigin.Begin);
            stream.WriteByte(writeByte);
        }
        
        public static void WriteBytesAtOffset(this Stream stream, long offset, byte[] writeBytes)
        {
            int bytesWrittenTotal = 0;
            stream.Seek(offset, SeekOrigin.Begin);
            do
            {
                var oldPosition = stream.Position;
                var bytesToWrite = Math.Min(writeBytes.Length - bytesWrittenTotal, writeBytes.Length);
                stream.Write(writeBytes, bytesWrittenTotal, bytesToWrite);
                bytesWrittenTotal += (int)(stream.Position - oldPosition);

            } while (bytesWrittenTotal < writeBytes.Length);
        }

        public static void CopySubStream(this Stream input, Stream output, long start, long length)
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

                output.Write(buffer, 0, bytesRead);
                bytesReadTotal += bytesRead;
            } while (bytesReadTotal < length);
        }

        public static Stream InsertIntoStream(this FileStream stream, long offset, byte[] data)
        {
            // you can't really insert into a stream without pulling it entirely into memory, so cheat a bit
            var streamFileName = stream.Name;
            var newStream = $"{streamFileName}.bak".GetNewStream();

            // write any pending changes
            // copy old stream into new stream up to offset
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopySubStream(newStream, 0, offset);

            // copy our new data
            newStream.Seek(offset, SeekOrigin.Begin);
            newStream.Write(data);

            // copy rest of oldstream
            stream.CopySubStream(newStream, offset, (int)stream.Length - offset);

            // dispose of old stream
            stream.Dispose();
            // flush new stream and close it
            newStream.Flush();
            newStream.Dispose();

            if (!Configuration.UseMemoryStreams)
            {
                File.Delete(streamFileName);
                File.Move($"{streamFileName}.bak", streamFileName);
            }

            return streamFileName.GetNewStream();
        }
        
        public static Stream DeleteFromStream(this FileStream stream, long offset, int length)
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

            if (!Configuration.UseMemoryStreams)
            {
                File.Delete(streamFileName);
                File.Move($"{streamFileName}.bak", streamFileName);
            }

            return streamFileName.GetNewStream();
        }

        public static List<IUnicodeCharacters> GetStringAtOffset(this Stream stream, long offset)
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
