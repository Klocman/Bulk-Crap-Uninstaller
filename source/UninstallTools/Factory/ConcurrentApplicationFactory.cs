/*
    Copyright (c) 2018 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace UninstallTools.Factory
{
    internal sealed class ConcurrentApplicationFactory : IDisposable
    {
        private ListGenerationProgress.ListGenerationCallback _callback;

        private bool _cancelled;
        private ListGenerationProgress _parentProgress;

        private readonly Thread _thread;
        private ListGenerationProgress _threadLastReport;

        private List<ApplicationUninstallerEntry> _threadResults;

        public ConcurrentApplicationFactory(
            Func<ListGenerationProgress.ListGenerationCallback, List<ApplicationUninstallerEntry>> factoryMethod)
        {
            _thread = new Thread(() =>
            {
                try
                {
                    _threadResults = factoryMethod(ProgressCallback);
                }
                catch (OperationCanceledException)
                {
                    _cancelled = true;
                }
            });
            _thread.IsBackground = false;
            _thread.Name = "ConcurrentGetUninstallerEntries";
            _thread.Priority = ThreadPriority.Normal;
        }

        public void Dispose()
        {
            _cancelled = true;
        }

        private void ProgressCallback(ListGenerationProgress report)
        {
            if (_cancelled) throw new OperationCanceledException();

            lock (_thread)
            {
                if (_parentProgress == null)
                {
                    _threadLastReport = report;
                }
                else
                {
                    _parentProgress.Inner = report;
                    _callback.Invoke(_parentProgress);
                }
            }
        }

        public void Start()
        {
            _thread.Start();
        }

        public List<ApplicationUninstallerEntry> GetResults(ListGenerationProgress.ListGenerationCallback callback,
            ListGenerationProgress parentProgress)
        {
            if (parentProgress == null) throw new ArgumentNullException(nameof(parentProgress));
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            Debug.Assert(_thread != null);

            if (_thread.IsAlive)
            {
                lock (_thread)
                {
                    _parentProgress = parentProgress;

                    if (_threadLastReport != null)
                    {
                        _parentProgress.Inner = _threadLastReport;
                        callback.Invoke(_parentProgress);
                    }

                    _callback = callback;
                }

                _thread.Join();
            }

            if (_cancelled)
                throw new OperationCanceledException();

            return _threadResults ?? new List<ApplicationUninstallerEntry>();
        }
    }
}