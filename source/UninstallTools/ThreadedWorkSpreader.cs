/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace UninstallTools
{
    internal class ThreadedWorkSpreader<TData, TState> where TState : class
    {
        private readonly List<WorkerData> _workers = new List<WorkerData>();

        public ThreadedWorkSpreader(int maxThreadsPerBucket, Action<TData, TState> workLogic,
            Func<List<TData>, TState> stateGenerator, Func<TData, string> dataNameGetter)
        {
            if (maxThreadsPerBucket <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxThreadsPerBucket), maxThreadsPerBucket, @"Minimum value is 1");
            MaxThreadsPerBucket = maxThreadsPerBucket;
            StateGenerator = stateGenerator ?? throw new ArgumentNullException(nameof(stateGenerator));
            WorkLogic = workLogic ?? throw new ArgumentNullException(nameof(workLogic));
            DataNameGetter = dataNameGetter ?? throw new ArgumentNullException(nameof(dataNameGetter));
        }

        public Func<List<TData>, TState> StateGenerator { get; }
        public Func<TData, string> DataNameGetter { get; }
        public Action<TData, TState> WorkLogic { get; }

        public int MaxThreadsPerBucket { get; }
        
        public void Start(List<List<TData>> dataBuckets, ListGenerationProgress.ListGenerationCallback progressCallback)
        {
            if (dataBuckets == null) throw new ArgumentNullException(nameof(dataBuckets));
            if (progressCallback == null) throw new ArgumentNullException(nameof(progressCallback));

            var totalCount = dataBuckets.Aggregate(0, (i, list) => i + list.Count);

            var progress = 0;

            void OnItemDone(string itemName)
            {
                progressCallback(new ListGenerationProgress(progress++, totalCount, itemName));
            }

            foreach (var itemBucket in dataBuckets)
            {
                if (itemBucket.Count == 0) continue;

                var threadCount = Math.Min(MaxThreadsPerBucket, itemBucket.Count / 10 + 1);

                var threadWorkItemCount = itemBucket.Count / threadCount + 1;

                for (var i = 0; i < threadCount; i++)
                {
                    var firstUnique = i * threadWorkItemCount;
                    var workerItems = itemBucket.Skip(firstUnique).Take(threadWorkItemCount).ToList();

                    var worker = new Thread(WorkerThread)
                    {
                        Name = nameof(ThreadedWorkSpreader<TData, TState>) + "_worker",
                        IsBackground = false
                    };
                    var workerData = new WorkerData(workerItems, worker, OnItemDone, StateGenerator(itemBucket));
                    _workers.Add(workerData);
                    worker.Start(workerData);
                }
            }
        }

        public IEnumerable<TState> Join()
        {
            foreach (var workerData in _workers)
                try
                {
                    workerData.Worker.Join();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            return _workers.Select(x => x.State);
        }

        private void WorkerThread(object obj)
        {
            var workerInterface = obj as WorkerData;
            if (workerInterface == null) throw new ArgumentNullException(nameof(workerInterface));

            foreach (var data in workerInterface.Input)
            {
                try
                {
                    workerInterface.OnInputItemDone(DataNameGetter?.Invoke(data) ?? data?.ToString());
                }
                catch (OperationCanceledException)
                {
                    return;
                }

                WorkLogic.Invoke(data, workerInterface.State);
            }
        }

        private sealed class WorkerData
        {
            public WorkerData(List<TData> input, Thread worker, Action<string> onInputItemDone, TState state)
            {
                Input = input;
                Worker = worker;
                OnInputItemDone = onInputItemDone;
                State = state;
            }

            public List<TData> Input { get; }
            public Thread Worker { get; }
            public Action<string> OnInputItemDone { get; }
            public TState State { get; }
        }
    }
}