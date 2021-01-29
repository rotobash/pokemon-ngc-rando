using System;
using System.Collections.Generic;
using System.IO;

namespace XDCommon.Utility
{
    public static class StringExtensions
    {
        public static string RemoveFileExtensions(this string fileName)
        {
            return fileName.Split(".")[0];
        }

        public static string GetSafeFileName(this string fileName, string path, FileTypes fileType)
        {
            var safeFileName = fileName;
            var counter = 1;
            while (File.Exists($"{path}/{safeFileName}"))
            {
                safeFileName = $"{fileName.RemoveFileExtensions()}{counter,4}{fileType.FileTypeName()}";
                counter++;
            }

            return safeFileName;
        }
    }
}
