using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
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
            var tempDir = Directory.CreateTempSubdirectory();
            try
            {
                Console.WriteLine("Extracting zip file...");
                Console.WriteLine($"Arguments: {string.Join(", ", args)}");
                var zipFileName = args[0];

                if (zipFileName != string.Empty)
                {
                    using var fileStream = File.Open($"{zipFileName}", FileMode.Open);
                    using (var zipFile = new ZipArchive(fileStream))
                    {
                        var root = zipFile.Entries.FirstOrDefault();
                        foreach (ZipArchiveEntry zipFileEntry in zipFile.Entries)
                        {
                            var pathParts = zipFileEntry.FullName.Split(Path.DirectorySeparatorChar).Skip(1);
                            if (pathParts.Count() > 1)
                            {
                                string directory = $"{string.Join(Path.DirectorySeparatorChar, pathParts.Take(pathParts.Count() - 1))}";

                                if (!Directory.Exists($"{tempDir.FullName}{Path.DirectorySeparatorChar}{directory}"))
                                    tempDir.CreateSubdirectory(directory);
                            }

                            if (zipFileEntry.Name == string.Empty) continue;

                            string path = $"{string.Join(Path.DirectorySeparatorChar, pathParts)}";

                            if (!path.Contains(AppDomain.CurrentDomain.FriendlyName))
                            {
                                zipFileEntry.ExtractToFile($"{tempDir.FullName}{Path.DirectorySeparatorChar}{path}", true);
                            }
                            else
                            {
                                var newPath = $"{tempDir.FullName}{Path.DirectorySeparatorChar}{Path.GetFileName(path)}.2{Path.GetExtension(path)}";
                                zipFileEntry.ExtractToFile(newPath, true);

                                var version = Assembly.GetExecutingAssembly().GetName().Version;
                                var otherVersion = FileVersionInfo.GetVersionInfo(newPath)?.FileVersion;

                                if (otherVersion == null || new Version(otherVersion) <= version)
                                {
                                    File.Delete(newPath);
                                }
                            }
                        }

                        MoveFiles(tempDir, Directory.GetCurrentDirectory());
                    }

                    File.Delete(zipFileName);
                }
                Console.WriteLine($"Done. Everything is up to date!");
                Console.WriteLine($"Open randomizer now? Y/N");
                var input = Console.ReadLine();
                if (input.StartsWith("Y", StringComparison.CurrentCultureIgnoreCase))
                {
                    using var process = new Process();

                    process.StartInfo.FileName = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}/Randomizer.exe";

                    process.Start();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an error, you may have to re-download the program from GitHub. Error: {ex.Message}");

                Console.WriteLine($"Hit any key to close this window.");
                Console.ReadLine();
            }
            finally
            {
                tempDir.Delete(true);
            }
        }

        public static void MoveFiles(DirectoryInfo directoryInfo, string destFolder)
        {

            foreach (var dir in directoryInfo.GetDirectories())
            {
                var dirName = $"{destFolder}{Path.DirectorySeparatorChar}{dir.Name}";
                Directory.CreateDirectory(dirName);
                MoveFiles(dir, dirName);
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                File.Copy(file.FullName, $"{destFolder}{Path.DirectorySeparatorChar}{file.Name}", true);
            }
        }
    }
}
