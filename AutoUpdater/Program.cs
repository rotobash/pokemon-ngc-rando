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
            Console.WriteLine("Extracting zip file...");
            var zipFileName = string.Empty;
            var currentfileName = $"{AppDomain.CurrentDomain.FriendlyName}.exe";
            var currentdllName = $"{AppDomain.CurrentDomain.FriendlyName}.dll";

            foreach (var fileEntry in Directory.EnumerateFileSystemEntries("."))
            {
                if (Directory.Exists(fileEntry))
                {
                    Directory.Delete(fileEntry, true);
                }
                else if (Path.GetExtension(fileEntry).Contains("zip"))
                {
                    Console.WriteLine($"Found zip file {fileEntry}!");
                    zipFileName = fileEntry;
                }
                else if (!(fileEntry.Contains(currentfileName) || fileEntry.Contains(currentdllName)))
                {
                    try
                    {
                        // Attempt to get a list of security permissions from the folder. 
                        // This will raise an exception if the path is read only or do not have access to view the permissions. 
                        File.Delete(fileEntry);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                }
            }

            try
            {
                if (zipFileName != string.Empty)
                {
                    using var fileStream = File.Open(zipFileName, FileMode.Open);
                    using (var zipFile = new ZipArchive(fileStream))
                    {
                        var root = zipFile.Entries.FirstOrDefault();
                        foreach (ZipArchiveEntry zipFileEntry in zipFile.Entries)
                        {
                            string[] pathSplit = zipFileEntry.FullName.Split(root.FullName);
                            if (pathSplit.All(p => p == string.Empty))
                            {
                                continue;
                            }

                            string path = string.Join(string.Empty, pathSplit);
                            if (zipFileEntry.Name == string.Empty)
                            {
                                _ = Directory.CreateDirectory(path);
                            }
                            else if (!(path.Contains(currentfileName) || path.Contains(currentdllName)))
                            {
                                zipFileEntry.ExtractToFile(path, true);
                            }
                            else
                            {
                                var newPath = $"{Path.GetFileName(path)}.2{Path.GetExtension(path)}";
                                zipFileEntry.ExtractToFile(newPath, true);

                                var version = Assembly.GetExecutingAssembly().GetName().Version;
                                var otherVersion = new Version(FileVersionInfo.GetVersionInfo(newPath).FileVersion);

                                if (otherVersion > version)
                                {
                                    Console.WriteLine($"There was an update to the AutoUpdater. Please delete AutoUpdater.exe and AutoUpdater.dll and rename AutoUpdater2 -> AutoUpdater.");
                                }
                                else
                                {
                                    File.Delete(newPath);
                                }
                            }
                        }
                    }

                    File.Delete(zipFileName);
                }
                Console.WriteLine($"Done. Everything is up to date!");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an error, you may have to re-download the program from GitHub. Error: {ex.Message}");
            }

            Console.WriteLine($"Hit any key to close this window.");
            Console.ReadLine();
        }
    }
}
