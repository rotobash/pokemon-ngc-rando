using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoUpdater;

namespace Randomizer
{
    public partial class Updater : Form
    {
        const string updateMessage = "In order to finish updating, the tool needs to close. Click 'OK' to close it now.";
        readonly GithubReleaseAsset releaseAsset;
        Progress<float> progressHandler;
        public Updater(GithubRelease updateToVersion)
        {
            InitializeComponent();

            backgroundWorker.DoWork += UpdateTool;
            backgroundWorker.ProgressChanged += UpdateProgress;
            backgroundWorker.RunWorkerCompleted += OnDone;
            backgroundWorker.WorkerReportsProgress = true;

            // bit of a hack to check if we're using the self-contained binary or not
            var isSelfContained = File.Exists("System.dll");
            foreach (var asset in updateToVersion.assets)
            {
                if ((isSelfContained && asset.name.Contains("sc")) 
                    || !(isSelfContained || asset.name.Contains("sc")))
                {
                    releaseAsset = asset;
                    break;
                }
            }
        }

        private void Updater_Load(object sender, EventArgs e)
        {
            progressLabel.Text = "Starting Download...";
            backgroundWorker.RunWorkerAsync();
        }

        public void UpdateTool(object sender, DoWorkEventArgs args)
        {
            var assetHandler = BeginInvoke(new Func<GithubReleaseAsset>(() => releaseAsset));
            var asset = (GithubReleaseAsset)EndInvoke(assetHandler);

            var startUpdateTime = DateTime.UtcNow;
            var totalMB = (float)asset.size / 1024 / 1024;
            progressHandler = new Progress<float>(progress =>
            {
                var currentProgressMB = progress * totalMB;
                var speed = currentProgressMB / (DateTime.UtcNow - startUpdateTime).TotalSeconds;

                progressLabel.Invoke(new Action(() => progressLabel.Text = $"{currentProgressMB:f2}Mb / {totalMB:f2}Mb @ {speed:f2}/MBs"));
                backgroundWorker.ReportProgress((int)Math.Round(progress * 100));
            });

            SelfUpdater.Update(asset, progressHandler);
        }

        public void UpdateProgress(object sender, ProgressChangedEventArgs args)
        {
            progressBar.Value = args.ProgressPercentage;
        }

        public void OnDone(object sender, RunWorkerCompletedEventArgs args)
        {
            progressLabel.Text = "Finished Downloading.";
            MessageBox.Show(updateMessage, "Warning", MessageBoxButtons.OK);
            using var process = new Process();

            process.StartInfo.FileName = "AutoUpdater.exe";
            process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();

            if (process.Start())
            {
                Close();
            }
        }
    }
}
