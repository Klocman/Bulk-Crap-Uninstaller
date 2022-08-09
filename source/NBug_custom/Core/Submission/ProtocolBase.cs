// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtocolBase.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NBug.Core.Reporting.Info;
using NBug.Core.Util;
using NBug.Core.Util.Serialization;

namespace NBug.Core.Submission
{
    public abstract class ProtocolBase : IProtocol
    {
        /// <summary>
        ///     Initializes a new instance of the ProtocolBase class to be extended by derived types.
        /// </summary>
        /// <param name="connectionString">Connection string to be parsed.</param>
        protected ProtocolBase(string connectionString)
        {
            var fields = ConnectionStringParser.Parse(connectionString);
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (
                var property in
                    properties.Where(property => property.Name != "Type" && fields.ContainsKey(property.Name)))
            {
                if (property.PropertyType == typeof (bool))
                {
                    property.SetValue(this, Convert.ToBoolean(fields[property.Name].Trim()), null);
                }
                else if (property.PropertyType == typeof (int))
                {
                    property.SetValue(this, Convert.ToInt32(fields[property.Name].Trim()), null);
                }
                else if (property.PropertyType.BaseType == typeof (Enum))
                {
                    property.SetValue(this, Enum.Parse(property.PropertyType, fields[property.Name]), null);
                }
                else
                {
                    property.SetValue(this, fields[property.Name], null);
                }
            }
        }

        protected ProtocolBase()
        {
        }

        /// <summary>
        ///     Gets serialized representation of the connection string.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                var connectionString = string.Format("Type={0};", GetType().Name);
                var properties =
                    GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty |
                                       BindingFlags.GetProperty)
                        .Where(p => p.Name != "ConnectionString");

                foreach (var property in properties)
                {
                    var prop = property.GetValue(this, null);
                    if (prop != null)
                    {
                        var val = prop.ToString();

                        if (!string.IsNullOrEmpty(val))
                        {
                            // Escape = and ; characters
                            connectionString += string.Format(
                                "{0}={1};", property.Name.Replace(";", @"\;").Replace("=", @"\="),
                                val.Replace(";", @"\;").Replace("=", @"\="));
                        }
                    }
                }

                return connectionString;
            }
        }

        // Password field may contain the illegal ';' character so it is always the last field and isolated
        public abstract bool Send(string fileName, Stream file, Report report, SerializableException exception);

        internal string GetSettingsPasswordField(string connectionString)
        {
            return connectionString.Substring(connectionString.ToLower().IndexOf("password=", StringComparison.Ordinal) + 9)
                .Substring(0, connectionString.Substring(connectionString.ToLower().IndexOf("password=", StringComparison.Ordinal) + 9).Length - 1);
        }
    }
}