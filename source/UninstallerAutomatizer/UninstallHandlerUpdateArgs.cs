/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;

namespace UninstallerAutomatizer
{
    public class UninstallHandlerUpdateArgs : EventArgs
    {
        public UninstallHandlerUpdateArgs(UninstallHandlerUpdateKind updateKind, string message)
        {
            Message = //DateTime.UtcNow.ToShortTimeString() + " - " + 
                message;
            UpdateKind = updateKind;
        }

        public string Message { get; }
        public UninstallHandlerUpdateKind UpdateKind { get; }
    }
}