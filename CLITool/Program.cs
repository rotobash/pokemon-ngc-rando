using System;
using System.IO;
using XDCommon.Utility;

namespace CLITool
{
    class Program
    {
        static void Main(string[] args)
        {
            var directoryName = "./files";
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            var isoExtract = new ISOExtractor("Pokemon XD - Gale of Darkness (USA).iso");
            var iso = isoExtract.ExtractISO();
            
            Console.Read();
        }
    }
}
