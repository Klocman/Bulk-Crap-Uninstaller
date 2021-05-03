/*
    Copyright (c) 2017 Marcin Szeniak (https://github.com/Klocman/)
    Apache License Version 2.0
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Klocman.Localising
{
    public static class LocalisationExtensions
    {
        private static readonly Dictionary<Enum, string> LocalisedEnumNameCache = new Dictionary<Enum, string>();

        /// <summary>
        ///     Get a fancy name of selected property or field
        /// </summary>
        public static string GetLocalisedMemberName<TContainer, TMember>(this TContainer instance,
            Expression<Func<TContainer, TMember>> selector) where TContainer : class
        {
            var expression = selector.Body as MemberExpression;
            if (expression == null)
                throw new ArgumentException("Selector is invalid, it has to be in format x => x.Property");
            
            var member = expression.Member;
            return GetLocalisedMemberName(member);
        }

        /// <summary>
        ///     Get a fancy name of selected property or field
        /// </summary>
        public static string GetLocalisedMemberName<TContainer, TMember>(Expression<Func<TContainer, TMember>> selector) 
            where TContainer : class
        {
            var expression = selector.Body as MemberExpression;
            if (expression == null)
                throw new ArgumentException("Selector is invalid, it has to be in format x => x.Property");

            var member = expression.Member;
            return GetLocalisedMemberName(member);
        }

        /// <summary>
        ///     Get a fancy name of selected property or field
        /// </summary>
        public static string GetLocalisedMemberName(MemberInfo member)
        {
            var attrib = member.GetCustomAttributes(typeof (LocalisedNameAttribute), false)
                .FirstOrDefault() as LocalisedNameAttribute;

            return attrib == null ? member.Name : attrib.GetName();
        }

        /// <summary>
        ///     Get a fancy name of this enum
        /// </summary>
        public static string GetLocalisedName(this Enum enumValue)
        {
            string output;
            if (LocalisedEnumNameCache.TryGetValue(enumValue, out output))
                return output;

            var getName = new Func<FieldInfo, string>(f =>
            {
                var attribute = f.GetCustomAttributes(typeof(LocalisedNameAttribute), false)
                    .FirstOrDefault() as LocalisedNameAttribute;

                return attribute == null ? f.Name : attribute.GetName();
            });

            var type = enumValue.GetType();

            var isFlags = type.GetCustomAttributes(typeof(FlagsAttribute), true).Any();
            if (isFlags)
            {
                var fields = from object value in Enum.GetValues(type)
                             let flag = Convert.ToInt64(value)
                             where (Convert.ToInt64(enumValue) & flag) == flag
                             select type.GetField(value.ToString());

                var names = fields.Select(f => getName(f)).OrderBy(x => x).ToArray();

                return names.Length > 0 ? string.Join(", ", names) : enumValue.ToString();
            }

            var field = type.GetField(enumValue.ToString());
            output = field == null ? enumValue.ToString() : getName(field);
            LocalisedEnumNameCache.Add(enumValue, output);
            return output;
        }

        /// <summary>
        ///     Get a fancy name using propertyinfo
        /// </summary>
        public static string GetLocalisedName(this PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo.GetCustomAttributes(typeof (LocalisedNameAttribute), false)
                .FirstOrDefault() as LocalisedNameAttribute;

            return attribute == null ? propertyInfo.Name : attribute.GetName();
        }
    }
}