using System.Collections.Generic;
using System.Linq;
using Klocman.Tools;

namespace UninstallTools.Factory
{
    public class ApplicationUninstallerFactoryCache
    {
        private ApplicationUninstallerFactoryCache(string filename, Dictionary<string, ApplicationUninstallerEntry> cacheData)
        {
            Filename = filename;
            Cache = cacheData;
        }

        public ApplicationUninstallerFactoryCache(string filename) : this(filename, new Dictionary<string, ApplicationUninstallerEntry>())
        {
        }

        public Dictionary<string, ApplicationUninstallerEntry> Cache { get; }

        public string Filename { get; set; }

        public static ApplicationUninstallerFactoryCache Load(string filename)
        {
            var result = SerializationTools.DeserializeFromXml<List<ApplicationUninstallerEntry>>(filename);

            return new ApplicationUninstallerFactoryCache(filename, result.ToDictionary(x => x.RatingId, x => x));
        }

        public void Save()
        {
            SerializationTools.SerializeToXml(Filename, Cache.Select(x => x.Value).ToList());
        }
    }
}