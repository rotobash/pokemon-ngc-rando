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
            pokemonBSTSelector.Value = Configuration.StrongPokemonBST;
            pokemonImpossibleEvolutionLevelSelector.Value = Configuration.PokemonImpossibleEvolutionLevel;
            pokemonFinalEvolutionLevelSelector.Value = Configuration.PokemonEasierFinalEvolutionLevel;
            pokemonSecondEvolutionLevelSelector.Value = Configuration.PokemonEasierSecondEvolutionLevel;
            movePower.Value = Configuration.GoodDamagingMovePower;
            fileStreamCheck.Checked = !Configuration.UseMemoryStreams;
            verboseLogCheck.Checked = Configuration.Verbose;
            browseForDirButton.Enabled = !fileStreamCheck.Checked;
        }

        private void pokemonBSTSelector_ValueChanged(object sender, EventArgs e)
        {
            Configuration.StrongPokemonBST = (int)pokemonBSTSelector.Value;
        }

        private void browseForDirButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Configuration.ExtractDirectory = folderBrowserDialog.SelectedPath;
                extractDirLabel.Text = Configuration.ExtractDirectory;
                this.infoToolTip.SetToolTip(extractDirLabel, Configuration.ExtractDirectory);
            }
        }

        private void memoryStreamCheck_CheckedChanged(object sender, EventArgs e)
        {
            Configuration.UseMemoryStreams = !fileStreamCheck.Checked;
            browseForDirButton.Enabled = fileStreamCheck.Checked;
        }

        private void verboseLogCheck_CheckedChanged(object sender, EventArgs e)
        {
            Configuration.Verbose = verboseLogCheck.Checked;
        }

        private void movePower_ValueChanged(object sender, EventArgs e)
        {
            Configuration.GoodDamagingMovePower = (int)movePower.Value;
        }

        private void pokemonEvolutionLevelSelector_ValueChanged(object sender, EventArgs e)
        {
            var newLevelValue = (int)pokemonFinalEvolutionLevelSelector.Value;
            Configuration.PokemonEasierFinalEvolutionLevel = newLevelValue;

            if (pokemonSecondEvolutionLevelSelector.Value >= newLevelValue)
            {
                pokemonSecondEvolutionLevelSelector.Value = newLevelValue - 1;
            }
            pokemonSecondEvolutionLevelSelector.Maximum = newLevelValue - 1;
        }

        private void pokemonSecondEvolutionLevelSelector_ValueChanged(object sender, EventArgs e)
        {
            Configuration.PokemonEasierSecondEvolutionLevel = (int)pokemonSecondEvolutionLevelSelector.Value;
        }

        private void bstRangeNumeric_ValueChanged(object sender, EventArgs e)
        {
            Configuration.BSTRange = (int)bstRangeSelector.Value;
        }
    }
}
