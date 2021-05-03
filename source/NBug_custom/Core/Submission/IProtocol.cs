// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProtocol.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using NBug.Core.Reporting.Info;
using NBug.Core.Util.Serialization;

namespace NBug.Core.Submission
{
    /// <summary>
    ///     Implement this to support sending data to a specific location.
    ///     We recommend you base your implementation on ProtocolBase.
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        ///     Connection string suitable for serialization.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        ///     Send the report file to the destination.
        /// </summary>
        /// <param name="fileName">Name of the file (e.g. "Exception_12345.zip")</param>
        /// <param name="file">File stream</param>
        /// <param name="report">Report</param>
        /// <param name="exception"></param>
        /// <returns>True if report was sent successfully.</returns>
        bool Send(string fileName, Stream file, Report report, SerializableException exception);
    }
}