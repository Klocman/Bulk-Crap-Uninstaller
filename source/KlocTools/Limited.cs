/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Globalization;

namespace Klocman
{
    /// <summary>
    /// Wrapper for value types that forces them to be constrained within a specified range of values.
    /// </summary>
    /// <typeparam name="T">Type of the wrapped value</typeparam>
    public struct Limited<T> : IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
        where T : struct, IComparable, IFormattable, IConvertible, IComparable<T>, IEquatable<T>
    {
        T _value, _minimum, _maximum;

        /// <summary>
        /// Wrapped value, always equal to any of or in between Minimum and Maximum.
        /// If ThrowOutOfRange if false and the value is assigned out of this range, it is trimmed.
        /// Otherwise an exception is thrown.
        /// </summary>
        public T Value
        {
            get { return _value; }
            set
            {
                _value = GetLimitedValue(value, Minimum, Maximum, ThrowOutOfRange);
            }
        }

        private static Tvalue GetLimitedValue<Tvalue>(Tvalue value, Tvalue min, Tvalue max, bool throwOutOfRange)
            where Tvalue : IComparable<Tvalue>
        {
            if (value.CompareTo(max) > 0)
            {
                if (throwOutOfRange) throw new ArgumentOutOfRangeException(nameof(value), @"New value must be equal to or in between the minimum and maximum values");
                return max;
            }
            else if (value.CompareTo(min) < 0)
            {
                if (throwOutOfRange) throw new ArgumentOutOfRangeException(nameof(value), @"New value must be equal to or in between the minimum and maximum values");
                return min;
            }
            return value;
        }

        /// <summary>
        /// Minimal value of this variable. If set higher than the current value, the current value is trimmed up.
        /// Must be lower than or equal to the maximal value.
        /// </summary>
        public T Minimum
        {
            get { return _minimum; }
            set
            {
                if (value.CompareTo(_maximum) > 0)
                    throw new ArgumentOutOfRangeException(nameof(value), @"Minimum value must be lower than or equal to the maximum value");
                _minimum = value;

                if (value.CompareTo(_value) > 0)
                    _value = value;
            }
        }

        /// <summary>
        /// Maximal value of this variable. If set lower than the current value, the current value is trimmed down.
        /// Must be higher than or equal to the minimal value.
        /// </summary>
        public T Maximum
        {
            get { return _maximum; }
            set
            {
                if (value.CompareTo(_minimum) < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), @"Minimum value must be lower than or equal to the maximum value");
                _maximum = value;

                if (value.CompareTo(_value) < 0)
                    _value = value;
            }
        }

        /// <summary>
        /// Indicates whether an exception should be thrown or not when assigning an out-of-range value.
        /// If set to false the value is trimmed quietly. False by default. 
        /// </summary>
        public bool ThrowOutOfRange { get; set; }

        /// <summary>
        /// Create new limited value of type T.
        /// </summary>
        /// <param name="value">Initial value of this variable</param>
        /// <param name="min">Minimal value of this variable</param>
        /// <param name="max">Maximal value of this variable</param>
        /// <param name="throwOutOfRange">Indicates whether an exception should be thrown or not when assigning an out-of-range value.</param>
        public Limited(T value, T min, T max, bool throwOutOfRange)
        {
            ThrowOutOfRange = throwOutOfRange;

            if (min.CompareTo(max) > 0)
                throw new ArgumentException("Minimum value must be lower than or equal to the maximum value");
            _minimum = min;
            _maximum = max;

            _value = GetLimitedValue(value, min, max, throwOutOfRange);
        }

        /// <summary>
        /// Create new limited value of type T. It automatically truncates out-of-range values.
        /// </summary>
        /// <param name="value">Initial value of this variable</param>
        /// <param name="min">Minimal value of this variable</param>
        /// <param name="max">Maximal value of this variable</param>
        public Limited(T value, T min, T max) : this(value, min, max, false) { }

        /// <summary>
        /// Create new limited value of type T. The default and minimal values are set using default(T).
        /// </summary>
        /// <param name="max">Maximal value of this variable</param>
        public Limited(T max) : this(default(T), default(T), max) { }

        public int CompareTo(T other)
        {
            return _value.CompareTo(other);
        }

        public int CompareTo(object obj)
        {
            return _value.CompareTo(obj);
        }

        public bool Equals(T other)
        {
            return _value.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return _value.Equals(obj);
        }

        public static bool operator ==(Limited<T> a, Limited<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Limited<T> a, Limited<T> b)
        {
            return !(a == b);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return _value.ToString(format, formatProvider);
        }

        public override string ToString()
        {
            return _value.ToString(CultureInfo.CurrentCulture);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        TypeCode IConvertible.GetTypeCode()
        {
            return _value.GetTypeCode();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            return _value.ToBoolean(provider);
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            return _value.ToByte(provider);
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            return _value.ToChar(provider);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            return _value.ToDateTime(provider);
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            return _value.ToDecimal(provider);
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            return _value.ToDouble(provider);
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            return _value.ToInt16(provider);
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            return _value.ToInt32(provider);
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            return _value.ToInt64(provider);
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            return _value.ToSByte(provider);
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            return _value.ToSingle(provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return _value.ToString(provider);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            return _value.ToType(conversionType, provider);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            return _value.ToUInt16(provider);
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            return _value.ToUInt32(provider);
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            return _value.ToUInt64(provider);
        }
    }
}
