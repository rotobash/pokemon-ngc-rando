using System;
using System.Collections.Generic;
using System.Linq;

namespace XDCommon.Utility.Extensions
{
    public static class EnumExtensions
    {
        public static T[] GetValues<T>() where T : Enum
        {
            var enumValues = Enum.GetValues(typeof(T));
            return enumValues.Cast<T>().ToArray();
        }
    }
}
