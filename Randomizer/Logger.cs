using Randomizer.Shufflers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using XDCommon;
using XDCommon.Utility;

namespace Randomizer
{
    public static class Logger
    {
        static string LogFilePath;
        static string LogFileName;
        static List<string> LogBuffer;
        public static void CreateNewLogFile(string path, Settings settings)
        {
            LogFilePath = path.RemoveFileExtensions();
            LogFileName = $"-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt";
            LogBuffer = new List<string>
            {
                "================================== Settings File ==================================",
                JsonSerializer.Serialize(settings)
            };
           
        }

        public static void Log(string text)
        {
            if (Configuration.Verbose && LogBuffer != null)
            {
               LogBuffer.Add(text);
            }
        }

        public static void Flush()
        {
            if (Configuration.Verbose && LogBuffer != null)
            {
                using var writer = File.CreateText($"{LogFilePath}{LogFileName}");
                foreach (var line in LogBuffer)
                    writer.Write(line);
                writer.Flush();
            }
        }
    }
}
