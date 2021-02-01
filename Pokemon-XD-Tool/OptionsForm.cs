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
            memoryStreamCheck.Checked = Configuration.UseMemoryStreams;
            verboseLogCheck.Checked = Configuration.Verbose;
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
        }

        private void verboseLogCheck_CheckedChanged(object sender, EventArgs e)
        {
            Configuration.Verbose = verboseLogCheck.Checked;
        }
    }
}
