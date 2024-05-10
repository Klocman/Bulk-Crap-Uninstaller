using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;

[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Windows-only app")]
[assembly: SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Address properly at some point? Almost all of these get logged")]

[assembly: SuppressMessage("Style", "IDE0057:Use range operator", Justification = "Keep code portable")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Keep code portable")]
[assembly: SuppressMessage("Style", "IDE0090:Use 'new(...)'", Justification = "Keep code portable")]
[assembly: SuppressMessage("Style", "IDE0066:Convert switch statement to expression", Justification = "Keep code portable")]

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyCompany("Marcin Szeniak")]
[assembly: AssemblyCopyright("Copyright © 2023")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: NeutralResourcesLanguage("en")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:

[assembly: AssemblyVersion("5.8.0.0")]
