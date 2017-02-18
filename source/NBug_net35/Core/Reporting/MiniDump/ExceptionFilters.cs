// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionFilters.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace NBug.Core.Reporting.MiniDump
{
    /// <summary>
    ///     This class provides some utilities for working with exceptions and exception filters. The assembly will
    ///     get generated (with automatic locking) on first use of this class with <see cref="ILGenerator.Emit(OpCode)" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Code inside of exception filters runs before the stack has been logically unwound, and so the throw point
    ///         is still visible in tools like debuggers, and backout code from finally blocks has not yet been run.
    ///         See http://blogs.msdn.com/rmbyers/archive/2008/12/22/getting-good-dumps-when-an-exception-is-thrown.aspx.
    ///         Filters can also be used to provide more fine-grained control over which exceptions are caught.
    ///     </para>
    ///     <para>
    ///         Be aware, however, that filters run at a surprising time - after an exception has occurred but before
    ///         any finally clause has been run to restore broken invariants for things lexically in scope.  This can lead to
    ///         confusion if you access or manipulate program state from your filter.  See this blog entry for details
    ///         and more specific guidance:
    ///         http://blogs.msdn.com/clrteam/archive/2009/08/25/the-good-and-the-bad-of-exception-filters.aspx.
    ///     </para>
    ///     <para>
    ///         This class relies on Reflection.Emit to generate code which can use filters.  If you are willing to add some
    ///         complexity to your build process, a static technique (like writing in VB and use ILMerge, or rewriting with
    ///         CCI)
    ///         may be a better choice (eg. more performant and easier to specialize).  Again see
    ///         http://blogs.msdn.com/rmbyers/archive/2008/12/22/getting-good-dumps-when-an-exception-is-thrown.aspx for
    ///         details.
    ///     </para>
    /// </remarks>
    internal static class ExceptionFilters
    {
        /// <summary>
        ///     The filter.
        /// </summary>
        private static readonly Action<Action, Func<Exception, bool>, Action<Exception>> filter = GenerateFilter();

        /// <summary>
        ///     Set to true to write the generated assembly to disk for debugging purposes (eg. to run peverify
        ///     and ildasm on in the case of bad codegen).
        /// </summary>
        private static readonly bool writeGeneratedAssemblyToDisk = false;

        /// <summary>
        ///     Execute the body which is not expected to ever throw any exceptions.
        ///     If an exception does escape body, stop in the debugger if one is attached and then fail-fast.
        /// </summary>
        /// <param name="body">
        ///     Body of code to be executed.
        /// </param>
        /// <example>
        ///     To call code that you don’t expect to ever throw an exception you can just wrap it with ExecuteWithFailFast.  If
        ///     any exceptions
        ///     escape it’ll immediately fail fast with a watson report and minidump (at the point of throw), or if a debugger is
        ///     attached break
        ///     at the throw point.
        ///     <code>
        /// // FailFast on throw
        /// ExceptionFilters.FailFast(() =>
        /// {
        ///   // Code you don't expect to throw exceptions
        ///   throw new ApplicationException("Test unexpected exception");
        /// }); // A minidump will be generated here with good stack trace (not a reseted one like it would happen in a catch block)
        /// </code>
        /// </example>
        internal static void FailFast(Action body)
        {
            Filter(
                body,
                e =>
                {
                    Debugger.Log(10, "ExceptionFilter", "Saw unexpected exception: " + e.ToString());

                    // Terminate the process with this fatal error
                    if (Environment.Version.Major >= 4)
                    {
                        // .NET 4 adds a FailFast overload which takes the exception, usefull for getting good watson buckets
                        // This will also cause an attached debugger to stop at the throw point, just as if the exception went unhandled.
                        // Note that this code may be compiled against .NET 2.0 but running in CLR v4, so we want to take advantage of
                        // this API even if it's not available at compile time, so we use a late-bound call.
                        typeof (Environment).InvokeMember(
                            "FailFast", BindingFlags.Static | BindingFlags.InvokeMethod, null, null,
                            new object[] {"Unexpected Exception", e});
                    }
                    else
                    {
                        // The experience isn't quite as nice in CLR v2 and before (no good watson buckets, debugger shows a
                        // 'FatalExecutionEngineErrorException' at this point), but still deubggable.
                        Environment.FailFast("Exception: " + e.GetType().FullName);
                    }

                    return false; // should never be reached
                },
                null);
        }

        /// <summary>
        ///     Execute the body with the specified filter with no handler ever being invoked
        /// </summary>
        /// <param name="body">
        ///     Body of code to be executed.
        /// </param>
        /// <param name="filter">
        ///     Body of filter code to be executed to do something with the filtered exception object.
        /// </param>
        /// <remarks>
        ///     Note that this allocates a delegate and closure class, a small amount of overhead but something that may not be
        ///     appropriate
        ///     for inside of a tight inner loop.  If you want to call this on a very hot path, you may want to consider a direct
        ///     call
        ///     rather than using an anonymous method.
        /// </remarks>
        /// <example>
        ///     Example of a general-purpose exception filter:
        ///     <code>
        ///  ExceptionFilters.Filter(() =>
        ///  {
        /// 		// This is the body of the 'try'
        ///    MyCode();
        ///  }, (ex) =>
        ///  {
        ///    // This is the body of the filter
        ///    System.Environment.FailFast("Unexpected exception: " + ex.Message);
        ///    return false; // don't catch - this code isn't reached
        ///  }); // no catch block needed
        ///  </code>
        /// </example>
        internal static void Filter(Action body, Action<Exception> filter)
        {
            Filter(
                body,
                e =>
                {
                    filter(e);
                    return false;
                },
                null);
        }

        /// <summary>
        ///     Execute the body with the specified filter.
        /// </summary>
        /// <param name="body">
        ///     The code to run inside the "try" block
        /// </param>
        /// <param name="filter">
        ///     Called whenever an exception escapes body, passing the exeption object.
        ///     For exceptions that aren't derived from System.Exception, they'll show up as an instance of
        ///     RuntimeWrappedException.
        /// </param>
        /// <param name="handler">
        ///     Invoked (with the exception) whenever the filter returns true, causes the exception to be swallowed
        /// </param>
        /// <example>
        ///     Example of a general-purpose exception filter:
        ///     <code>
        /// // General-purpose filter
        /// ExceptionFilters.Filter(() => {
        ///   Console.WriteLine("In body");
        ///   throw new ApplicationException("test");
        /// }, (e) => { // Filter block, good for minidumps
        ///   Console.WriteLine("In filter, exception type: {0}", e.GetType().FullName);
        ///   return true;
        /// }, (e) => { // Catch block, just like a real catch(Exception e)
        ///   Console.WriteLine("In catch, exception type: {0}", e.GetType().FullName);
        /// });
        /// </code>
        /// </example>
        internal static void Filter(Action body, Func<Exception, bool> filter, Action<Exception> handler)
        {
            ExceptionFilters.filter(body, filter, handler);
        }

        /// <summary>
        ///     Like a normal C# Try/Catch but allows one catch block to catch multiple different types of exceptions.
        /// </summary>
        /// <typeparam name="TExceptionBase">
        ///     The common base exception type to catch
        /// </typeparam>
        /// <param name="body">
        ///     Code to execute inside the try
        /// </param>
        /// <param name="typesToCatch">
        ///     All exception types to catch (each of which must be derived from or exactly TExceptionBase)
        /// </param>
        /// <param name="handler">
        ///     The catch block to execute when one of the specified exceptions is caught
        /// </param>
        /// <example>
        ///     Sometimes it’s useful to be able to catch multiple distinct exception types with the same catch block (without
        ///     unwinding the
        ///     stack for other exceptions, so unexpected exceptions are easier to debug live or in a dump file).
        ///     <code>
        ///  // Catching multiple exception types at once as System.Exception
        /// 	ExceptionFilters.TryCatch{SystemException}(() =>
        /// 	{
        /// 		throw new ArgumentNullException();
        /// 	},
        /// 	new Type[] { typeof(InvalidCastException), typeof(ArgumentException), typeof(System.IO.FileNotFoundException) },
        /// 	(e) =>
        /// 	{
        /// 		Console.WriteLine("Caught: " + e.Message);
        /// 	});
        ///  </code>
        /// </example>
        internal static void TryCatch<TExceptionBase>(Action body, Type[] typesToCatch, Action<TExceptionBase> handler)
            where TExceptionBase : Exception
        {
            // Verify that every type in typesToCatch is a sub-type of TExceptionBase
#if DEBUG
            foreach (var tc in typesToCatch)
            {
                Debug.Assert(
                    typeof (TExceptionBase).IsAssignableFrom(tc),
                    string.Format("Error: {0} is not a sub-class of {1}", tc.FullName, typeof (TExceptionBase).FullName));
            }

#endif

            Filter(
                body,
                e =>
                {
                    // If the thrown exception is a sub-type (including the same time) of at least one of the provided types then
                    // catch it.
                    foreach (var catchType in typesToCatch)
                    {
                        if (catchType.IsAssignableFrom(e.GetType()))
                        {
                            return true;
                        }
                    }

                    return false;
                },
                e => handler((TExceptionBase) e));
        }

        /// <summary>
        ///     A convenience method for when only the base type of 'Exception' is needed.
        /// </summary>
        /// <param name="body">
        ///     Body of code to be executed.
        /// </param>
        /// <param name="typesToCatch">
        ///     The types To Catch.
        /// </param>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        /// <example>
        ///     <code>
        ///  // Catching multiple exception types at once as System.Exception
        /// 	ExceptionFilters.TryCatch(() =>
        /// 	{
        /// 		throw new ArgumentNullException();
        /// 	},
        /// 	new Type[] { typeof(InvalidCastException), typeof(ArgumentException), typeof(System.IO.FileNotFoundException) },
        /// 	(e) =>
        /// 	{
        /// 		Console.WriteLine("Caught: " + e.Message);
        /// 	});
        ///  </code>
        /// </example>
        internal static void TryCatch(Action body, Type[] typesToCatch, Action<Exception> handler)
        {
            TryCatch<Exception>(body, typesToCatch, handler);
        }

        /// <summary>
        ///     Generate a function which has an EH filter
        /// </summary>
        /// <returns>
        ///     The new generated assembly with exception filtering capabilities.
        /// </returns>
        private static Action<Action, Func<Exception, bool>, Action<Exception>> GenerateFilter()
        {
            // Create a dynamic assembly with reflection emit
            var name = new AssemblyName("DynamicFilter");
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                name, writeGeneratedAssemblyToDisk ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run);
            ModuleBuilder module;
            if (writeGeneratedAssemblyToDisk)
            {
                module = assembly.DefineDynamicModule("DynamicFilter", "DynamicFilter.dll");
            }
            else
            {
                module = assembly.DefineDynamicModule("DynamicFilter");
            }

            // Add an assembly attribute that tells the CLR to wrap non-CLS-compliant exceptions in a RuntimeWrappedException object
            // (so the cast to Exception in the code will always succeed).  C# and VB always do this, C++/CLI doesn't.
            assembly.SetCustomAttribute(
                new CustomAttributeBuilder(
                    typeof (RuntimeCompatibilityAttribute).GetConstructor(new Type[] {}),
                    new object[] {},
                    new[] {typeof (RuntimeCompatibilityAttribute).GetProperty("WrapNonExceptionThrows")},
                    new object[] {true}));

            // Add an assembly attribute that tells the CLR not to attempt to load PDBs when compiling this assembly
            assembly.SetCustomAttribute(
                new CustomAttributeBuilder(
                    typeof (DebuggableAttribute).GetConstructor(new[] {typeof (DebuggableAttribute.DebuggingModes)}),
                    new object[] {DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints}));

            // Create the type and method which will contain the filter
            var type = module.DefineType("Filter", TypeAttributes.Class | TypeAttributes.Public);
            var argTypes = new[] {typeof (Action), typeof (Func<Exception, bool>), typeof (Action<Exception>)};
            var meth = type.DefineMethod("InvokeWithFilter", MethodAttributes.Public | MethodAttributes.Static,
                typeof (void), argTypes);

            var il = meth.GetILGenerator();
            var exLoc = il.DeclareLocal(typeof (Exception));

            // Invoke the body delegate inside the try
            il.BeginExceptionBlock();
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCall(OpCodes.Callvirt, typeof (Action).GetMethod("Invoke"), null);

            // Invoke the filter delegate inside the filter block
            il.BeginExceptFilterBlock();
            il.Emit(OpCodes.Castclass, typeof (Exception));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc_0);
            il.EmitCall(OpCodes.Callvirt, typeof (Func<Exception, bool>).GetMethod("Invoke"), null);

            // Invoke the handler delegate inside the catch block
            il.BeginCatchBlock(null);
            il.Emit(OpCodes.Castclass, typeof (Exception));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Ldloc_0);
            il.EmitCall(OpCodes.Callvirt, typeof (Action<Exception>).GetMethod("Invoke"), null);

            il.EndExceptionBlock();
            il.Emit(OpCodes.Ret);

            var bakedType = type.CreateType();
            if (writeGeneratedAssemblyToDisk)
            {
                assembly.Save("DynamicFilter.dll");
            }

            // Construct a delegate to the filter function and return it
            var bakedMeth = bakedType.GetMethod("InvokeWithFilter");
            var del = Delegate.CreateDelegate(typeof (Action<Action, Func<Exception, bool>, Action<Exception>>),
                bakedMeth);
            return (Action<Action, Func<Exception, bool>, Action<Exception>>) del;
        }
    }
}