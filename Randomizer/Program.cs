using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Randomizer
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            foreach (var file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), $"AutoUpdater.*.2.*"))
            {
                var fileName = string.Join(string.Empty, file.Split(".").Take(1));
                File.Copy(file, $"{fileName}{Path.GetExtension(file)}", true);
                File.Delete(file);
            }

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
