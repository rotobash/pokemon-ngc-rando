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
    public class ISO : BaseExtractedFile
    {
        public override FileTypes FileType { get => FileTypes.ISO; }

        public Region Region { get; internal set; }
        public Game Game { get; internal set; }
        public DOL DOL { get; set; }
        public FST TOC { get; internal set; }
        public REL CommonRel { get; internal set; }
        public StringTable DolStringTable { get; internal set; }
        public StringTable DolStringTable2 { get; internal set; }
        public StringTable CommonRelStringTable { get; internal set; }
        public Dictionary<string, FSys> Files { get; }


        ConcurrentQueue<Func<Task>> extractTasks = new ConcurrentQueue<Func<Task>>();
        Task[] executors;
        CancellationTokenSource tokenSource;

        private int tasksCompleted;
        private int taskTotal;

        public ISO(Stream baseStream, string extractPath)
        {
            executors = new Task[Configuration.ThreadCount];
            tokenSource = new CancellationTokenSource();
            Files = new Dictionary<string, FSys>();
            ExtractedFile = baseStream;
            Path = extractPath;

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

        public FSys GetFSysFile(string fileName)
        {
            if (fileName.Contains("fsys", StringComparison.InvariantCultureIgnoreCase))
            {
                FSTFileEntry fileEntry;
                if (Files.ContainsKey(fileName))
                {
                    return Files[fileName];
                }
                else if ((fileEntry = TOC.GetFileEntry(fileName)) != null)
                {
                    var fsys = new FSys(fileEntry, this);
                    Files.Add(fsys.FileName, fsys);
                    return fsys;
                }
            }
            return null;
        }

        public void ExtractFiles(params string[] fileNames)
        {
            
            foreach (var fileName in fileNames)
            {
                var fsys = GetFSysFile(fileName);
                if (fsys != null)
                {
                    extractTasks.Enqueue(() => new Task(() => FSysExtractor.ExtractFSys(fsys, true)));
                    taskTotal++;
                }
            }
        }

        public void BuildStringTables()
        {
            var dolStrTable = new MemoryStream();
            var dolStrTable2 = new MemoryStream();
            var commonRelTable = new MemoryStream();
            int dolStartOffset, dol2StartOffset, commonRelStartOffset;
            int dolSize, dol2Size, commonRelSize;
            if (Game == Game.XD)
            {
                commonRelStartOffset = (int)(CommonRel.GetPointer(Constants.stringTable1) + 0x68);
                switch (Region)
                {
                    default:
                    case Region.US:
                        dolStartOffset = 0x374FC0;
                        dolSize = 0x178BC;
                        dol2StartOffset = 0x38c87c;
                        dol2Size = 0x364;
                        commonRelSize = 0xDC70;
                        break;
                    case Region.Europe:
                        dolStartOffset = 0x38B0B0;
                        dolSize = 0x11D10;
                        dol2StartOffset = 0x3EA5A4;
                        dol2Size = 0x364;
                        commonRelSize = 0xDC70;
                        break;
                    case Region.Japan:
                        dolStartOffset = 0x372AD8;
                        dolSize = 0x17938;
                        dol2StartOffset = 0x39CDC0;
                        dol2Size = 0x2A0;
                        commonRelSize = 0xAC8C;
                        break;
                }
            }
            else
            {
                switch (Region)
                {
                    default:
                    case Region.US:
                        dolStartOffset = 0x2cc810;
                        dolSize = 0x124e0;
                        commonRelStartOffset = 0x59890;
                        commonRelSize = 0xC770;
                        break;
                    case Region.Europe:
                        dolStartOffset = 0x2c1b20;
                        dolSize = 0x124e0;
                        commonRelStartOffset = 0x5A448;
                        commonRelSize = 0xC544;
                        break;
                    case Region.Japan:
                        dolStartOffset = 0x2bece0;
                        dolSize = 0xd850;
                        commonRelStartOffset = 0x4580;
                        commonRelSize = 0x9cf8;
                        break;
                }
                dol2StartOffset = dolStartOffset;
                dol2Size = dolSize;
            }

            DOL.ExtractedFile.CopySubStream(dolStrTable, dolStartOffset, dolSize);
            DolStringTable = new StringTable(dolStrTable);

            DOL.ExtractedFile.CopySubStream(dolStrTable2, dol2StartOffset, dol2Size);
            DolStringTable2 = new StringTable(dolStrTable2);

            CommonRel.ExtractedFile.CopySubStream(commonRelTable, commonRelStartOffset, commonRelSize);
            CommonRelStringTable = new StringTable(commonRelTable);
        }

        public override Stream Encode(bool _)
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (!disposedValue)
            {
                TOC.Dispose();
                TOC = null;
                DOL.Dispose();
                DOL = null;
                
                CommonRel.Dispose();
                CommonRel = null;
                CommonRelStringTable.Dispose();
                CommonRelStringTable = null;

                DolStringTable.Dispose();
                DolStringTable = null;
                DolStringTable2.Dispose();
                DolStringTable2 = null;

                foreach (var fsys in Files.Values)
                {
                    fsys.Dispose();
                }
                Files.Clear();

                base.Dispose(isDisposing);
            }
        }
    }
}
