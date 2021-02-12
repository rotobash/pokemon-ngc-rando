using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XDCommon.Contracts;

namespace XDCommon.Utility
{
    public class ISOExtractor : IDisposable
    {
        private bool disposedValue;

        public Stream ISOStream { get; }
        public string ExtractPath { get; private set; }

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
            iso.TOC.Load(iso.DOL);

            var commonFsys = iso.GetFSysFile("common.fsys");
            iso.CommonRel = FSysFileEntry.ExtractFromFSys(commonFsys, iso.Region == Region.Japan ? 1 : 0) as REL;
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
            // write disk size first so we can seek around the stream without hitting the end
            ISOStream.Seek(0, SeekOrigin.Begin);
            ISOStream.CopyTo(isoStream);
            isoStream.Flush();
            isoStream.Seek(0, SeekOrigin.Begin);

            // pack header

            //// pack dol
            //isoStream.Seek(iso.DOL.Offset, SeekOrigin.Begin);
            //iso.DOL.ExtractedFile.CopyTo(isoStream);

            //// pack FST
            isoStream.Seek(iso.TOC.Offset + 12 , SeekOrigin.Begin);
            using var tocstream = iso.TOC.Encode();
            tocstream.CopyTo(isoStream);

            // pack fsys files
            foreach (var fsys in iso.Files.Values)
            {
                using var fsysStream = fsys.Encode();
                var fsysStartOffset = iso.TOC.LocationForFile(fsys.FileName);
                isoStream.Seek(fsysStartOffset + fsys.Offset, SeekOrigin.Begin);
                fsysStream.CopyTo(isoStream);
            }

            // pack padding


            isoStream.Flush();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                ISOStream.Dispose();
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
