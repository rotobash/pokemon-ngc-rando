using Randomizer.Colosseum;
using Randomizer.XD;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XDCommon;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace Randomizer
{
    public partial class MainForm : Form
    {
        ISO iso;
        ISOExtractor extractor;
        public MainForm()
        {
            InitializeComponent();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void settingsTabPage_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void loadIsoButton_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Colosseum or XD Game File|*.iso";
            openFileDialog.CheckFileExists = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (!Directory.Exists(Configuration.ExtractDirectory))
                {
                    Directory.CreateDirectory(Configuration.ExtractDirectory);
                }
                extractor = new ISOExtractor(openFileDialog.FileName);
                iso = extractor.ExtractISO();
                IGameExtractor ex ;
                switch (iso.Game)
                {
                    case Game.Colosseum:
                        gamePictureBox.Image = new Bitmap("Images/colo-logo.jpg");
                        ex = new ColoExtractor();
                        break;
                    case Game.XD:
                        gamePictureBox.Image = new Bitmap("Images/xd-logo.jpg");
                        ex = new XDExtractor(iso);
                        break;
                    default:
                        MessageBox.Show("Game not recognized!");
                        return;
                }
                gameLabel.Text = iso.Game.ToString();
                regionLabel.Text = iso.Region.ToString();
            }
        }

        private void optionsButton_Click(object sender, EventArgs e)
        {
            var options = new OptionsForm();
            options.ShowDialog();
        }

        private void randomizerButton_Click(object sender, EventArgs e)
        {
            saveFileDialog.FileName = $"{iso.Game}-{iso.Region}.iso";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var path = saveFileDialog.FileName;
                if (!path.EndsWith(".iso"))
                {
                    path = $"{path}.iso";
                }
                extractor.RepackISO(iso, path);
            }
            
        }
    }
}
