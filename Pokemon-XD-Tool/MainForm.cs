using Randomizer.Colosseum;
using Randomizer.Shufflers;
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
        ISOExtractor isoExtractor;
        IGameExtractor gameExtractor;
        Randomizer randomizer;

        int seed;

        public MainForm()
        {
            InitializeComponent();

            infoToolTip.SetToolTip(movePowerCheck, "Randomize damaging move power. Uses a normal distribution with an average of 80 power and a variance of 70 power.");
            infoToolTip.SetToolTip(moveAccCheck, "Randomize move accuracy between 0 and 100, ");
            infoToolTip.SetToolTip(movePPCheck, "Randomize move PP from 5 to 40 in intervals of 5.");
            infoToolTip.SetToolTip(moveTypeCheck, "Randomize damaging move's type, will avoid None types.");
            infoToolTip.SetToolTip(moveCategoryCheck, "Randomize whether a move is physical or special, XD only unless you've patched Colosseum.");
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
                isoExtractor = new ISOExtractor(openFileDialog.FileName);
                iso = isoExtractor.ExtractISO();
                switch (iso.Game)
                {
                    case Game.Colosseum:
                        gamePictureBox.Image = new Bitmap("Images/colo-logo.jpg");
                        gameExtractor = new ColoExtractor();
                        break;
                    case Game.XD:
                        gamePictureBox.Image = new Bitmap("Images/xd-logo.jpg");
                        gameExtractor = new XDExtractor(iso);
                        break;
                    default:
                        MessageBox.Show("Game not recognized!");
                        return;
                }
                gameLabel.Text = iso.Game.ToString();
                regionLabel.Text = iso.Region.ToString();

                //foreach (var move in moves)
                //{
                //    if (move.Name.ToString().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                //        continue;

                //    File.WriteAllText(
                //        $"{Configuration.ExtractDirectory}/{move.Name}.txt",
                //        $"Index: {move.MoveIndex}\nDescription: {move.MDescription}\nType: {move.Type}\nPP:{move.PP}\nPower:{move.BasePower}\nCategory: {move.Category}\nAccuracy: {move.Accuracy}\nPriority: {move.Priority}\nEffect: {move.Effect}\nEffectAccuracy{move.EffectAccuracy}"
                //    );

                //}
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
                //var path = saveFileDialog.FileName;
                //if (!path.EndsWith(".iso"))
                //{
                //    path = $"{path}.iso";
                //}
                //extractor.RepackISO(iso, path);


                randomizer = new Randomizer(iso, gameExtractor);
                randomizer.RandomizeMoves(new MoveShufflerSettings
                {
                    RandomMovePower = movePowerCheck.Checked,
                    RandomMoveAcc = moveAccCheck.Checked,
                    RandomMovePP = movePPCheck.Checked,
                    RandomMoveTypes = moveTypeCheck.Checked,
                    RandomMoveCategory = moveCategoryCheck.Checked,
                });

                MessageBox.Show("Done!");
            }
        }
    }
}
