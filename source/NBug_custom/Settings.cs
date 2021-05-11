// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="NBug Project">
//   Copyright (c) 2011 - 2013 Teoman Soygul. Licensed under MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NBug.Core.Reporting;
using NBug.Core.Reporting.Info;
using NBug.Core.Submission;
using NBug.Core.Util;
using NBug.Core.Util.Exceptions;
using NBug.Core.Util.Logging;
using NBug.Enums;
using NBug.Events;
using NBug.Properties;
using StoragePath = NBug.Core.Util.Storage.StoragePath;

namespace NBug
{
    public static class Settings
    {
        /// <summary>
        ///     Gets or sets an event for a CustomSubmission.
        /// </summary>
        internal static Delegate CustomSubmissionHandle;

        /// <summary>
        ///     Gets or sets an event for a CustomUI.
        /// </summary>
        internal static Delegate CustomUIHandle;

        /// <summary>
        ///     Lookup for quickly finding the type to instantiate for a given connection string type.
        ///     By making this lazy we don't do the lookup until we know we have to, as
        ///     reflection against all assemblies can be slow.
        /// </summary>
        private static Dictionary<string, IProtocolFactory> _availableProtocols;
            /*= new Dictionary<string, IProtocolFactory>(
			() =>
				{
					// find all concrete implementations of IProtocolFactory
					var type = typeof(IProtocolFactory);
					return
						
				});*/

        private static bool releaseMode; // False by default

        static Settings()
        {
            // Crucial startup settings
            Resources = new PublicResources();
            EntryAssembly = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()) ??
                            Assembly.GetCallingAssembly();

            // GetEntryAssembly() is null if there is no initial GUI/CLI
            NBugDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) ??
                            Environment.CurrentDirectory;
            AdditionalReportFiles = new List<string>();

            // Default to developer mode settings. Settings this now so that any exception below will be handled with correct settings
            ReleaseMode = false;

