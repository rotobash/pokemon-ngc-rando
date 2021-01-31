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

        public string ExtractPath { get; }
        public Stream ISOStream { get; }
        public FST TOC { get; }
        DOL dol;

        List<FSys> fSysFiles = new List<FSys>();

        ConcurrentQueue<Func<Task>> extractTasks = new ConcurrentQueue<Func<Task>>();
        Task[] executors;
        CancellationTokenSource tokenSource;

        public Region Region { get; }
        public Game Game { get; }

        public ISOExtractor(string pathToISO, bool verbose = false)
        {
            ISOStream = File.Open(pathToISO, FileMode.Open, FileAccess.ReadWrite);
            ExtractPath = Configuration.ExtractDirectory;
            
            executors = new Task[Configuration.ThreadCount];
            tokenSource = new CancellationTokenSource();

            var _ = ISOStream.ReadByte();
            int gamecode2 = ISOStream.ReadByte() << 8;
            int gamecode1 = ISOStream.ReadByte();
            int game = gamecode2 | gamecode1;
            var region = ISOStream.ReadByte();

            switch (game)
            {
                case (ushort)Game.Colosseum:
                case (ushort)Game.XD:
                    Game = (Game)game;
                    break;
                default:
                    throw new Exception("Unsupported game!");
            }

            switch (region)
            {
                case (byte)Region.US:
                case (byte)Region.Europe:
                case (byte)Region.Japan:
                    Region = (Region)region;
                    break;
                default:
                    throw new Exception("Unknown region!");
            }

            dol = new DOL(ExtractPath, this);
            TOC = new FST(ExtractPath, this);
            TOC.Load(dol, verbose);

            //ExtractFiles("B1_1.fsys");
            ExtractFiles(TOC.AllFileNames.ToArray());
            while (!extractTasks.IsEmpty)
            {
                Thread.Sleep(1000);
            }

        }

        public void ExtractFiles(params string[] fileNames)
        {
            foreach (string fileName in fileNames)
            {
                if (fileName == "Start.dol" || fileName == "Game.toc")
                    continue;

                if (fileName.Contains("fsys", StringComparison.InvariantCultureIgnoreCase))
                {
                    var fsys = new FSys(fileName, this);
                    fSysFiles.Add(fsys);
                }
            }

            foreach (var fsysFile in fSysFiles)
            {
                extractTasks.Enqueue(() => new Task(() => FSysExtractor.ExtractFSys(fsysFile, true)));
            }

            for (int i = 0; i < executors.Length; i++)
            {
                executors[i] = new Task(async () =>
                {
                    while (!tokenSource.Token.IsCancellationRequested)
                    {
                        if (extractTasks.TryDequeue(out var extractTask))
                        {
                            var t = extractTask();
                            t.Start();
                            await t;
                        }
                        else
                        {
                            await Task.Delay(TimeSpan.FromSeconds(10));
                        }
                    }
                }, tokenSource.Token);

                executors[i].Start();
            }
        }

        ~ISOExtractor()
        {
            ISOStream.Flush();
            ISOStream.Dispose();

            tokenSource.Cancel();
            Task.WaitAll(executors);
        }
    }
}
