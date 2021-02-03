using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XDCommon.Contracts;
using XDCommon.PokemonDefinitions;

namespace XDCommon.Utility
{
    public class ISO
    {
        public Region Region { get; internal set; }
        public Game Game { get; internal set; }
        public DOL DOL { get; internal set;  }
        public FST TOC { get; internal set;  }
        public Dictionary<string, FSys> Files { get; } = new Dictionary<string, FSys>();


        ConcurrentQueue<Func<Task>> extractTasks = new ConcurrentQueue<Func<Task>>();
        Task[] executors;
        CancellationTokenSource tokenSource;

        private int tasksCompleted;
        private int taskTotal;

        public ISO()
        {
            executors = new Task[Configuration.ThreadCount];
            tokenSource = new CancellationTokenSource();

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
                            tasksCompleted++;
                        }
                        else
                        {
                            await Task.Delay(TimeSpan.FromSeconds(10), tokenSource.Token);
                        }
                    }
                }, tokenSource.Token);

                executors[i].Start();
            }
        }

        public int ExtractionProgress => tasksCompleted;
        public int ExtractionTotal => taskTotal;

        public void ExtractFiles(params string[] fileNames)
        {

            foreach (var fileName in fileNames)
            {
                if (Files.ContainsKey(fileName))
                {
                    extractTasks.Enqueue(() => new Task(() => FSysExtractor.ExtractFSys(Files[fileName], true)));
                }
            }

            if (tasksCompleted < taskTotal)
            {
                taskTotal += extractTasks.Count;
            }
            else
            {
                tasksCompleted = 0;
                taskTotal = extractTasks.Count;
            }
        }

        public StringTable DolStringTable()
        {
            var strTable = new MemoryStream();
            int startOffset = 0;
            int size = 0;
            if (Game == Game.XD)
            {
                switch (Region)
                {
                    case Region.US:
                        startOffset = 0x374FC0;
                        size = 0x178BC;
                        break;
                    case Region.Europe:
                        startOffset = 0x38B0B0;
                        size = 0x11D10;
                        break;
                    case Region.Japan:
                        startOffset = 0x372AD8;
                        size = 0x17938;
                        break;
                }
            }
            else
            {
                switch (Region)
                {
                    case Region.US:
                        startOffset = 0x2cc810;
                        size = 0x124e0;
                        break;
                    case Region.Europe:
                        startOffset = 0x2c1b20;
                        size = 0x124e0;
                        break;
                    case Region.Japan:
                        startOffset = 0x2bece0;
                        size = 0xd850;
                        break;
                }
            }
            DOL.ExtractedFile.CopySubStream(strTable, startOffset, size);
            return new StringTable(strTable);
        }
	

        public StringTable DolStringTable2()
        {
            if (Game == Game.XD)
            {
                var strTable = new MemoryStream();
                int startOffset = 0;
                int size = 0;
                switch (Region)
                {
                    case Region.US:
                        startOffset = 0x38c87c;
                        size = 0x364;
                        break;
                    case Region.Europe:
                        startOffset = 0x3EA5A4;
                        size = 0x364;
                        break;
                    case Region.Japan:
                        startOffset = 0x39CDC0;
                        size = 0x2A0;
                        break;
                }
                DOL.ExtractedFile.CopySubStream(strTable, startOffset, size);
                return new StringTable(strTable);
            }
            else
            {
                return DolStringTable();
            }
        }

	    public StringTable CommonRelStringTable()
        {
            var strTable = new MemoryStream();
            uint startOffset = 0;
            int size = 0;
            var extractedFile = CommonRel();
            if (Game == Game.XD)
            {
                startOffset = extractedFile.GetPointer(Constants.stringTable1) + 0x68;
                switch (Region)
                {
                    case Region.US:
                    case Region.Europe:
                        size = 0xDC70;
                        break;
                    case Region.Japan:
                        size = 0xAC8C;
                        break;
                }
            }
            else
            {
                switch (Region)
                {
                    case Region.US:
                        startOffset = 0x59890;
                        size = 0xC770;
                        break;
                    case Region.Europe:
                        startOffset = 0x5A448;
                        size = 0xC544;
                        break;
                    case Region.Japan:
                        startOffset = 0x4580;
                        size = 0x9cf8;
                        break;
                }
            }

            extractedFile.ExtractedFile.CopySubStream(strTable, startOffset, size);
            return new StringTable(strTable);
	    }

        public REL CommonRel()
        {
            var fSys = Files["common.fsys"];
            if (!fSys.ExtractedEntries.ContainsKey("common_rel.rel"))
            {
                FSysExtractor.ExtractFSys(fSys, true);
            }
            return fSys.ExtractedEntries["common_rel.rel"] as REL;
        }

        ~ISO()
        {
            tokenSource.Cancel();
            Task.WaitAll(executors);
        }
    }
}
