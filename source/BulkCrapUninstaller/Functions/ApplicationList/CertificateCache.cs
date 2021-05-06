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
        public byte[] CertData
        {
            get => Cert?.RawData;
            set => Cert = value == null ? null : new X509Certificate2(value);
        }
    }

    internal class CertificateCache
    {
        public string CacheFilename { get; set; }
        private IDictionary<string, CertCacheEntry> _dictionaryCache;

        public CertificateCache(string cacheFilename)
        {
            CacheFilename = cacheFilename;
        }

        public void LoadCertificateCache()
        {
            _dictionaryCache = new Dictionary<string, CertCacheEntry>();

            if (!File.Exists(CacheFilename)) return;

            try
            {
                _dictionaryCache = SerializationTools.DeserializeDictionary<string, CertCacheEntry>(CacheFilename);
            }
            catch (SystemException e)
            {
                Console.WriteLine(e);
                File.Delete(CacheFilename);
            }
        }

        public void SaveCertificateCache()
        {
            if (_dictionaryCache == null)
            {
                File.Delete(CacheFilename);
                return;
            }

            try
            {
                SerializationTools.SerializeDictionary(_dictionaryCache, CacheFilename);
            }
            catch (SystemException e)
            {
                File.Delete(CacheFilename);
                Console.WriteLine(e);
            }
        }

        public void ClearChache()
        {
            _dictionaryCache = null;
            File.Delete(CacheFilename);
        }

        public void AddItem(string id, X509Certificate2 cert, bool verified)
        {
            if (_dictionaryCache != null && id != null)
                _dictionaryCache[id] = new CertCacheEntry { Cert = cert, Valid = verified };
        }

        public bool ContainsKey(string id)
        {
            return _dictionaryCache != null && !string.IsNullOrEmpty(id) && _dictionaryCache.ContainsKey(id);
        }

        public CertCacheEntry GetCachedItem(string id)
        {
            return _dictionaryCache[id];
        }
    }
}