            // Check to see if the settings are overriden manually. If so, don't load the settings file automatically.
            if (SettingsOverride.Overridden == false)
            {
                /*
				 * Settings file search order:
				 * 1) NBug.config (inside the same folder with 'NBug.dll')
				 * 2) NBug.dll.config (fool proof!) (inside the same folder with 'NBug.dll')
				 * 3) app.config (i.e. MyProduct.exe.config inside the same folder with the main executable 'MyProduct.exe')
				 */
                var path1 = Path.Combine(NBugDirectory, "NBug.config");
                var path2 = Path.Combine(NBugDirectory, "NBug.dll.config");

                /*string path3; // This is automatically handled by System.Configuration*/
                if (File.Exists(path1) && new FileInfo(path1).Length > 0)
                    //check if file is empty to avoid false exceptions
                {
                    try
                    {
                        LoadCustomSettings(XElement.Load(path1));
                        Logger.Trace("Initialized NBug.Settings using the configuration file: " + path1);
                    }
                    catch (Exception exception)
                    {
                        // File is invalid so load default settings
                        LoadAppconfigSettings();
                        Logger.Error(
                            "Default configuration file was either corrupt or empty. Loading default app.config settings. File location: " +
                            path1, exception);
                    }
                }
                else if (File.Exists(path2) && new FileInfo(path2).Length > 0)
                    //check if file is empty to avoid false exceptions
                {
                    try
                    {
                        LoadCustomSettings(XElement.Load(path2));
                        Logger.Trace("Initialized NBug.Settings using the configuration file: " + path2);
                    }
                    catch (Exception exception)
                    {
                        // File is invalid so load default settings
                        LoadAppconfigSettings();
                        Logger.Error(
                            "Default configuration file was either corrupt or empty. Loading default app.config settings. File location: " +
                            path2, exception);
                    }
                }
                else
                {
                    LoadAppconfigSettings();
                }
            }
        }

        /// <summary>
        ///     Gets or sets a list of additional files to be added to the report zip. The files can use * or ? in the same way as
        ///     DOS modifiers.
        /// </summary>
        public static List<string> AdditionalReportFiles { get; set; }

        public static ICollection<IProtocol> Destinations { get; } = new Collection<IProtocol>();

        /// <summary>
        ///     Gets or sets a value indicating whether the application will exit after handling and logging an unhandled
        ///     exception.
        ///     This value is disregarded for anything but UIMode.None. For UIMode.None, you can choose not to exit the application
        ///     which will result in
        ///     'Windows Error Reporting' (aka Dr. Watson) window to kick in. One reason to do so would be to keep in line with
        ///     Windows 7 Logo requirements,
        ///     which is a corner case. This may also be helpful in using the NBug library as a simple unhandled exception logger
        ///     facility, just to log and submit
        ///     exceptions but not interfering with the application execution flow. Default value is true.
        /// </summary>
        public static bool ExitApplicationImmediately { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to handle exceptions even in a corrupted process thought the
        ///     'HandleProcessCorruptedStateExceptions'
        ///     flag. The default value for this is false since generating bug reports for a corrupted process may not be
        ///     successful so use with caution.
        /// </summary>
        public static bool HandleProcessCorruptedStateExceptions { get; set; }

        /// <summary>
        ///     Gets or sets the number of bug reports that can be queued for submission. Each time an unhandled exception occurs,
        ///     the bug report is prepared to
        ///     be send at the next application startup. If submission fails (i.e. there is no Internet connection), the queue
        ///     grows with each additional
        ///     unhandled exception and resulting bug reports. This limits the max no of queued reports to limit the disk space
        ///     usage.
        ///     Default value is 5.
        /// </summary>
        public static int MaxQueuedReports { get; set; }

        /// <summary>
        ///     Gets or sets the memory dump type. Memory dumps are quite useful for replicating the exact conditions that the
        ///     application crashed (i.e.
        ///     getting the stack trace, local variables, etc.) but they take up a great deal of space, so choose wisely. Options
        ///     are:
        ///     None: No memory dump is generated.
        ///     Tiny: Dump size ~200KB compressed.
        ///     Normal: Dump size ~20MB compressed.
        ///     Full: Dump size ~100MB compressed.
        ///     Default value is Tiny.
        /// </summary>
        public static MiniDumpType MiniDumpType { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to enable release mode for the NBug library. In release mode the internal
        ///     developer UI is not displayed and
        ///     unhandled exceptions are only handled if there is no debugger attached to the process. Once properly configured and
        ///     verified to be working
        ///     as intended, NBug release mode should be enabled to be able to properly use the Visual Studio debugger, without
        ///     NBug trying to handle exceptions.
        ///     before Visual Studio does. Default value is false.
        /// </summary>
        public static bool ReleaseMode
        {
            get { return releaseMode; }

            set
            {
                releaseMode = value;

                if (releaseMode)
                {
                    ThrowExceptions = false;
                    DisplayDeveloperUI = false;
                    HandleExceptions = !Debugger.IsAttached;
                    DispatcherIsAsynchronous = true;
                    SkipDispatching = false;
                    RemoveThreadSleep = false;
                }
                else
                {
                    // If developer mode is on (default)
                    ThrowExceptions = true;
                    DisplayDeveloperUI = true;
                    HandleExceptions = true;
                    DispatcherIsAsynchronous = false;
                    SkipDispatching = false;
                    RemoveThreadSleep = true;
                }
            }
        }

        /// <summary>
        ///     Gets or sets a public resources object which provides programmatic access to all string resources used in NBug. You
        ///     can
        ///     programmatically appoint new values for all the user interface texts using this property. You can also localize all
        ///     the
        ///     dialog text, if there is no default localization provided for your language.
        /// </summary>
        public static PublicResources Resources { get; set; }

        /// <summary>
        ///     Gets or sets the time in seconds that report dispatcher waits before starting to submit queued bug reports.
        ///     Dispatcher initializes as
        ///     soon as the application is run but waits for given number of seconds so that it won't slow down the application
        ///     startup.
        ///     Default value is 10 seconds.
        /// </summary>
        public static int SleepBeforeSend { get; set; }

        /// <summary>
        ///     Gets or sets the number of days that NBug will be collecting bug reports for the application. Most of the time, 30
        ///     to 60 days after the
        ///     release, there will be a new release and the current one will be obsolete. Due to this, it is not logical to
        ///     continue to create and submit
        ///     bug reports after a given number of days. After the predefined no of days, the user will still get to see the bug
        ///     report UI but the reports
        ///     will not be actually submitted. Default value is 30 days.
        /// </summary>
        public static int StopReportingAfter { get; set; }

        /// <summary>
        ///     Gets or sets the bug report items storage path. After and unhandled exception occurs, the bug reports are created
        ///     and queued for submission
        ///     on the next application startup. Until then, the reports will be stored in this location. Default value is the
        ///     application executable directory.
        ///     This setting can either be assigned a full path string or a value from <see cref="NBug.Enums.StoragePath" />
        ///     enumeration.
        /// </summary>
        public static StoragePath StoragePath { get; set; }

        /// <summary>
        ///     Gets or sets the UI mode. You should only change this if you read the documentation and understood it. Otherwise
        ///     leave it to auto.
        ///     Default value is Auto.
        /// </summary>
        public static UIMode UIMode { get; set; }

        /// <summary>
        ///     Gets or sets the UI provider. You should only change this if you read the documentation and understood it.
        ///     Otherwise leave it to auto.
        ///     Default value is Auto.
        /// </summary>
        public static UIProvider UIProvider { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to write "NLog.log" file to disk. Otherwise, you can subscribe to log
        ///     events through the
        ///     <see cref="InternalLogWritten" /> event. All the logging is done through System.Diagnostics.Trace.Write() function
        ///     so you can also get
        ///     the log with any trace listener. Default value is true.
        /// </summary>
        public static bool WriteLogToDisk { get; set; }

        /// <summary>
        ///     Gets the Cipher text used for encrypting connection strings before saving to disk. This is automatically generated
        ///     when the
        ///     method <see cref="SaveCustomSettings(Stream, bool)" /> method is called with encryption set to true.
        /// </summary>
        internal static byte[] Cipher { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the dispatcher the class deals with sending of reports to their
        ///     destinations like mail
        ///     address or an issue tracker, runs asynchronously (in a background worker thread as a
        ///     <see cref="System.Threading.Tasks.Task" />).
        ///     By default dispatcher runs on a background thread except for debug builds, where it blocks the UI and runs in a
        ///     synchronous manner.
        ///     This is made so to prevent any exceptions thrown by the dispatcher from being swallowed by the CLR since background
        ///     thread exceptions
        ///     are ignored in most cases, which is not desirable during development (i.e. in a debug build).
        /// </summary>
        internal static bool DispatcherIsAsynchronous { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to enable developer user interface facilities which enable easier diagnosis
        ///     of
        ///     configuration and other internal errors.
        /// </summary>
        internal static bool DisplayDeveloperUI { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to enable network tracing and write the network trace log to
        ///     "NBug.Network.log" file.
        ///     This should only be used for diagnostics, debugging purposes as it slows down network connections considerably.
        ///     Network tracing is disabled by default.
        /// </summary>
        internal static bool? EnableNetworkTrace { get; set; }

        /// <summary>
        ///     Gets or sets the entry assembly which hosts the NBug assembly. It is used for retrieving the version and the full
        ///     name
        ///     of the host application. i.e. Settings.EntryAssembly.GetLoadedModules()[0].Name; @ Info\General.cs
        /// </summary>
        internal static Assembly EntryAssembly { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the unhandled exception handlers in NBug.Handler class actually handle
        ///     exceptions.
        ///     Exceptions will not be handled if the application is in release mode via <see cref="Settings.ReleaseMode" /> and a
        ///     debugger
        ///     is attached to the process. This enables proper debugging of normal exceptions even in the presence of NBug.
        /// </summary>
        internal static bool HandleExceptions { get; set; }

        /// <summary>
        ///     Gets or sets the absolute path to the directory that NBug.dll assembly currently resides. This is used in place of
        ///     CWD
        ///     throughout this assembly to prevent the library from getting affected of CWD changes that happens with
        ///     Directory.SetCurrentDirectory().
        /// </summary>
        internal static string NBugDirectory { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to remove all the <see cref="System.Threading.Thread.Sleep(int)" />
        ///     statements from
        ///     the thread executions. Some thread sleep statements are used to increase the host application performance i.e. the
        ///     <see cref="Settings.SleepBeforeSend" /> halts the execution of <see cref="Dispatcher.Dispatch()" /> for a given
        ///     number of
        ///     seconds to let the host application initialize properly.
        /// </summary>
        internal static bool RemoveThreadSleep { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to skip the report dispatching process altogether.
        /// </summary>
        internal static bool SkipDispatching { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether internal <see cref="NBugException" /> derived types are thrown or
        ///     swallowed.
        ///     Exceptions are NOT thrown by  default except for debug builds. Note that exceptions are caught and re-thrown by the
        ///     Logger.Error() method with added information so stack trace is reset. The inner exceptions should be inspected to
        ///     get
        ///     the actual stack trace.
        /// </summary>
        internal static bool ThrowExceptions { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether to use the deferred reporting feature. With this feature enabled, all bug
        ///     reports are sent
        ///     after the next application start and as a background task. This helps facilitate sending of bug reports with large
        ///     memory dumps
        ///     with them. When this feature is disabled, bug reports are sent as soon as an unhandled exception is caught. For the
        ///     users, it is
        ///     very uncomfortable to wait for bug reports to be sent after an application crash, so it is best to leave this
        ///     feature on.
        ///     Default value is true.
        /// </summary>
        private static bool DeferredReporting { get; set; }

        private static void PopulateProtocols()
        {
            if (_availableProtocols != null)
                return;

            // find all concrete implementations of IProtocolFactory
            var type = typeof (IProtocolFactory);

            _availableProtocols = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(type.IsAssignableFrom)
                .Where(t => t.IsClass)
                .Where(t => !t.IsAbstract)
                .Select(t => (IProtocolFactory) Activator.CreateInstance(t))
                .ToDictionary(f => f.SupportedType);
        }

        public static event EventHandler<CustomSubmissionEventArgs> CustomSubmissionEvent
        {
            add { CustomSubmissionHandle = Delegate.Combine(CustomSubmissionHandle, value); }

            remove { CustomSubmissionHandle = Delegate.Remove(CustomSubmissionHandle, value); }
        }

        public static event EventHandler<CustomUIEventArgs> CustomUIEvent
        {
            add { CustomUIHandle = Delegate.Combine(CustomUIHandle, value); }

            remove { CustomUIHandle = Delegate.Remove(CustomUIHandle, value); }
        }

        /// <summary>
        ///     The internal logger write event for getting notifications for all internal NBug loggers. Using this event, you can
        ///     attach internal NBug
        ///     logs to your applications own logging facility (i.e. log4net, NLog, etc.). First parameters is the message string,
        ///     second one is the log
        ///     category (info, warning, error, etc.).
        /// </summary>
        public static event Action<string, LoggerCategory> InternalLogWritten
        {
            add { Logger.LogWritten += value; }

            remove { Logger.LogWritten -= value; }
        }

        /// <summary>
        ///     This event is fired just before any caught exception is processed, to make them into an orderly bug report.
        ///     Parameters passed with
        ///     this event can be inspected for some internal decision making or to add more information to the bug report.
        ///     Supplied parameters are:
        ///     -First parameter: <see cref="System.Exception" />: This is the actual exception object that is caught to be report
        ///     as a bug. This object
        ///     is processed to extract standard information from it but it can still carry some custom data that you may want to
        ///     use so it is supplied
        ///     as a parameter of this event for your convenience.
        ///     -Second parameter: <see cref="System.Object" />: This is any XML serializable object which can carry any additional
        ///     information to be
        ///     embedded in the actual bug report. For instance you may capture  more information about the system than NBug does
        ///     for you, so you can put
        ///     all those new information in a user defined type and pass it here. You can also pass in any system type that is
        ///     serializable. Make sure
        ///     that passed objects are XML serializable or the information will not appear in the report. See the sample usage for
        ///     proper usage if this
        ///     event.
        /// </summary>
        /// <example>
        ///     A sample code demonstrating the proper use of this event:
        ///     <code>
        ///  NBug.Settings.ProcessingException += (exception, report) =>
        /// 	{
        /// 		report.CustomInfo = new MyCusomSystemInformation { UtcTime = DateTime.UtcNow, AdditionalData = RubyExceptionData.GetInstance(exception) };
        /// 	};
        ///  </code>
        /// </example>
        public static event Action<Exception, Report> ProcessingException
        {
            add { BugReport.ProcessingException += value; }

            remove { BugReport.ProcessingException -= value; }
        }

        /// <summary>
        ///     Adds a destination based on a connection string.
        /// </summary>
        /// <param name="connectionString">Connection string.</param>
        /// <returns>The protocol that was created and added. Null if empty connection string.</returns>
        /// <exception cref="System.ArgumentException">
        ///     The protocol corresponding to the Type parameter in the connection string
        ///     was not found.
        /// </exception>
        public static IProtocol AddDestinationFromConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return null;
            }

            var connectionStringParts = ConnectionStringParser.Parse(connectionString);
            var type = connectionStringParts[@"Type"];
            PopulateProtocols();
            if (!_availableProtocols.ContainsKey(type))
            {
                throw new ArgumentException(string.Format("No protocol factory found for type '{0}'.", type),
                    nameof(connectionString));
            }

            var factory = _availableProtocols[type];
            var protocol = factory.FromConnectionString(connectionString);
            Destinations.Add(protocol);
            return protocol;
        }

        public static IProtocol AddDestinationFromConnectionString(string connectionString, string cipherString)
        {
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(cipherString))
            {
                return null;
            }

            Cipher = Convert.FromBase64String(cipherString);
            return AddDestinationFromConnectionString(Decrypt(connectionString));
        }

        /// <summary>
        ///     This should not be used directly. Rather, <see cref="SettingsOverride.LoadCustomSettings(Stream)" /> should be
        ///     preferred.
        /// </summary>
        internal static void LoadCustomSettings(XElement config)
        {
            // Read defaults first
            UIMode = GetDefaultEnumValue<UIMode>();
            UIProvider = GetDefaultEnumValue<UIProvider>();
            SleepBeforeSend = Convert.ToInt32(GetDefaultValue(() => SleepBeforeSend));
            MaxQueuedReports = Convert.ToInt32(GetDefaultValue(() => MaxQueuedReports));
            StopReportingAfter = Convert.ToInt32(GetDefaultValue(() => StopReportingAfter));
            StoragePath = GetDefaultValue(() => StoragePath);
            MiniDumpType = GetDefaultEnumValue<MiniDumpType>();
            WriteLogToDisk = Convert.ToBoolean(GetDefaultValue(() => WriteLogToDisk));
            ExitApplicationImmediately = Convert.ToBoolean(GetDefaultValue(() => ExitApplicationImmediately));
            HandleProcessCorruptedStateExceptions =
                Convert.ToBoolean(GetDefaultValue(() => HandleProcessCorruptedStateExceptions));
            ReleaseMode = Convert.ToBoolean(GetDefaultValue(() => ReleaseMode));
            DeferredReporting = Convert.ToBoolean(GetDefaultValue(() => DeferredReporting));

            if (config.XPathSelectElement("system.diagnostics") != null &&
                config.XPathSelectElement("system.diagnostics/sharedListeners") != null)
            {
                var traceLog = from networkTrace in config.XPathSelectElements("system.diagnostics/sharedListeners/add")
                    where
                        networkTrace.Attribute("initializeData") != null &&
                        networkTrace.Attribute("initializeData").Value == "NBug.Network.log"
                    select networkTrace;

                if (traceLog.Any())
                {
                    EnableNetworkTrace = true;
                }
            }

            // Read application settings
            var applicationSettings =
                from element in
                    config.Elements("applicationSettings").Elements("NBug.Properties.Settings").Elements("setting")
                where element.Attribute("name") != null && element.Element("value") != null
                select element;

            foreach (var applicationSetting in applicationSettings)
            {
                var property = applicationSetting.Attribute("name").Value;
                var value = applicationSetting.Element("value").Value;

                if (property == GetPropertyName(() => UIMode))
                {
                    UIMode = (UIMode) Enum.Parse(typeof (UIMode), value);
                }
                else if (property == GetPropertyName(() => UIProvider))
                {
                    UIProvider = (UIProvider) Enum.Parse(typeof (UIProvider), value);
                }
                else if (property == GetPropertyName(() => SleepBeforeSend))
                {
                    SleepBeforeSend = Convert.ToInt32(value);
                }
                else if (property == GetPropertyName(() => MaxQueuedReports))
                {
                    MaxQueuedReports = Convert.ToInt32(value);
                }
                else if (property == GetPropertyName(() => StopReportingAfter))
                {
                    StopReportingAfter = Convert.ToInt32(value);
                }
                else if (property == GetPropertyName(() => StoragePath))
                {
                    StoragePath = value;
                }
                else if (property == GetPropertyName(() => MiniDumpType))
                {
                    MiniDumpType = (MiniDumpType) Enum.Parse(typeof (MiniDumpType), value);
                }
                else if (property == GetPropertyName(() => WriteLogToDisk))
                {
                    WriteLogToDisk = Convert.ToBoolean(value);
                }
                else if (property == GetPropertyName(() => ExitApplicationImmediately))
                {
                    ExitApplicationImmediately = Convert.ToBoolean(value);
                }
                else if (property == GetPropertyName(() => HandleProcessCorruptedStateExceptions))
                {
                    HandleProcessCorruptedStateExceptions = Convert.ToBoolean(value);
                }
                else if (property == GetPropertyName(() => ReleaseMode))
                {
                    ReleaseMode = Convert.ToBoolean(value);
                }
                else if (property == GetPropertyName(() => DeferredReporting))
                {
                    DeferredReporting = Convert.ToBoolean(value);
                }
                else
                {
                    Logger.Error(
                        string.Format(
                            "There is a problem with the 'applicationSettings' section of the configuration file. The property read from the file '{0}' is undefined. This is probably a refactoring problem, or a malformed config file.",
                            property));
                }
            }

            // Read connection strings
            var connectionStrings = from element in config.Elements("connectionStrings").Elements("add")
                where element.Attribute("name") != null && element.Attribute("connectionString") != null
                select element;

            foreach (var connectionString in connectionStrings)
            {
                var property = connectionString.Attribute("name").Value;
                var value = connectionString.Attribute("connectionString").Value;
                var prefix = "NBug.Properties.Settings.";

                if (property == prefix + GetPropertyName(() => Cipher))
                {
                    Cipher = Convert.FromBase64String(value);
                }
                else if (property.StartsWith(prefix))
                {
                    var decodedConnectionString = Decrypt(value);
                    try
                    {
                        AddDestinationFromConnectionString(decodedConnectionString);
                    }
                    catch (ArgumentException e)
                    {
                        Logger.Error(e.Message);
                    }
                }
                else
                {
                    Logger.Error(
                        "There is a problem with the 'connectionStrings' section of the configuration file. The property read from the file '" +
                        property
                        + "' is undefined. This is probably a refactoring problem, or malformed config file.");
                }
            }
            PopulateProtocols();
            var x = _availableProtocols;
        }

        /// <summary>
        ///     This should not be used directly. Rather <see cref="SettingsOverride.SaveCustomSettings(Stream)" /> should be
        ///     preferred.
        /// </summary>
        internal static void SaveCustomSettings(Stream settingsFile, bool encryptConnectionStrings)
        {
            XDocument config;

            try
            {
                config = XDocument.Load(XmlReader.Create(settingsFile));
            }
            catch (XmlException)
            {
                // Root element is missing so recreate the configuration file
                config = XDocument.Parse("<?xml version=\"1.0\" encoding=\"utf-8\" ?><configuration></configuration>");
            }

            // Restructure the configuration file
            if (config.Root == null || config.Root.Name != "configuration")
            {
                config =
                    XDocument.Parse(
                        "<?xml version=\"1.0\" encoding=\"utf-8\" ?><configuration><configSections><sectionGroup name=\"applicationSettings\" type=\"System.Configuration.ApplicationSettingsGroup, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" ><section name=\"NBug.Properties.Settings\" type=\"System.Configuration.ClientSettingsSection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" requirePermission=\"false\" /></sectionGroup></configSections><connectionStrings></connectionStrings><applicationSettings><NBug.Properties.Settings></NBug.Properties.Settings></applicationSettings></configuration>");
            }
            else
            {
                if (config.Root.Element("configSections") == null)
                {
                    config.Root.AddFirst(
                        XElement.Parse(
                            "<configSections><sectionGroup name=\"applicationSettings\" type=\"System.Configuration.ApplicationSettingsGroup, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" ><section name=\"NBug.Properties.Settings\" type=\"System.Configuration.ClientSettingsSection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" requirePermission=\"false\" /></sectionGroup></configSections>"));
                }
                else
                {
                    var sectionGroup = from setting in config.Root.Element("configSections").Elements()
                        where
                            setting.Attribute("name") != null &&
                            setting.Attribute("name").Value == "applicationSettings"
                        select setting;

                    if (!sectionGroup.Any())
                    {
                        config.Root.Element("configSections")
                            .Add(
                                XElement.Parse(
                                    "<sectionGroup name=\"applicationSettings\" type=\"System.Configuration.ApplicationSettingsGroup, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" ><section name=\"NBug.Properties.Settings\" type=\"System.Configuration.ClientSettingsSection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" requirePermission=\"false\" /></sectionGroup>"));
                    }
                    else
                    {
                        var nbugSection = from section in sectionGroup.Elements()
                            where
                                section.Attribute("name") != null &&
                                section.Attribute("name").Value == "NBug.Properties.Settings"
                            select section;

                        if (!nbugSection.Any())
                        {
                            sectionGroup.First()
                                .Add(
                                    XElement.Parse(
                                        "<section name=\"NBug.Properties.Settings\" type=\"System.Configuration.ClientSettingsSection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\" requirePermission=\"false\" />"));
                        }
                    }
                }

                if (config.Root.Element("connectionStrings") == null)
                {
                    config.Root.Add(XElement.Parse("<connectionStrings></connectionStrings>"));
                }

                if (config.Root.Element("applicationSettings") == null)
                {
                    config.Root.Add(
                        XElement.Parse(
                            "<applicationSettings><NBug.Properties.Settings></NBug.Properties.Settings></applicationSettings>"));
                }
                else if (config.Root.Element("applicationSettings").Element("NBug.Properties.Settings") == null)
                {
                    config.Root.Element("applicationSettings")
                        .Add(XElement.Parse("<NBug.Properties.Settings></NBug.Properties.Settings>"));
                }
            }

            if (EnableNetworkTrace.HasValue)
            {
                if (EnableNetworkTrace.Value)
                {
                    if (config.Root.XPathSelectElement("system.diagnostics") != null)
                    {
                        config.Root.XPathSelectElement("system.diagnostics").Remove();
                    }

                    config.Root.Add(
                        XElement.Parse(
                            "<system.diagnostics><sources><source name=\"System.Net\" tracemode=\"includehex\" maxdatasize=\"1024\"><listeners><add name=\"System.Net\"/></listeners></source><source name=\"System.Net.Sockets\"><listeners><add name=\"System.Net\"/></listeners></source><source name=\"System.Net.Cache\"><listeners><add name=\"System.Net\"/></listeners></source></sources><switches><add name=\"System.Net\" value=\"Verbose\"/><add name=\"System.Net.Sockets\" value=\"Verbose\"/><add name=\"System.Net.Cache\" value=\"Verbose\"/></switches><sharedListeners><add name=\"System.Net\" type=\"System.Diagnostics.TextWriterTraceListener\" initializeData=\"NBug.Network.log\"/></sharedListeners><trace autoflush=\"true\" indentsize=\"2\"/></system.diagnostics>"));
                }
                else
                {
                    if (config.Root.XPathSelectElement("system.diagnostics") != null)
                    {
                        config.Root.XPathSelectElement("system.diagnostics").Remove();
                    }
                }
            }

            // Replace connection strings
            var prefix = "NBug.Properties.Settings.";
            var connectionStrings = from connString in config.Root.Element("connectionStrings").Elements()
                where connString.Attribute("name") != null && connString.Attribute("name").Value.StartsWith(prefix)
                select connString;
            connectionStrings.Remove();

            if (encryptConnectionStrings)
            {
                if (Cipher == null || Cipher.Length == 0)
                {
                    Cipher = GenerateKey();
                }

                config.Root.Element("connectionStrings")
                    .Add(
                        new XElement(
                            "add",
                            new XAttribute("name", "NBug.Properties.Settings." + GetPropertyName(() => Cipher)),
                            new XAttribute("connectionString", Convert.ToBase64String(Cipher))));
            }
            else
            {
                Cipher = null;
            }

            var i = 1;
            foreach (var destination in Destinations)
            {
                AddConnectionString(config, destination.ConnectionString, i);
                i++;
            }

            // Replace application setting
            var applicationSettings =
                from appSetting in
                    config.Root.Element("applicationSettings").Element("NBug.Properties.Settings").Elements()
                where
                    appSetting.Attribute("name") != null
                    && (appSetting.Attribute("name").Value == GetPropertyName(() => UIMode)
                        || appSetting.Attribute("name").Value == GetPropertyName(() => UIProvider)
                        || appSetting.Attribute("name").Value == GetPropertyName(() => SleepBeforeSend)
                        || appSetting.Attribute("name").Value == GetPropertyName(() => MaxQueuedReports)
                        || appSetting.Attribute("name").Value == GetPropertyName(() => StopReportingAfter)
                        || appSetting.Attribute("name").Value == GetPropertyName(() => StoragePath)
                        || appSetting.Attribute("name").Value == GetPropertyName(() => MiniDumpType)
                        || appSetting.Attribute("name").Value == GetPropertyName(() => WriteLogToDisk)
                        || appSetting.Attribute("name").Value == GetPropertyName(() => ExitApplicationImmediately)
                        ||
                        appSetting.Attribute("name").Value ==
                        GetPropertyName(() => HandleProcessCorruptedStateExceptions)
                        || appSetting.Attribute("name").Value == GetPropertyName(() => ReleaseMode)
                        || appSetting.Attribute("name").Value == GetPropertyName(() => DeferredReporting))
                select appSetting;
            applicationSettings.Remove();

            AddApplicationSetting(config, UIMode, () => UIMode);
            AddApplicationSetting(config, UIProvider, () => UIProvider);
            AddApplicationSetting(config, SleepBeforeSend, () => SleepBeforeSend);
            AddApplicationSetting(config, MaxQueuedReports, () => MaxQueuedReports);
            AddApplicationSetting(config, StopReportingAfter, () => StopReportingAfter);
            AddApplicationSetting(config, MiniDumpType, () => MiniDumpType);
            AddApplicationSetting(config, WriteLogToDisk, () => WriteLogToDisk);
            AddApplicationSetting(config, ExitApplicationImmediately, () => ExitApplicationImmediately);
            AddApplicationSetting(config, HandleProcessCorruptedStateExceptions,
                () => HandleProcessCorruptedStateExceptions);
            AddApplicationSetting(config, ReleaseMode, () => ReleaseMode);
            AddApplicationSetting(config, DeferredReporting, () => DeferredReporting);

            if (StoragePath == Enums.StoragePath.Custom)
            {
                AddApplicationSetting(config, (string) StoragePath, () => StoragePath);
            }
            else
            {
                AddApplicationSetting(config, (Enums.StoragePath) StoragePath, () => StoragePath);
            }

            settingsFile.SetLength(0);
            settingsFile.Flush();
            config.Save(XmlWriter.Create(settingsFile));
            settingsFile.Flush();
        }

        private static void AddApplicationSetting<T>(XDocument document, object content,
            Expression<Func<T>> propertyExpression)
        {
            document.Root.Element("applicationSettings")
                .Element("NBug.Properties.Settings")
                .Add(
                    new XElement(
                        "setting",
                        new XAttribute("name", GetPropertyName(propertyExpression)),
                        new XAttribute("serializeAs", "String"),
                        new XElement("value", content)));
        }

        private static void AddConnectionString(XDocument document, string content, int number)
        {
            if (!string.IsNullOrEmpty(content))
            {
                document.Root.Element("connectionStrings")
                    .Add(
                        new XElement(
                            "add", new XAttribute("name", "NBug.Properties.Settings.Connection" + number),
                            new XAttribute("connectionString", Encrypt(content))));
            }
        }

        private static string Decrypt(string connectionString)
        {
            if (Cipher == null || Cipher.Length == 0)
            {
                return connectionString;
            }
            // Preserve FIPS compliance
            SHA512 hashProvider;
            if (Environment.OSVersion.Version.Major > 5 ||
                (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 2))
            {
                hashProvider = new SHA512CryptoServiceProvider();
            }
            else
            {
                hashProvider = new SHA512Managed();
            }

            using (var hash = hashProvider)
                //using (var cipher = new Rfc2898DeriveBytes(Cipher, hash.ComputeHash(Cipher), 3))
            using (var decryptor = new AesCryptoServiceProvider())
            {
                var cipher = new Rfc2898DeriveBytes(Cipher, hash.ComputeHash(Cipher), 3);
                var key = cipher.GetBytes(decryptor.KeySize/8);
                var iv = cipher.GetBytes(decryptor.BlockSize/8);
                var dec = decryptor.CreateDecryptor(key, iv);

                var connectionStringBytes = Convert.FromBase64String(connectionString);
                    // Reading from config file is always in Base64

                var decryptedBytes = dec.TransformFinalBlock(connectionStringBytes, 0, connectionStringBytes.Length);

                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }

        private static string Encrypt(string connectionString)
        {
            if (Cipher == null || Cipher.Length == 0)
            {
                return connectionString;
            }
            // Preserve FIPS compliance via using xxxCryptoServiceProvider classes where possible
            SHA512 hashProvider;
            if (Environment.OSVersion.Version.Major > 5 ||
                (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 2))
            {
                hashProvider = new SHA512CryptoServiceProvider();
            }
            else
            {
                hashProvider = new SHA512Managed();
            }

            using (var hash = hashProvider)
                //using (var cipher = new Rfc2898DeriveBytes(Cipher, hash.ComputeHash(Cipher), 3))
            using (var encryptor = new AesCryptoServiceProvider())
            {
                var cipher = new Rfc2898DeriveBytes(Cipher, hash.ComputeHash(Cipher), 3);
                var key = cipher.GetBytes(encryptor.KeySize/8);
                var iv = cipher.GetBytes(encryptor.BlockSize/8);
                var enc = encryptor.CreateEncryptor(key, iv);

                var connectionStringBytes = Encoding.UTF8.GetBytes(connectionString);
                var encryptedBytes = enc.TransformFinalBlock(connectionStringBytes, 0, connectionStringBytes.Length);

                return Convert.ToBase64String(encryptedBytes); // Writing to config file is always in Base64
            }
        }

        private static byte[] GenerateKey()
        {
            using (var encryptor = new AesCryptoServiceProvider())
            {
                encryptor.GenerateKey();
                return encryptor.Key;
            }
        }

        private static T GetDefaultEnumValue<T>()
        {
            var defaultSetting =
                typeof (Properties.Settings).GetProperty(typeof (T).Name)
                    .GetCustomAttributes(typeof (DefaultSettingValueAttribute), false)[0] as
                    DefaultSettingValueAttribute;

            try
            {
                return (T) Enum.Parse(typeof (T), defaultSetting.Value);
            }
            catch (Exception exception)
            {
                throw new NBugRuntimeException(
                    "There is no internal default value supplied for '" + typeof (T).Name +
                    "' or the supplied value is invalid. See the inner exception for details.",
                    exception);
            }
        }

        /// <summary>
        ///     Replicate the behavior of normal Properties.Settings class via getting default values for null settings.
        ///     Use this like GetDefaultValue(() =&gt; SleepBeforeSend);
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        private static string GetDefaultValue<T>(Expression<Func<T>> propertyExpression)
        {
            if (typeof(Properties.Settings).GetProperty(((MemberExpression) propertyExpression.Body).Member.Name)
                .GetCustomAttributes(typeof(DefaultSettingValueAttribute), false)[0] is DefaultSettingValueAttribute defaultSetting)
                return defaultSetting.Value;
            return null;
        }

        private static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            return ((MemberExpression) propertyExpression.Body).Member.Name;
        }

        private static void LoadAppconfigSettings()
        {
            // Application settings
            UIMode = Properties.Settings.Default.UIMode;
            UIProvider = Properties.Settings.Default.UIProvider;
            SleepBeforeSend = Properties.Settings.Default.SleepBeforeSend;
            MaxQueuedReports = Properties.Settings.Default.MaxQueuedReports;
            StopReportingAfter = Properties.Settings.Default.StopReportingAfter;
            StoragePath = Properties.Settings.Default.StoragePath;
            MiniDumpType = Properties.Settings.Default.MiniDumpType;
            WriteLogToDisk = Properties.Settings.Default.WriteLogToDisk;
            ExitApplicationImmediately = Properties.Settings.Default.ExitApplicationImmediately;
            HandleProcessCorruptedStateExceptions = Properties.Settings.Default.HandleProcessCorruptedStateExceptions;
            ReleaseMode = Properties.Settings.Default.ReleaseMode;
            DeferredReporting = Properties.Settings.Default.DeferredReporting;

            // Connection strings
            Cipher = Convert.FromBase64String(Properties.Settings.Default.Cipher);
            AddDestinationFromConnectionString(Decrypt(Properties.Settings.Default.Destination1));
            AddDestinationFromConnectionString(Decrypt(Properties.Settings.Default.Destination2));
            AddDestinationFromConnectionString(Decrypt(Properties.Settings.Default.Destination3));
            AddDestinationFromConnectionString(Decrypt(Properties.Settings.Default.Destination4));
            AddDestinationFromConnectionString(Decrypt(Properties.Settings.Default.Destination5));
        }
    }
}