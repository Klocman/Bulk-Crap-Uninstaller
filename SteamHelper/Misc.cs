/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;

namespace SteamHelper
{
    public static class Misc
    {
        /// <summary>
        ///     Attempts to separate filename (or filename with path) from the supplied arguments.
        ///     KeyValuePair(filename, arguments)
        /// </summary>
        public static KeyValuePair<string, string> SeparateArgsFromCommand(string fullCommand)
        {
            if (fullCommand == null)
                throw new ArgumentNullException(nameof(fullCommand));

            // Get rid of whitespaces
            fullCommand = fullCommand.Trim();

            if (string.IsNullOrEmpty(fullCommand))
                throw new ArgumentException();

            var firstDot = fullCommand.IndexOf('.');
            if (firstDot < 0)
                throw new FormatException();

            // Check if the path is contained inside of quotation marks.
            // Assume that the quotation mark must come before the dot. Otherwise, it is likely that the arguments use quotations.
            var pathEnd = fullCommand.IndexOf('"', 0, firstDot);

            if (pathEnd >= 0)
            {
                // If yes, find the closing quotation mark and set its index as path end
                pathEnd = fullCommand.IndexOf('"', pathEnd + 1);

                if (pathEnd < 0)
                {
                    // If no ending quote has been found, explode gracefully.
                    throw new FormatException();
                }
                pathEnd += 1; //?
            }

            // If quotation marks were missing, check for any invalid characters after last dot
            // in case of eg: c:\test.dir thing\filename.exe?0 used to get icons
            if (pathEnd < 0)
            {
                var endIndex = 0;
                while (true)
                {
                    var dot = fullCommand.IndexOf('.', endIndex);
                    if (dot < 0)
                        break;

                    var space = fullCommand.IndexOfAny(" ,:;?-=", dot);
                    var dash = fullCommand.IndexOfAny("\\/", dot);
                    if (space < 0)
                        break;

                    if (space < dash || dash < 0)
                    {
                        pathEnd = space;
                        break;
                    }
                    endIndex = dash;
                }
            }

            // Begin extracting filename and arguments
            string filename;
            var args = string.Empty;

            if (pathEnd < 0 || pathEnd >= fullCommand.Length)
            {
                // Looks like there were no arguments, assume whole command is a filename
                filename = fullCommand;
            }
            else
            {
                // pathEnd shows the end of the filename (and start of the arguments)
                filename = fullCommand.Substring(0, pathEnd).TrimEnd();
                args = fullCommand.Substring(pathEnd).TrimStart();
            }

            filename = filename.Trim('"'); // Get rid of the quotation marks
            return new KeyValuePair<string, string>(filename, args);
        }

        public static int IndexOfAny(this string str, IEnumerable<char> chars, int startIndex)
        {
            foreach (var c in chars)
            {
                var i = str.IndexOf(c, startIndex);
                if (i >= 0)
                    return i;
            }
            return -1;
        }
    }
}
