// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomFactory.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using NBug.Core.Reporting.Info;
using NBug.Core.Util.Serialization;
using NBug.Events;

namespace NBug.Core.Submission.Custom
{
    public class CustomFactory : IProtocolFactory
    {
        public string SupportedType
        {
            get { return "Custom"; }
        }

        public IProtocol FromConnectionString(string connectionString)
        {
            return new Custom(connectionString);
        }
    }

    public class Custom : ProtocolBase
    {
        public Custom(string connectionString)
            : base(connectionString)
        {
        }

        public Custom()
        {
        }

        public override bool Send(string fileName, Stream file, Report report, SerializableException exception)
        {
            if (Settings.CustomSubmissionHandle == null)
                return false;

            var e = new CustomSubmissionEventArgs(fileName, file, report, exception);
            Settings.CustomSubmissionHandle.DynamicInvoke(this, e);
            return e.Result;
        }
    }
}