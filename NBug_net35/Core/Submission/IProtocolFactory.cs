// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProtocolFactory.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NBug.Core.Submission
{
    /// <summary>
    ///     The protocol factory creates an IProtocol instance.
    ///     This interface is to get around the fact that the IProtocol interface
    ///     cannot be guaranteed to contain a constructor that takes a connection
    ///     string. At the same time, we allow the protocol implementation to supply
    ///     the name of its connection string Type parameter.
    ///     The protocol factory is only used for protocols that originate as connection
    ///     strings in a configuration file.
    ///     Note! Your implementation of this class MUST HAVE a parameterless constructor.
    /// </summary>
    public interface IProtocolFactory
    {
        string SupportedType { get; }
        IProtocol FromConnectionString(string connectionString);
    }
}