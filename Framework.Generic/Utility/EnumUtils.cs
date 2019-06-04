using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Framework.Generic.Utility
{
    public static class EnumUtils
    {
        /// <summary>
        /// Returns all values for a given enum type.
        /// </summary>
        /// <typeparam name="T">The generic enum return type</typeparam>
        /// <returns>Returns an enumerable containing all values for enum <typeparamref name="T"/></returns>
        public static IEnumerable<T> GetValues<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Returns the display value for a given enum.
        /// </summary>
        /// <typeparam name="T">The generic enum return type</typeparam>
        /// <param name="value">The enum value.</param>
        /// <returns>Returns the display value for a given enum <paramref name="value"/></returns>
        public static string GetDisplayValue<T>(this T value) where T : Enum
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var displayAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];

            return displayAttributes == null || displayAttributes.Length == 0 ? value.ToString() : displayAttributes[0].Name;
        }

        /// <summary>
        /// Returns all display values for each item in an enum <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The generic enum return type</typeparam>
        /// <returns>Returns an enumerable containg all values for enum <typeparamref name="T"/></returns>
        public static IEnumerable<string> GetDisplayValues<T>() where T : Enum
        {
            foreach (var enumVal in GetValues<T>())
            {
                yield return GetDisplayValue(enumVal);
            }
        }

        /// <summary>
        /// Returns the description of the <paramref name="value"/>
        /// </summary>
        /// <typeparam name="T">The generic enum return type</typeparam>
        /// <param name="value">The enum value</param>
        /// <returns>The description of the <paramref name="value"/></returns>
        public static string GetDescription<T>(this T value) where T : Enum
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            return descriptionAttributes == null || descriptionAttributes.Length == 0 ? string.Empty : descriptionAttributes[0].Description;
        }

        /// <summary>
        /// Returns the enum value of the <paramref name="value"/> that is supplied.
        /// </summary>
        /// <typeparam name="T">The generic enum return type</typeparam>
        /// <param name="value">The string equivelent of the enum value that should be returned.</param>
        /// <param name="ignoreCase">Identifies if the method should ignore the case when parsing</param>
        /// <returns>Returns the enum value of the <paramref name="value"/> that is supplied.</returns>
        public static T Parse<T>(this string value, bool ignoreCase = true) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
