/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using Klocman.Tools;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    public sealed class CertCacheEntry
    {
        public bool Valid { get; set; }

        [XmlIgnore]
        public X509Certificate2 Cert { get; set; }
        public byte[] CertData // ReadOnlySpan<byte> crashes XML serializer in new versions of .NET
        {
            get => Cert?.RawData;
            set => Cert = value == null ? null : new X509Certificate2(value);
        }
    }

    internal class CertificateCache
    {
        public string CacheFilename { get; }
        private IDictionary<string, CertCacheEntry> _dictionaryCache;
        private readonly object _cacheLock = new object();

        public CertificateCache(string cacheFilename)
        {
            CacheFilename = cacheFilename ?? throw new ArgumentNullException(nameof(cacheFilename));
        }

        public void LoadCertificateCache()
        {
            lock (_cacheLock)
            {
                _dictionaryCache = null;
                try
                {
                    if (File.Exists(CacheFilename))
                        _dictionaryCache = SerializationTools.DeserializeDictionary<string, CertCacheEntry>(CacheFilename);
                }
                catch (SystemException e)
                {
                    Console.WriteLine(e);
                    ClearChache();
                }
            }
        }

        public void SaveCertificateCache()
        {
            lock (_cacheLock)
            {
                if (_dictionaryCache == null || _dictionaryCache.Count == 0)
                {
                    ClearChache();
                    return;
                }

                try
                {
                    SerializationTools.SerializeDictionary(_dictionaryCache, CacheFilename);
                }
                catch (SystemException e)
                {
                    Console.WriteLine(e);
                    ClearChache();
                }
            }
        }

        public void ClearChache()
        {
            lock (_cacheLock)
            {
                _dictionaryCache = null;
                try
                {
                    File.Delete(CacheFilename);
                }
                catch
                {
                    System.Threading.Thread.Sleep(50);
                    File.Delete(CacheFilename);
                }
            }
        }

        public void AddItem(string id, X509Certificate2 cert, bool verified)
        {
            lock (_cacheLock)
            {
                if (_dictionaryCache == null) _dictionaryCache = new Dictionary<string, CertCacheEntry>();

                if (id != null) _dictionaryCache[id] = new CertCacheEntry { Cert = cert, Valid = verified };
            }
        }

        public bool ContainsKey(string id)
        {
            lock (_cacheLock)
                return _dictionaryCache != null && !string.IsNullOrEmpty(id) && _dictionaryCache.ContainsKey(id);
        }

        public CertCacheEntry GetCachedItem(string id)
        {
            lock (_cacheLock)
            {
                // Keep retrieval safe if cache was cleared between ContainsKey and GetCachedItem calls.
                if (_dictionaryCache == null || string.IsNullOrEmpty(id))
                    return null;

                return _dictionaryCache.TryGetValue(id, out var entry) ? entry : null;
            }
        }
    }
}