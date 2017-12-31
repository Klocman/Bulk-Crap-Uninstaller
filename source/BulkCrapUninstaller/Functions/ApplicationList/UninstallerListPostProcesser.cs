/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using BulkCrapUninstaller.Properties;
using Klocman.Binding.Settings;
using Klocman.Collections;
using Klocman.Events;
using Klocman.Tools;
using UninstallTools;

namespace BulkCrapUninstaller.Functions.ApplicationList
{

    [Serializable]
    public class CertSerializeData
    {
        public CertSerializeData()
        {
        }

        public CertSerializeData(string appId, byte[] certData)
        {
            AppId = appId;
            CertData = certData;
        }

        public byte[] CertData { get; set; }
        public string AppId { get; set; }
    }
    internal class UninstallerListPostProcesser : IDisposable
    {
        readonly SettingBinder<Settings> _settings = Settings.Default.SettingBinder;
        private bool _abortPostprocessingThread;
        private Thread _finalizerThread;
        private readonly List<object> _objectsToUpdate = new List<object>();
        private readonly Action<IList> _updateItemsCallback;

        /// <summary>
        /// External lock to the uninstall system.
        /// </summary>
        internal object UninstallerFileLock { get; set; } = new object();

        public event EventHandler<CountingUpdateEventArgs> UninstallerPostprocessingProgressUpdate;

        public UninstallerListPostProcesser(Action<IList> updateItemsCallback)
        {
            if (updateItemsCallback == null) throw new ArgumentNullException(nameof(updateItemsCallback));
            _updateItemsCallback = updateItemsCallback;
        }

        public void AbortPostprocessingThread()
        {
            _abortPostprocessingThread = true;
        }

        Dictionary<string, X509Certificate2> _dictionaryCahe;

        public void LoadCertificateCache(string filename)
        {
            // todo config option
            _dictionaryCahe = new Dictionary<string, X509Certificate2>();

            if (!File.Exists(filename)) return;

            try
            {
                var l = SerializationTools.DeserializeFromXml<List<CertSerializeData>>(filename);

                _dictionaryCahe = l.ToDictionary(x => x.AppId, x => x.CertData != null ? new X509Certificate2(x.CertData) : null);
            }
            catch (SystemException e)
            {
                Console.WriteLine(e);
                File.Delete(filename);
                _dictionaryCahe.Clear();
            }
        }

        public void SaveCertificateCache(string filename)
        {
            if (_dictionaryCahe == null)
            {
                File.Delete(filename);
                return;
            }

            try
            {
                SerializationTools.SerializeToXml(filename, _dictionaryCahe.Select(x => new CertSerializeData(x.Key, x.Value?.RawData)).ToList());
            }
            catch (SystemException e)
            {
                File.Delete(filename);
                Console.WriteLine(e);
            }
        }

        public void StartProcessingThread(IEnumerable<ApplicationUninstallerEntry> itemsToProcess)
        {
            StopProcessingThread(true);

            _finalizerThread = new Thread(UninstallerPostprocessingThread)
            { Name = "UninstallerPostprocessingThread", IsBackground = true, Priority = ThreadPriority.Lowest };

            _abortPostprocessingThread = false;
            _finalizerThread.Start(itemsToProcess);
        }

        public void StopProcessingThread(bool block)
        {
            if (_finalizerThread == null || !_finalizerThread.IsAlive) return;

            _abortPostprocessingThread = true;

            if (!block) return;

            do
            {
                Thread.Sleep(100);
                // Process events in case we are blocking ui thread and the worker thread is trying to invoke.
                // TODO Reimplement the whole thing to avoid having to do this
                Application.DoEvents();
            } while (_finalizerThread.IsAlive);
        }

        private void UninstallerPostprocessingThread(object targets)
        {
            var items = targets as IEnumerable<ApplicationUninstallerEntry>;
            if (items == null)
                return;

            var targetList = items as IList<ApplicationUninstallerEntry> ?? items.ToList();
            var currentCount = 1;
            foreach (var uninstaller in targetList)
            {
                if (_abortPostprocessingThread)
                {
                    OnUninstallerPostprocessingProgressUpdate(new CountingUpdateEventArgs(0, 0, 0));
                    return;
                }

                var sendTag = true;
                if (_settings.Settings.AdvancedTestCertificates)
                {
                    lock (UninstallerFileLock)
                    {
                        var cert = GetCert(uninstaller);
                        sendTag = cert != null;
                    }
                }

                var countingUpdateEventArgs = new CountingUpdateEventArgs(0, targetList.Count, currentCount);
                if (sendTag) countingUpdateEventArgs.Tag = uninstaller;

                OnUninstallerPostprocessingProgressUpdate(countingUpdateEventArgs);

                currentCount++;
            }
        }

        private X509Certificate2 GetCert(ApplicationUninstallerEntry uninstaller)
        {
            var id = uninstaller.RatingId;

            if (_dictionaryCahe != null && id != null && _dictionaryCahe.ContainsKey(id))
            {
                var cert = _dictionaryCahe[id];
                uninstaller.SetCertificate(cert);
                return cert;
            }
            else
            {
                var cert = uninstaller.GetCertificate();
                if (_dictionaryCahe != null && id != null)
                    _dictionaryCahe.Add(id, cert);
                return cert;
            }
        }

        public void Dispose()
        {
            StopProcessingThread(false);
        }

        protected virtual void OnUninstallerPostprocessingProgressUpdate(CountingUpdateEventArgs y)
        {
            lock (_objectsToUpdate)
            {
                if (y.Tag != null)
                    _objectsToUpdate.Add(y.Tag);

                if (y.Value == y.Maximum || y.Value % 35 == 0)
                {
                    try
                    {
                        _updateItemsCallback(_objectsToUpdate);
                    }
                    catch (InvalidOperationException ex)
                    {
                        // The list view got disposed before we could update it.
                        AbortPostprocessingThread();
                        Debug.Fail(ex.Message, ex.StackTrace);
                    }
                    _objectsToUpdate.Clear();
                }
            }

            UninstallerPostprocessingProgressUpdate?.Invoke(this, y);
        }
    }
}