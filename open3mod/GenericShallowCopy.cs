using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace open3mod
{
    // Adapted from http://csharptest.net/372/how-to-implement-a-generic-shallow-clone-for-an-object-in-c/.
    class GenericShallowCopy
    {
        /// <summary>
        /// Shallow copy |instance| to |copy|. Use with care.
        /// 
        /// (This covers the case where we want to shallow copy into an existing object,
        ///  which is not handled by Object.MemberwiseCopy).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="copy"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static void Copy<T>(T copy, T instance)
        {
            var type = instance.GetType();
            var fields = new List<MemberInfo>();
            if (type.GetCustomAttributes(typeof(SerializableAttribute), false).Length == 0)
            {
                var t = type;
                while (t != typeof(Object))
                {
                    Debug.Assert(t != null, "t != null");
                    fields.AddRange(
                        t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                    BindingFlags.DeclaredOnly));
                    t = t.BaseType;
                }
            }
            else
            {
                fields.AddRange(FormatterServices.GetSerializableMembers(instance.GetType()));
            }
            var values = FormatterServices.GetObjectData(instance, fields.ToArray());
            FormatterServices.PopulateObjectMembers(copy, fields.ToArray(), values);
        }
    }
}
