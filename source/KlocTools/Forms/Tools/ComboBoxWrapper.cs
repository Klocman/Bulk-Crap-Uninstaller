/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;

namespace Klocman.Forms.Tools
{
    public class ComboBoxWrapper<T>
    {
        public ComboBoxWrapper()
        {
        }

        public ComboBoxWrapper(T wrappedObject)
        {
            WrappedObject = wrappedObject;
        }

        public ComboBoxWrapper(T wrappedObject, Func<T, string> toStringConverter) : this(wrappedObject)
        {
            ToStringConverter = toStringConverter;
        }

        public T WrappedObject { get; set; }
        public Func<T, string> ToStringConverter { get; set; }

        public override string ToString()
        {
            return ToStringConverter == null ? WrappedObject.ToString() : ToStringConverter(WrappedObject);
        }
    }
}