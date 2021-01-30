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
            var iso = new ISOExtractor("Pokemon XD - Gale of Darkness (USA).iso", true);
            
            
            Console.Read();
        }
    }
}
