/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;

namespace Klocman.Events
{
    public class CountingUpdateEventArgs : EventArgs
    {
        public CountingUpdateEventArgs(int minimum, int maximum, int value)
        {
            if (minimum < 0 || maximum < 0)
                throw new ArgumentException("min and max values can't be negative");

            Minimum = minimum;
            Maximum = maximum;
            Value = value;

            IsValid = (value >= minimum) && (value <= maximum);
            Finished = value == maximum;
        }

        /// <summary>
        ///     Same as comparing value to the maximum (Value == Maximum)
        /// </summary>
        public bool Finished { get; private set; }

        /// <summary>
        ///     Returns false if the value is out of bounds ((value >= minimum) && (maximum >= value))
        /// </summary>
        public bool IsValid { get; }

        public int Maximum { get; }
        public int Minimum { get; private set; }
        public object Tag { get; set; }
        public int Value { get; }

        /// <summary>
        ///     Get the percentage of this task. 100% is only returned when Finished is true.
        ///     If the values are invalid returns 0
        /// </summary>
        /// <returns></returns>
        public int GetPercentage()
        {
            if (IsValid)
                return (100*Value)/Maximum;
            return 0;
        }
    }
}