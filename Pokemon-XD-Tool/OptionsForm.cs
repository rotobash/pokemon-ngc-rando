using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XDCommon;

namespace Randomizer
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            extractDirLabel.Text = Configuration.ExtractDirectory;
            threadCountSelector.Value = Configuration.ThreadCount;
            movePower.Value = Configuration.GoodDamagingMovePower;
            memoryStreamCheck.Checked = Configuration.UseMemoryStreams;
            verboseLogCheck.Checked = Configuration.Verbose;
            browseForDirButton.Enabled = !memoryStreamCheck.Checked;
        }

        private void threadCountSelector_ValueChanged(object sender, EventArgs e)
        {
            Configuration.ThreadCount = (int)threadCountSelector.Value;
        }

        private void browseForDirButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Configuration.ExtractDirectory = folderBrowserDialog.SelectedPath;
                extractDirLabel.Text = Configuration.ExtractDirectory;
            }
        }

        private void memoryStreamCheck_CheckedChanged(object sender, EventArgs e)
        {
            Configuration.UseMemoryStreams = memoryStreamCheck.Checked;
            browseForDirButton.Enabled = !memoryStreamCheck.Checked;
        }

        private void verboseLogCheck_CheckedChanged(object sender, EventArgs e)
        {
            Configuration.Verbose = verboseLogCheck.Checked;
        }

        private void movePower_ValueChanged(object sender, EventArgs e)
        {
            Configuration.GoodDamagingMovePower = (int)movePower.Value;
        }
    }
}
