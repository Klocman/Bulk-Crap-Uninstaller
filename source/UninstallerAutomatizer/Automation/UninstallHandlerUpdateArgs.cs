/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;

namespace UninstallerAutomatizer
{
    public class UninstallHandlerUpdateArgs : EventArgs, IEquatable<UninstallHandlerUpdateArgs>
    {
        public UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind updateKind, string message)
        {
            Message = message;
            UpdateKind = updateKind;
        }

        public string Message { get; }
        public UninstallHandlerUpdateKind UpdateKind { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as UninstallHandlerUpdateArgs;
            return Equals(other);
        }

        public bool Equals(UninstallHandlerUpdateArgs other)
        {
            return other != null && (string.Equals(Message, other.Message) && UpdateKind == other.UpdateKind);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Message?.GetHashCode() ?? 0)*397) ^ (int) UpdateKind;
            }
        }
    }
}