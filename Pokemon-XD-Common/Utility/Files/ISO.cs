using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XDCommon.Contracts;

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
                            await Task.Delay(TimeSpan.FromSeconds(10));
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

        ~ISO()
        {
            tokenSource.Cancel();
            Task.WaitAll(executors);
        }
    }
}
