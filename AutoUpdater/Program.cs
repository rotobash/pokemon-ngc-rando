using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace AutoUpdater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /**
             * We need a seprate program because the randomizer exe can't replace it self in place
             * Basically, all this does is search for a zip file, if it finds one it extracts it in the working directory
             * Will also skip the AutoUpdater for the same reason this exists. The AutoUpdater shouldn't need an update after
             */
            Console.WriteLine("Extracting zip file...");
            var files = Directory.EnumerateFiles(".");
            var zipFileName = string.Empty;
            foreach (var file in files)
            {
                if (file.EndsWith(".zip"))
                {
                    Console.WriteLine($"Found zip file {file}!");
                    zipFileName = file;
                }
                else if (!file.Contains("AutoUpdater"))
                {
                    File.Delete(file);
                }
            }

            if (zipFileName != string.Empty)
            {
                ZipFile.ExtractToDirectory(zipFileName, "..", true);

                using var fileStream = File.Open(zipFileName, FileMode.Open);
                using (var zipFile = new ZipArchive(fileStream))
                {
                    var root = zipFile.Entries.FirstOrDefault();
                    foreach (var zipFileEntry in zipFile.Entries)
                    {
                        var pathSplit = zipFileEntry.FullName.Split(root.FullName);
                        if (pathSplit.All(p => p == string.Empty) || zipFileEntry.Name.Contains("AutoUpdater"))
                            continue;

                        var path = string.Join(string.Empty, pathSplit);
                        if (zipFileEntry.Name == string.Empty)
                            Directory.CreateDirectory(path);
                        else
                            zipFileEntry.ExtractToFile(path, true);
                    }
                }

                File.Delete(zipFileName);
            }

            Console.WriteLine($"Done. Everything is up to date!");
            Console.WriteLine($"Hit any key to close this window.");
            Console.ReadLine();
        }
    }
}
