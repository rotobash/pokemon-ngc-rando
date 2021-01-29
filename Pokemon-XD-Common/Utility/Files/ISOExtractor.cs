using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        public TOC TOC { get; }
        DOL dol;

        Dictionary<string, FSys> extractedFiles = new Dictionary<string, FSys>();

        ConcurrentQueue<Func<Task>> extractTasks = new ConcurrentQueue<Func<Task>>();
        Task[] executors;
        CancellationTokenSource tokenSource;

        public Region Region { get; }
        public Game Game { get; }

        public ISOExtractor(string extractPathDirectory, string pathToISO, bool verbose = false, int threadCount = 4)
        {
            ExtractPath = extractPathDirectory;
            ISOStream = File.Open(pathToISO, FileMode.Open, FileAccess.ReadWrite);
            executors = new Task[threadCount];
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
            TOC = new TOC(ExtractPath, this);
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
                    extractTasks.Enqueue(() =>
                        new Task(() =>
                        {
                            var fsys = new FSys(fileName, this);
                            FSysExtractor.ExtractFSys(fsys, true);
                            extractedFiles.Add(fileName, fsys);
                        })
                    );
                }
            }

            for (int i = 0; i < executors.Length; i++)
            {
                executors[i] = new Task(async () =>
                {
                    while (!tokenSource.Token.IsCancellationRequested)
                    {
                        extractTasks.TryDequeue(out var extractTask);
                        var t = extractTask();
                        t.Start();
                        await t;
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
