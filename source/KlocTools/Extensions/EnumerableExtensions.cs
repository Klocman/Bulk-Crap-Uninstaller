/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;

namespace Klocman.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        ///     Wraps the specified disposable in an using statement.
        ///     Dispose is called on the current item when the next item is enumerated,
        ///     at the end of the enumeration, and when an uncaught exception is thrown.
        /// </summary>
        /// <typeparam name="T">Type of the base enumerable</typeparam>
        /// <typeparam name="TDisp">Type of the disposable class</typeparam>
        /// <param name="baseEnumerable">Base enumerable</param>
        /// <param name="disposableGetter">Lambda for getting the disposable</param>
        public static IEnumerable<TDisp> Using<T, TDisp>(this IEnumerable<T> baseEnumerable,
            Func<T, TDisp> disposableGetter) where TDisp : class, IDisposable
        {
            TDisp disposable = null;
            try
            {
                foreach (var item in baseEnumerable)
                {
                    disposable?.Dispose();

                    disposable = disposableGetter(item);
                    yield return disposable;
                }
            }
            finally
            {
                disposable?.Dispose();
            }
        }

        /// <summary>
        ///     Executes a lambda on each item as it is enumerated. Doesn't enumerate.
        ///     WARNING: Will only run for the items that are enumerated. Enumerating twice will
        ///     run the action twice.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseEnumerable">Base enumerable</param>
        /// <param name="action">Action to execute on each of the enumerated items.</param>
        public static IEnumerable<T> Do<T>(this IEnumerable<T> baseEnumerable,
            Action<T> action)
        {
            foreach (var item in baseEnumerable)
            {
                action(item);
                yield return item;
            }
        }

        /// <summary>
        /// Select using the given action, but ignore exceptions and skip offending items.
        /// </summary>
        public static IEnumerable<TOut> Attempt<TIn, TOut>(this IEnumerable<TIn> baseEnumerable,
            Func<TIn, TOut> action)
        {
            foreach (var item in baseEnumerable)
            {
                TOut output;
                try
                {
                    output = action(item);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Attempt failed, skipping. Error: " + e);
                    continue;
                }
                yield return output;
            }
        }
    }
}