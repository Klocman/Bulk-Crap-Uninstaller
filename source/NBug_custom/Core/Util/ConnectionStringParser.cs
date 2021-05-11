// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStringParser.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NBug.Core.Util.Exceptions;

namespace NBug.Core.Util
{
    public static class ConnectionStringParser
    {
        // Currently ; and = characters are illegal and needs preparsing and escaping
        public static Dictionary<string, string> Parse(string connectionString)
        {
            try
            {
                // Pre-processing the connection string, detect escape sequence, trim trailing semicolon
                var data = new Dictionary<string, string>();
                var fields = Regex.Split(connectionString.TrimEnd(';'), @"(?<!\\);|(?<!\\)=");

                // Replace the escape sequence and build the dictionary
                for (var i = 0; i < fields.Length; i++)
                {
                    data.Add(fields[i].Replace(@"\;", ";").Replace(@"\=", "="),
                        fields[++i].Replace(@"\;", ";").Replace(@"\=", "="));
                }

                return data;
            }
            catch (Exception exception)
            {
                throw new NBugConfigurationException(
                    "Cannot parse the connection string supplied. The connection string may be malformed: " +
                    connectionString, exception);
            }
        }
    }
}