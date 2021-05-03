// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Feedback.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using NBug.Core.Reporting.Info;
using NBug.Core.Util.Logging;

namespace NBug.Core.Reporting
{
    internal class Feedback
    {
        private Report report;

        internal Feedback()
        {
            try
            {
                report = new Report(null);
            }
            catch (Exception exception)
            {
                Logger.Error(
                    "An exception occurred while sending a user feedback. See the inner exception for details.",
                    exception);
            }
        }
    }
}