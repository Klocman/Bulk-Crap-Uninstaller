/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Diagnostics;

namespace Klocman.Tools
{
    public class ProcessStartCommand
    {
        public ProcessStartCommand(string filename)
            : this(filename, string.Empty)
        {
        }

        /// <exception cref="ArgumentNullException">The values of 'filename' and 'args' cannot be null. </exception>
        public ProcessStartCommand(string filename, string args)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            FileName = filename.Trim().Trim('"', '\'');
            Arguments = args.Trim();
        }

        public string Arguments { get; set; }
        public string FileName { get; set; }

        public static bool TryParse(string command, out ProcessStartCommand result)
        {
            try
            {
                result = ProcessTools.SeparateArgsFromCommand(command);
            }
            catch (Exception)
            {
                result = null;
            }

            return (result != null);
        }

        public string ToCommandLine()
        {
            return ToString();
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(Arguments) ? $"\"{FileName}\"" : $"\"{FileName}\" {Arguments}";
        }

        public ProcessStartInfo ToProcessStartInfo()
        {
            return new ProcessStartInfo(FileName, Arguments) { UseShellExecute = true };
        }
    }
}