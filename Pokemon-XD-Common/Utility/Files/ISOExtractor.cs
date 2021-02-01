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
    public class ISOExtractor
    {
        const int kISOFirstFileOffsetLocation = 0x434; // The "user data" start offset. Basically all the game specific files
        const int kISOFilesTotalSizeLocation = 0x438; // The size of the game specific files, so everything after dol and toc

        public Stream ISOStream { get; }
        public string ExtractPath { get; private set; }
        internal FST TOC { get; private set; }

        public ISOExtractor(string pathToISO)
        {
            ExtractPath = Configuration.ExtractDirectory;
            ISOStream = File.Open(pathToISO, FileMode.Open, FileAccess.ReadWrite);
        }

        public ISO ExtractISO()
        {
            var iso = new ISO();

            ISOStream.Seek(0, SeekOrigin.Begin);
            var _ = ISOStream.ReadByte();
            int gamecode2 = ISOStream.ReadByte() << 8;
            int gamecode1 = ISOStream.ReadByte();
            int game = gamecode2 | gamecode1;
            var region = ISOStream.ReadByte();

            switch (game)
            {
                case (ushort)Game.Colosseum:
                case (ushort)Game.XD:
                    iso.Game = (Game)game;
                    break;
                default:
                    throw new Exception("Unsupported game!");
            }

            switch (region)
            {
                case (byte)Region.US:
                case (byte)Region.Europe:
                case (byte)Region.Japan:
                    iso.Region = (Region)region;
                    break;
                default:
                    throw new Exception("Unknown region!");
            }

            ExtractPath = $"{Configuration.ExtractDirectory}/{iso.Game}-{iso.Region}";
            if (!Directory.Exists(ExtractPath) && !Configuration.UseMemoryStreams)
            {
                Directory.CreateDirectory(ExtractPath);
            }

            iso.DOL = new DOL(ExtractPath, this);
            TOC = new FST(ExtractPath, this);
            TOC.Load(iso.DOL);
            iso.TOC = TOC;

            foreach (string fileName in iso.TOC.AllFileNames)
            {
                if (fileName == "Start.dol" || fileName == "Game.toc")
                    continue;

                if (fileName.Contains("fsys", StringComparison.InvariantCultureIgnoreCase))
                {
                    var fsys = new FSys(fileName, this);
                    iso.Files.Add(fsys.Filename, fsys);
                }
            }

            return iso;
        }
        public void RepackISO(ISO iso, string savePath)
        {
            if (!savePath.EndsWith(".iso"))
            {
                savePath = $"{savePath}.iso";
            }
            using var isoStream = File.Open(savePath, FileMode.OpenOrCreate, FileAccess.Write);
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
            //isoStream.Seek(TOC.Offset, SeekOrigin.Begin);
            //iso.TOC.ExtractedFile.CopyTo(isoStream);

            // pack fsys files
            foreach (var fsys in iso.Files.Values)
            {
                fsys.WriteToStream(isoStream);
            }

            // pack footer


            isoStream.Flush();
        }

        ~ISOExtractor()
        {
            ISOStream.Dispose();
        }
    }
}
