using System;
using System.Collections.Generic;
using System.IO;
using XDCommon.Contracts;

namespace XDCommon.Utility
{
    public static class StringExtensions
    {
        public static string RemoveFileExtensions(this string fileName)
        {
            return fileName.Split(".")[0];
        }
    }
}
