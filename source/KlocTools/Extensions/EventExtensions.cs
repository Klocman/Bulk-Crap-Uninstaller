/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.ComponentModel;
using System.Linq;

namespace Klocman.Extensions
{
    public static class EventExtensions
    {
        public static bool CancellableInvoke<T>(this EventHandler<T> handler, object sender, T eventArgs)
            where T : CancelEventArgs
        {
            var cancel = false;

            foreach (var tmp in handler.GetInvocationList().Cast<EventHandler<T>>())
            {
                tmp(sender, eventArgs);
                if (eventArgs.Cancel)
                {
                    cancel = true;
                    break;
                }
            }
            return !cancel;
        }
    }
}