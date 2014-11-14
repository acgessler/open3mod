
// HasFlags() backported from .net 4 to .net 3.5 (and earlier)
// Taken from http://stackoverflow.com/questions/4108828/generic-extension-method-to-see-if-an-enum-contains-a-flag

using System;

namespace open3mod
{
    public static class EnumExtensionsNet4Backported
    {
        /// <summary>
        /// Check to see if a flags enumeration has a specific flag set.
        /// </summary>
        /// <param name="variable">Flags enumeration to check</param>
        /// <param name="value">Flag to check for</param>
        /// <returns></returns>
        public static bool HasFlag(this Enum variable, Enum value)
        {
            if (variable == null)
            {
                return false;
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (!Enum.IsDefined(variable.GetType(), value))
            {
                throw new ArgumentException(string.Format(
                    "Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.",
                    value.GetType(), variable.GetType()));
            }

            var num = Convert.ToUInt64(value);
            return ((Convert.ToUInt64(variable) & num) == num);
        }
    }
}