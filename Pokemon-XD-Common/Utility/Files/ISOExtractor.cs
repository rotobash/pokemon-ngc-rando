using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XDCommon.Contracts;

namespace XDCommon.Utility
{
    public class ISOExtractor : IDisposable
    {
        private bool disposedValue;

        public Stream ISOStream { get; }
        public string ExtractPath { get; private set; }


        // keep a list of the original offsets in the ISO stream in case we just need to copy something back
        private Dictionary<string, uint> oldOffsets;

        public ISOExtractor(string pathToISO)
        {
            if (!Configuration.UseMemoryStreams)
            {
                Directory.CreateDirectory(Configuration.ExtractDirectory);
            }
            ExtractPath = Configuration.ExtractDirectory;
            ISOStream = File.Open(pathToISO, FileMode.Open, FileAccess.ReadWrite);
        }

        public ISOExtractor(Stream isoStream)
        {
            ExtractPath = Configuration.ExtractDirectory;
            ISOStream = isoStream;
        }

        public ISO ExtractISO()
        {
            // read game and region
            ISOStream.Seek(0, SeekOrigin.Begin);
            int game = ISOStream.GetUShortAtOffset(1);
            var region = ISOStream.ReadByte();

            Game gameEnum;
            Region regionEnum;

            switch (game)
            {
                case (ushort)Game.Colosseum:
                case (ushort)Game.XD:
                    gameEnum = (Game)game;
                    break;
                default:
                    throw new Exception("Unsupported game!");
            }

            switch (region)
            {
                case (byte)Region.US:
                case (byte)Region.Europe:
                case (byte)Region.Japan:
                    regionEnum = (Region)region;
                    break;
                default:
                    throw new Exception("Unknown region!");
            }

            ExtractPath = $"{Configuration.ExtractDirectory}/{gameEnum}-{regionEnum}";
            if (!Directory.Exists(ExtractPath) && !Configuration.UseMemoryStreams)
            {
                Directory.CreateDirectory(ExtractPath);
            }

            var iso = new ISO(ISOStream, ExtractPath)
            {
                Game = gameEnum,
                Region = regionEnum,
                DOL = new DOL(ExtractPath, this),
                TOC =  new FST(ExtractPath, this)
            };

            oldOffsets = iso.TOC.GetFlattenedFST().ToDictionary(k => k.Name.ToString(), v => v is FSTFileEntry fe ? fe.FileDataOffset : 0);
            iso.CommonRel = iso.GetFSysFile("common.fsys").GetEntryByFileName("common_rel.rel") as REL; 
            iso.BuildStringTables();

            return iso;
        }
        public void RepackISO(ISO iso, string savePath)
        {
            if (!savePath.EndsWith(".iso"))
            {
                savePath = $"{savePath}.iso";
            }
            using var isoStream = File.Open(savePath, FileMode.Create, FileAccess.Write);

            // boot.bin, bi2.bin, appldr.bin
            ISOStream.Seek(0, SeekOrigin.Begin);
            ISOStream.CopySubStream(isoStream, 0, iso.DOL.Offset);

            // pack dol
            isoStream.Seek(iso.DOL.Offset, SeekOrigin.Begin);
            using var dolStream = iso.DOL.Encode();
            dolStream.CopyTo(isoStream);
            int alignment = AlignByteCount(iso.DOL.Offset + (int)iso.DOL.ExtractedFile.Length);
            isoStream.WriteBytesAtOffset(isoStream.Position, new byte[alignment]);

            if (iso.TOC.StartDataOffset == uint.MaxValue)
            {
                throw new Exception("This is awkward... For some reason the TOC was never loaded.");
            }

            // pack fsys files
            isoStream.Seek(iso.TOC.StartDataOffset, SeekOrigin.Begin);

            iso.TOC.ReorderFST();
            var fileEntries = iso.TOC.GetFlattenedFST().OrderBy(f => f is FSTFileEntry fe ? fe.FileDataOffset : 0).ToArray();

            for (int i = 0; i < fileEntries.Length; i++)
            {
                var newEntry = fileEntries[i];
                if (newEntry is FSTFileEntry fileEntry)
                {
                    var fsysStartOffset = fileEntry.FileDataOffset;
                    var fsysSize = fileEntry.Size;
                    var entryFileName = fileEntry.Name.ToString();

                    isoStream.Seek(fileEntry.FileDataOffset, SeekOrigin.Begin);
                    if (iso.Files.ContainsKey(entryFileName))
                    {
                        var fsys = iso.Files[entryFileName];
                        using var fsysStream = fsys.Encode();
                        if (fsysStream.Length != fileEntry.Size)
                        {
                            var adjustSize = fileEntry.Size - fsysStream.Length;
                            fileEntry.Size = (uint)fsysStream.Length;
                            for (int j = i + 1; j < fileEntries.Length; j++)
                            {
                                if (fileEntries[j] is FSTFileEntry fe)
                                {
                                    fe.FileDataOffset += (uint)adjustSize;
                                }
                            }
                        }
                        fsysStream.CopyTo(isoStream);
                    }
                    else
                    {
                        ISOStream.CopySubStream(isoStream, oldOffsets[entryFileName], fsysSize);
                    }
                }
            }

            if (isoStream.Position < ISOStream.Length)
            {
                isoStream.Write(new byte[ISOStream.Length - isoStream.Position]);
            }

            // pack FST after to account for updates
            iso.TOC.RebuildFST();
            isoStream.Seek(iso.TOC.Offset, SeekOrigin.Begin);
            using var tocstream = iso.TOC.Encode();
            tocstream.CopyTo(isoStream);
            if (isoStream.Position < iso.TOC.StartDataOffset)
            {
                isoStream.Write(new byte[iso.TOC.StartDataOffset - isoStream.Position]);
            }

            isoStream.Flush();
        }

        int AlignByteCount(int val, int alignment = 2048)
        {
            var m = val % alignment;
            return m == 0 ? 0 : alignment - m;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (ISOStream != null && ISOStream.CanSeek)
                {
                    ISOStream.Dispose();
                }
                disposedValue = true;
            }
        }

        ~ISOExtractor()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
