// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializableException.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace NBug.Core.Util.Serialization
{
    [Serializable]
    public class SerializableException
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SerializableException" /> class.
        ///     Default constructor provided for XML serialization and de-serialization.
        /// </summary>
        public SerializableException()
        {
        }

        public SerializableException(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException();
            }

            var oldCulture = Thread.CurrentThread.CurrentCulture;
            var oldUICulture = Thread.CurrentThread.CurrentUICulture;
            try
            {
                // Prefer messages in English, instead of in language of the user.
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                Type = exception.GetType().ToString();

                if (exception.Data.Count != 0)
                {
                    foreach (DictionaryEntry entry in exception.Data)
                    {
                        if (entry.Value != null)
                        {
                            // Assign 'Data' property only if there is at least one entry with non-null value
                            if (Data == null)
                            {
                                Data = new SerializableDictionary<object, object>();
                            }

                            Data.Add(entry.Key, entry.Value);
                        }
                    }
                }

                if (exception.HelpLink != null)
                {
                    HelpLink = exception.HelpLink;
                }

                if (exception.InnerException != null)
                {
                    InnerException = new SerializableException(exception.InnerException);
                }
                /*
				if (exception is AggregateException)
				{
					this.InnerExceptions = new List<SerializableException>();

					foreach (var innerException in ((AggregateException)exception).InnerExceptions)
					{
						this.InnerExceptions.Add(new SerializableException(innerException));
					}

					this.InnerExceptions.RemoveAt(0);
				}*/

                Message = exception.Message != string.Empty ? exception.Message : string.Empty;

                if (exception.Source != null)
                {
                    Source = exception.Source;
                }

                if (exception.StackTrace != null)
                {
                    StackTrace = exception.StackTrace;
                }

                if (exception.TargetSite != null)
                {
                    TargetSite = string.Format("{0} @ {1}", exception.TargetSite, exception.TargetSite.DeclaringType);
                }

                ExtendedInformation = GetExtendedInformation(exception);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = oldCulture;
                Thread.CurrentThread.CurrentUICulture = oldUICulture;
            }
        }

        public SerializableDictionary<object, object> Data { get; set; }
        public SerializableDictionary<string, object> ExtendedInformation { get; set; }
        public string HelpLink { get; set; }
        public SerializableException InnerException { get; set; }
        public List<SerializableException> InnerExceptions { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        // This will make TargetSite property XML serializable but RuntimeMethodInfo class does not have a parameterless
        // constructor thus the serializer throws an exception if full info is used
        public string TargetSite { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            var serializer = XmlSerializer.FromTypes(new[] {typeof (SerializableException)})[0];
            //var serializer = new XmlSerializer(typeof(SerializableException));
            using (var stream = new MemoryStream())
            {
                stream.SetLength(0);
                serializer.Serialize(stream, this);
                stream.Position = 0;
                var doc = XDocument.Load(XmlReader.Create(stream));
                return doc.Root.ToString();
            }
        }

        private SerializableDictionary<string, object> GetExtendedInformation(Exception exception)
        {
            var extendedProperties = (from property in exception.GetType().GetProperties()
                where
                    property.Name != "Data" && property.Name != "InnerExceptions" && property.Name != "InnerException"
                    && property.Name != "Message" && property.Name != "Source" && property.Name != "StackTrace"
                    && property.Name != "TargetSite" && property.Name != "HelpLink" && property.CanRead
                select property).ToArray();

            if (extendedProperties.Any())
            {
                var extendedInformation = new SerializableDictionary<string, object>();

                foreach (
                    var property in extendedProperties.Where(property => property.GetValue(exception, null) != null))
                {
                    extendedInformation.Add(property.Name, property.GetValue(exception, null));
                }

                return extendedInformation;
            }
            return null;
        }
    }
}