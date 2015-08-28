// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NBugConfigurationException.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;

namespace NBug.Core.Util.Exceptions
{
    [Serializable]
    public class NBugConfigurationException : NBugException
    {
        public NBugConfigurationException(string message, Exception inner)
            : base(message, inner)
        {
            MisconfiguredProperty = string.Empty;
        }

        private NBugConfigurationException(string propertyName, string message)
            : base(message)
        {
            MisconfiguredProperty = propertyName;
        }

        public string MisconfiguredProperty { get; set; }

        public static NBugConfigurationException Create<T>(Expression<Func<T>> propertyExpression, string message)
        {
            return new NBugConfigurationException(((MemberExpression) propertyExpression.Body).Member.Name, message);
        }
    }
}