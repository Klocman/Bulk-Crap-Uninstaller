/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Klocman.Tools
{
    /// <summary>
    /// Compiled get and set methods with drastically improved performance.
    /// </summary>
    public class CompiledPropertyInfo<TInstance>
    {
        public CompiledPropertyInfo(PropertyInfo propertyInfo) : this(propertyInfo, null)
        {
        }

        public CompiledPropertyInfo(PropertyInfo propertyInfo, object tag)
        {
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            PropertyInfo = propertyInfo;
            Tag = tag;

            CompileProperty(propertyInfo);
        }

        private void CompileProperty(PropertyInfo propertyInfo)
        {
            var instanceParam = Expression.Parameter(typeof(TInstance), "instance");

            if (propertyInfo.CanRead)
            {
                var getCall = Expression.Call(instanceParam, propertyInfo.GetGetMethod(true)!);
                var convertedGet = Expression.Convert(getCall, typeof(object));
                var getLambda = Expression.Lambda<Func<TInstance, object>>(convertedGet, instanceParam);
                CompiledGet = getLambda.Compile();
            }

            if (propertyInfo.CanWrite)
            {
                var valueParam = Expression.Parameter(typeof(object), "value");
                var convertedValue = Expression.Convert(valueParam, propertyInfo.PropertyType);
                var setCall = Expression.Call(instanceParam, propertyInfo.GetSetMethod(true)!, convertedValue);
                var setLambda = Expression.Lambda<Action<TInstance, object>>(setCall, instanceParam, valueParam);
                CompiledSet = setLambda.Compile();
            }
        }

        /// <summary>
        /// Takes instance containing this property, and returns a boxed property's value.
        /// Null if property doesn't have a getter.
        /// </summary>
        public Func<TInstance, object> CompiledGet { get; private set; }

        /// <summary>
        /// Takes instance containing this property and a boxed new value to set.
        /// Null if property doesn't have a setter.
        /// </summary>
        public Action<TInstance, object> CompiledSet { get; private set; }

        public PropertyInfo PropertyInfo { get; }
        public object Tag { get; set; }
    }
}