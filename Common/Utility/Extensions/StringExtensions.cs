using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XDCommon.Contracts;

namespace XDCommon.Utility
{
    public static class StringExtensions
    {
        public static string RemoveFileExtensions(this string fileName)
        {
            return fileName.Split(".")[0];
        }

        public static bool CheckListEquality<T>(this IEnumerable<T> arr, IEnumerable<T> other)
        {
            return arr.Zip(other, (f, s) => f.Equals(s)).All(i => i);
        }
    }
}
