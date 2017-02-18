// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Report.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using NBug.Core.Util.Serialization;

namespace NBug.Core.Reporting.Info
{
    [Serializable]
    public class Report
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Report" /> class to be filled with information later on.
        /// </summary>
        public Report()
        {
        }

        internal Report(SerializableException serializableException)
        {
            GeneralInfo = new GeneralInfo(serializableException);
        }

        /// <summary>
        ///     Gets or sets a custom object property to store user supplied information in the bug report. You need to handle
        ///     <see cref="NBug.Settings.ProcessingException" /> event to fill this property with required information.
        /// </summary>
        public object CustomInfo { get; set; }

        /// <summary>
        ///     Gets or sets the general information about the exception and the system to be presented in the bug report.
        /// </summary>
        public GeneralInfo GeneralInfo { get; set; }

        /*/// <summary>
		/// Gets or sets a custom object property to store user supplied information in the bug report. You need to handle
		/// <see cref="NBug.Settings.ProcessingException"/> event to fill this property with required information.
		/// </summary>
		public object StaticInfo { get; set; }*/

        public override string ToString()
        {
            var serializer = CustomInfo != null
                ? new XmlSerializer(typeof (Report), new[] {CustomInfo.GetType()})
                : new XmlSerializer(typeof (Report));
            using (var stream = new MemoryStream())
            {
                stream.SetLength(0);
                serializer.Serialize(stream, this);
                stream.Position = 0;
                var doc = XDocument.Load(XmlReader.Create(stream));
                return doc.Root.ToString();
            }
        }
    }
}