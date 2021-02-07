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

        int seed = -1;
        int moveSteps = 3;

        public MainForm()
        {
            InitializeComponent();

            backgroundWorker.DoWork += randomizeTask;
            backgroundWorker.RunWorkerCompleted += onRandomizeTaskComplete;

            infoToolTip.SetToolTip(movePowerCheck, "Randomize damaging move power. Uses a normal distribution with an average of 80 power and a variance of 70 power.");
            infoToolTip.SetToolTip(moveAccCheck, "Randomize move accuracy between 0 and 100, ");
            infoToolTip.SetToolTip(movePPCheck, "Randomize move PP from 5 to 40 in intervals of 5.");
            infoToolTip.SetToolTip(moveTypeCheck, "Randomize damaging move's type, will avoid None types.");
            infoToolTip.SetToolTip(moveCategoryCheck, "Randomize whether a move is physical or special, XD only unless you've patched Colosseum.");
        }

        private void loadIsoButton_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Colosseum or XD Game File|*.iso";
            openFileDialog.CheckFileExists = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                progressMessageLabel.Text = "Reading ISO...";
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

                progressMessageLabel.Text = "Successfully read ISO";
            }
        }

        private void optionsButton_Click(object sender, EventArgs e)
        {
            var options = new OptionsForm();
            options.ShowDialog();
        }

        private void randomizerButton_Click(object sender, EventArgs e)
        {
            if (iso != null)
            {
                saveFileDialog.FileName = $"{iso.Game}-{iso.Region}.iso";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    progressBar.Value = 0;
                    progressMessageLabel.Text = "Randomizing Moves...";
                    randomizer = new Randomizer(gameExtractor, seed);
                    randomizer.RandomizeMoves(new MoveShufflerSettings
                    {
                        RandomMovePower = movePowerCheck.Checked,
                        RandomMoveAcc = moveAccCheck.Checked,
                        RandomMovePP = movePPCheck.Checked,
                        RandomMoveTypes = moveTypeCheck.Checked,
                        RandomMoveCategory = moveCategoryCheck.Checked,
                    });

                    progressMessageLabel.Text = "Randomizing Pokemon Traits...";

                    randomizer.RandomizePokemon(new PokemonTraitShufflerSettings
                    {
                        RandomizeBaseStats = baseStatsUnchangedCheck.Checked ? 0 : baseStatsShuffleCheck.Checked ? 1 : 2,
                        StandardizeEXPCurves = standardizeExpCureCheck.Checked,
                        BaseStatsFollowEvolution = bstFollowEvolutionCheck.Checked,
                        UpdateBaseStats = updateBSTCheck.Checked,

                        RandomizeAbilities = randomizeAbilitiesCheck.Checked,
                        AllowWonderGuard = allowWonderGuardCheck.Checked,
                        AbilitiesFollowEvolution = abilitiesFollowEvolutionCheck.Checked,
                        BanNegativeAbilities = banBadAbilitiesCheck.Checked,

                        RandomizeTypes = randomizeTypesCheck.Checked,
                        TypesFollowEvolution = typesFollowEvolutionCheck.Checked,

                        RandomizeEvolutions = randomizeEvolutionsCheck.Checked,
                        EvolutionHasSimilarStrength = evolutionSimilarStrengthCheck.Checked,
                        EvolutionHasSameType = evolutionSameTypeCheck.Checked,
                        ThreeStageEvolution = threeStageMaxCheck.Checked,
                        EasyEvolutions = easyEvolutionsCheck.Checked,
                        FixImpossibleEvolutions = fixImpossibleEvolutionsCheck.Checked,
                    });

                    progressMessageLabel.Text = "Randomizing Trainers...";
                    randomizer.RandomizeTrainers(new TeamShufflerSettings
                    {
                        RandomizePokemon = true
                    });



                    //var path = saveFileDialog.FileName;
                    //if (!path.EndsWith(".iso"))
                    //{
                    //    path = $"{path}.iso";
                    //}
                    //extractor.RepackISO(iso, path);

                    progressMessageLabel.Text = "Finished.";
                    MessageBox.Show("Done!");
                }
            }
        }

        private void randomizeTask(object sender, DoWorkEventArgs e)
        {
            e.Result = true;
        }

        private void progressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void onRandomizeTaskComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Done!");
        }


        private void boostShadowCatchRateCheck_CheckedChanged(object sender, EventArgs e)
        {
            shadowCatchBoostPercent.Enabled = boostShadowCatchRateCheck.Checked;
        }
        private void boostPokeSpotCatchRate_CheckedChanged(object sender, EventArgs e)
        {
            pokeSpotCatchBoostPercent.Enabled = boostPokeSpotCatchRate.Checked;
        }
        private void randomizeTypesCheck_CheckedChanged(object sender, EventArgs e)
        {
            typesFollowEvolutionCheck.Enabled = randomizeTypesCheck.Checked;
        }

        private void baseStatsRandomCheck_CheckedChanged(object sender, EventArgs e)
        {
            bstFollowEvolutionCheck.Enabled = baseStatsRandomCheck.Checked;
        }

        private void randomizeEvolutionsCheck_CheckedChanged(object sender, EventArgs e)
        {
            evolutionSameTypeCheck.Enabled = randomizeEvolutionsCheck.Checked;
            evolutionSimilarStrengthCheck.Enabled = randomizeEvolutionsCheck.Checked;
            threeStageMaxCheck.Enabled = randomizeEvolutionsCheck.Checked;
        }

        private void randomizeAbilitiesCheck_CheckedChanged(object sender, EventArgs e)
        {
            banBadAbilitiesCheck.Enabled = randomizeAbilitiesCheck.Checked;
            allowWonderGuardCheck.Enabled = randomizeAbilitiesCheck.Checked;
            abilitiesFollowEvolutionCheck.Enabled = randomizeAbilitiesCheck.Checked;
        }

        private void randomizeTMsCheck_CheckedChanged(object sender, EventArgs e)
        {
            forceGoodDamagingTMsCheck.Enabled = randomizeTMsCheck.Checked;
            tmCompatibilityPreferTypeCheck.Enabled = randomizeTMsCheck.Checked;
            if (tmCompatibilityPreferTypeCheck.Checked)
                tmCompatibilityUnchangedCheck.Checked = true;
        }

        private void randomizeTutorMoveCheck_CheckedChanged(object sender, EventArgs e)
        {
            forceGoodDamagingTutorMoveCheck.Enabled = randomizeTutorMoveCheck.Checked;
            tutorCompatibilityPreferTypeCheck.Enabled = randomizeTutorMoveCheck.Checked;
            if (tutorCompatibilityPreferTypeCheck.Checked)
                tutorCompatibilityUnchangedCheck.Checked = true;
        }

        private void forceGoodDamagingTMsCheck_CheckedChanged(object sender, EventArgs e)
        {
            forceGoodDamagingTMPercent.Enabled = forceGoodDamagingTMsCheck.Checked;
        }

        private void forceGoodDamagingTutorMoveCheck_CheckedChanged(object sender, EventArgs e)
        {
            forceGoodDamagingTutorMovePercent.Enabled = forceGoodDamagingTutorMoveCheck.Checked;
        }
    }
}
