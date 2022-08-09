/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Klocman.Properties;

namespace Klocman.IO
{
    public struct FileSize : IXmlSerializable, IComparable<FileSize>, IEquatable<FileSize>, IComparable
    {
        public enum SizeRange
        {
            None = 0,
            Kb,
            Mb,
            Gb,
            Tb
        }

        public static FileSize SumFileSizes(IEnumerable<FileSize> sizes)
        {
            return FromKilobytes(sizes.Sum(x => x.GetKbSize()));
        }

        public static readonly FileSize Empty = new(0);
        private long _sizeInKb;

        public FileSize(long kiloBytes)
        {
            _sizeInKb = kiloBytes;
        }

        public static bool operator !=(FileSize a, FileSize b)
        {
            return !a.Equals(b);
        }

        public static FileSize operator +(FileSize a, FileSize b)
        {
            return new FileSize(a._sizeInKb + b._sizeInKb);
        }

        public static FileSize operator -(FileSize a, FileSize b)
        {
            return new FileSize(a._sizeInKb - b._sizeInKb);
        }

        public static bool operator ==(FileSize a, FileSize b)
        {
            return a.Equals(b);
        }

        public static FileSize FromBytes(long bytes)
        {
            if (bytes < 0)
                bytes = 0;
            return new FileSize(bytes / 1024);
        }

        public static FileSize FromKilobytes(long kiloBytes)
        {
            if (kiloBytes < 0)
                kiloBytes = 0;

            return new FileSize(kiloBytes);
        }

        public int CompareTo(object obj)
        {
            return obj is FileSize other ? CompareTo(other) : 0;
        }

        public override bool Equals(object obj)
        {
            return obj is FileSize other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _sizeInKb.GetHashCode();
        }

        public int CompareTo(FileSize other)
        {
            return _sizeInKb.CompareTo(other._sizeInKb);
        }

        public bool Equals(FileSize other)
        {
            return _sizeInKb == other._sizeInKb;
        }
        
        /// <summary>
        ///     Empty items return long.MaxValue. For use in sorting
        /// </summary>
        public long GetKbSize(bool treatEmptyAsLargest = false)
        {
            if (treatEmptyAsLargest && _sizeInKb <= 0)
                return long.MaxValue;
            return _sizeInKb;
        }

        /// <summary>
        ///     Returns string representation of the unif of this FileSize.
        ///     (eg. Megabytes, Kilobytes)
        /// </summary>
        public string GetUnitName()
        {
            return GetUnitName(_sizeInKb);
        }

        /// <inheritdoc cref="GetUnitName()"/>
        public static string GetUnitName(long sizeInKb)
        {
            var tempSize = sizeInKb;
            if (tempSize <= 0)
                return string.Empty;

            if (tempSize < 1000)
                return Localisation.FileSize_KB_Long;

            tempSize /= 1024;
            if (tempSize < 1000)
                return Localisation.FileSize_MB_Long;

            tempSize /= 1024;
            if (tempSize < 1000)
                return Localisation.FileSize_GB_Long;

            return Localisation.FileSize_TB_Long;
        }

        public float GetCompactSize(out SizeRange sizeRange)
        {
            sizeRange = SizeRange.None;

            if (_sizeInKb <= 0)
                return 0;

            sizeRange = SizeRange.Kb;
            var tempSize = (double)_sizeInKb;
            if (tempSize < 1024)
                return (float)tempSize;

            sizeRange = SizeRange.Mb;
            tempSize /= 1024;
            if (tempSize < 1024)
                return (float)tempSize;

            sizeRange = SizeRange.Gb;
            tempSize /= 1024;
            if (tempSize < 1024)
                return (float)tempSize;

            sizeRange = SizeRange.Tb;
            tempSize /= 1024;
            return (float)tempSize;
        }

        /// <summary>
        ///     Returns string representation of the filesize in format "Number.Decimal ShortName"
        ///     (eg. 32,51 MB, 1021 KB)
        /// </summary>
        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        ///     Returns string representation of the filesize in format "Number.Decimal LongName"
        ///     (eg. 32,51 Megabytes, 1021 Kilobytes)
        /// </summary>
        public string ToString(bool longFormat)
        {
            var value = GetCompactSize(out var range);

            if (range == SizeRange.None || value <= 0)
                return string.Empty;

            return string.Format("{0} {1}", Math.Round(value, 2), GetRangeString(range, longFormat));
        }

        private static string GetRangeString(SizeRange range, bool longString)
        {
            if (range == SizeRange.None)
                return string.Empty;

            string rangeName;
            switch (range)
            {
                case SizeRange.Tb:
                    rangeName = longString ? Localisation.FileSize_TB_Long : Localisation.FileSize_TB_Short;
                    break;
                case SizeRange.Gb:
                    rangeName = longString ? Localisation.FileSize_GB_Long : Localisation.FileSize_GB_Short;
                    break;
                case SizeRange.Mb:
                    rangeName = longString ? Localisation.FileSize_MB_Long : Localisation.FileSize_MB_Short;
                    break;
                case SizeRange.Kb:
                    rangeName = longString ? Localisation.FileSize_KB_Long : Localisation.FileSize_KB_Short;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(range), range, @"Unknown range");
            }

            return rangeName;
        }


        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            if (reader.MoveToContent() == XmlNodeType.Element)
            {
                reader.Read();
                if (reader.HasValue)
                {
                    _sizeInKb = long.Parse(reader.Value);
                    reader.Read();
                }
                reader.ReadEndElement();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteValue(_sizeInKb);
        }

        public long GetRoundedKbSize()
        {
            var size = _sizeInKb;
            if (size <= 0) return 0;
            var result = 1;
            while(size >= 1024)
            {
                size /= 1024;
                result *= 1024;
            }
            return result;
        }
    }
}