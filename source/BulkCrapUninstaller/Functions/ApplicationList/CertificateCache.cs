/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Klocman.Tools;

namespace BulkCrapUninstaller.Functions.ApplicationList
{
    internal class CertificateCache
    {
        public string CacheFilename { get; set; }
        private Dictionary<string, X509Certificate2> _dictionaryCahe;

        public CertificateCache(string cacheFilename)
        {
            CacheFilename = cacheFilename;
        }

        public void LoadCertificateCache()
        {
            _dictionaryCahe = new Dictionary<string, X509Certificate2>();

            if (!File.Exists(CacheFilename)) return;

            try
            {
                var l = SerializationTools.DeserializeDictionary<string, byte[]>(CacheFilename);

                _dictionaryCahe = l.ToDictionary(x => x.Key, x => x.Value != null ? new X509Certificate2(x.Value) : null);
            }
            catch (SystemException e)
            {
                Console.WriteLine(e);
                File.Delete(CacheFilename);
                _dictionaryCahe.Clear();
            }
        }

        public void SaveCertificateCache()
        {
            if (_dictionaryCahe == null)
            {
                File.Delete(CacheFilename);
                return;
            }

            try
            {
                SerializationTools.SerializeDictionary(_dictionaryCahe.ToDictionary(x => x.Key, x => x.Value?.RawData), CacheFilename);
            }
            catch (SystemException e)
            {
                File.Delete(CacheFilename);
                Console.WriteLine(e);
            }
        }

        public void Delete()
        {
            _dictionaryCahe = null;
            File.Delete(CacheFilename);
        }

        public void AddItem(string id, X509Certificate2 cert)
        {
            if (_dictionaryCahe != null && id != null)
                _dictionaryCahe[id] = cert;
        }

        public bool ContainsKey(string id)
        {
            return _dictionaryCahe != null && !string.IsNullOrEmpty(id) && _dictionaryCahe.ContainsKey(id);
        }

        public X509Certificate2 GetCachedItem(string id)
        {
            return _dictionaryCahe[id];
        }
    }
}