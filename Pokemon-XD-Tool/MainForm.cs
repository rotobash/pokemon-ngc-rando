using Randomizer.Colosseum;
using Randomizer.Shufflers;
using Randomizer.XD;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
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

        int seed = -1;
        int moveSteps = 3;

        public MainForm()
        {
            InitializeComponent();

            backgroundWorker.DoWork += StartRandomizing;

            infoToolTip.SetToolTip(movePowerCheck, "Randomize damaging move power. Uses a normal distribution with an average of 80 power and a variance of 70 power.");
            infoToolTip.SetToolTip(moveAccCheck, "Randomize move accuracy between 0 and 100, ");
            infoToolTip.SetToolTip(movePPCheck, "Randomize move PP from 5 to 40 in intervals of 5.");
            infoToolTip.SetToolTip(moveTypeCheck, "Randomize damaging move's type, will avoid None types.");
            infoToolTip.SetToolTip(moveCategoryCheck, "Randomize whether a move is physical or special, XD only unless you've patched Colosseum.");
        }

        private void loadIsoButton_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Colosseum or XD Game File|*.iso;*.zip";
            openFileDialog.CheckFileExists = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                progressMessageLabel.Text = "Reading ISO...";

                if (OpenFile())
                {

                    iso = isoExtractor.ExtractISO();
                    switch (iso.Game)
                    {
                        case Game.Colosseum:
                            gamePictureBox.Image = new Bitmap("Images/colo-logo.jpg");
                            gameExtractor = new ColoExtractor();
                            starterComboBox.SelectedItem = "ESPEON";
                            starter2ComboBox.SelectedItem = "UMBREON";
                            break;
                        case Game.XD:
                            gamePictureBox.Image = new Bitmap("Images/xd-logo.jpg");
                            gameExtractor = new XDExtractor(iso);
                            starterComboBox.SelectedItem = "EEVEE";
                            starter2Label.Visible = false;
                            starter2ComboBox.Visible = false;
                            break;
                        default:
                            return;
                    }
                    gameLabel.Text = iso.Game.ToString();
                    regionLabel.Text = iso.Region.ToString();

                    progressMessageLabel.Text = "Successfully read ISO";

                }
            }
        }

        private bool OpenFile()
        {
            // did they give us crap?
            try
            {
                if (openFileDialog.FileName.EndsWith("zip"))
                {
                    if (!Configuration.UseMemoryStreams)
                    {
                        var dialogResult = MessageBox.Show(
                            "Selecting zip files was intended for people using memory stream but you have disabled this option. Hitting Ok will extract the ISO to your disk.",
                            "Just so you know...",
                            MessageBoxButtons.OKCancel
                        );
                        if (dialogResult != DialogResult.OK)
                            return false;
                    }

                    // keep this one open
                    var extractStream = $"{Configuration.ExtractDirectory}/{openFileDialog.SafeFileName}.iso".GetNewStream();
                    using var fileStream = File.OpenRead(openFileDialog.FileName);
                    using var zip = new ZipArchive(fileStream);
                    foreach (var entry in zip.Entries)
                    {
                        if (entry.Name.EndsWith(".iso"))
                        {
                            using var entryStream = entry.Open();
                            entryStream.CopyTo(extractStream);
                            break;
                        }
                    }
                    isoExtractor = new ISOExtractor(extractStream);
                }
                else
                {
                    isoExtractor = new ISOExtractor(openFileDialog.FileName);
                }
                return true;
            }
            catch
            {
                // aha!
                MessageBox.Show("Game not recognized!");
                progressMessageLabel.Text = "Failed";
                if (iso != null || isoExtractor != null)
                {
                    // dereference the iso, the GC will call it's destructor which will free the streams
                    iso = null;
                    isoExtractor = null;
                }
                return false;
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
                saveFileDialog.Filter = "Randomized game file|*.iso";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    backgroundWorker.RunWorkerAsync(saveFileDialog.FileName);
                }
            }
        }

        private void StartRandomizing(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument is string path)
            {
                backgroundWorker.ReportProgress(0);

                var randoInvoke = BeginInvoke(new Func<Randomizer>(() => new Randomizer(gameExtractor, seed)));
                var settingsInvoke = BeginInvoke(new Func<Settings>(() => CreateRandoSettings()));
                var extractorInvoke = BeginInvoke(new Func<ISOExtractor>(() => isoExtractor));
                var settings = EndInvoke(settingsInvoke) as Settings;
                var randomizer = EndInvoke(randoInvoke) as Randomizer;

                backgroundWorker.ReportProgress(10);
                progressMessageLabel.BeginInvoke(new Action(() => progressMessageLabel.Text = "Randomizing Moves..."));
                randomizer.RandomizeMoves(settings.MoveShufflerSettings);

                backgroundWorker.ReportProgress(20);
                progressMessageLabel.BeginInvoke(new Action(() => progressMessageLabel.Text = "Randomizing Pokemon Traits..."));
                randomizer.RandomizePokemonTraits(settings.PokemonTraitShufflerSettings);

                backgroundWorker.ReportProgress(30);
                progressMessageLabel.BeginInvoke(new Action(() => progressMessageLabel.Text = "Randomizing Trainers..."));
                randomizer.RandomizeTrainers(settings.TeamShufflerSettings);

                backgroundWorker.ReportProgress(40);
                progressMessageLabel.BeginInvoke(new Action(() => progressMessageLabel.Text = "Randomizing Items..."));
                randomizer.RandomizeItems(settings.ItemShufflerSettings);

                backgroundWorker.ReportProgress(50);
                progressMessageLabel.BeginInvoke(new Action(() => progressMessageLabel.Text = "Randomizing Statics..."));
                randomizer.RandomizeStatics(settings.StaticPokemonShufflerSettings);

                // will only do something if game is XD
                backgroundWorker.ReportProgress(60);
                progressMessageLabel.BeginInvoke(new Action(() => progressMessageLabel.Text = "Randomizing Bingo..."));
                randomizer.RandomizeBattleBingo(settings.BingoCardShufflerSettings);

                backgroundWorker.ReportProgress(70);
                progressMessageLabel.BeginInvoke(new Action(() => progressMessageLabel.Text = "Randomizing PokeSpots..."));
                randomizer.RandomizePokeSpots();

                backgroundWorker.ReportProgress(80);
                progressMessageLabel.BeginInvoke(new Action(() => progressMessageLabel.Text = "Packing ISO..."));

                //if (!path.EndsWith(".iso"))
                //{
                //    path = $"{path}.iso";
                //}
                //extractor.RepackISO(iso, path);

            }

            progressMessageLabel.BeginInvoke(new Action(() => progressMessageLabel.Text = "Finished."));
            backgroundWorker.ReportProgress(100);
        }

        private Settings CreateRandoSettings()
        {
            // sorry
            return new Settings
            {
                PokemonTraitShufflerSettings = new PokemonTraitShufflerSettings
                {
                    RandomizeBaseStats = baseStatsUnchangedCheck.Checked ? 0 : baseStatsShuffleCheck.Checked ? 1 : 2,
                    StandardizeEXPCurves = standardizeExpCurveCheck.Checked,
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

                    TMCompatibility = tmFullCompatibilityCheck.Checked
                            ? MoveCompatibility.Full : (tmCompatibilityRandomCheck.Checked
                            ? MoveCompatibility.Random : (tmCompatibilityPreferTypeCheck.Checked
                            ? MoveCompatibility.RandomPreferType
                            : MoveCompatibility.Unchanged)),

                    TutorCompatibility = tutorFullCompatibilityCheck.Checked
                            ? MoveCompatibility.Full : (tutorCompatibilityRandomCheck.Checked
                            ? MoveCompatibility.Random : (tutorCompatibilityPreferTypeCheck.Checked
                            ? MoveCompatibility.RandomPreferType
                            : MoveCompatibility.Unchanged)),

                    NoEXP = noEXPCheck.Checked,
                },
                MoveShufflerSettings = new MoveShufflerSettings
                {
                    RandomMovePower = movePowerCheck.Checked,
                    RandomMoveAcc = moveAccCheck.Checked,
                    RandomMovePP = movePPCheck.Checked,
                    RandomMoveTypes = moveTypeCheck.Checked,
                    RandomMoveCategory = moveCategoryCheck.Checked,
                },
                ItemShufflerSettings = new ItemShufflerSettings
                {
                    RandomizeItems = randomizeOverworldItemsCheck.Checked,
                    RandomizeItemQuantity = randomizeItemQuantityCheck.Checked,
                    RandomizeMarts = randomizeMartItems.Checked,
                    MartsSellEvoStones = martsSellEvoStonesCheck.Checked,
                    MartsSellXItems = martsSellXItemsCheck.Checked,

                    RandomizeTMs = randomizeTMsCheck.Checked,
                    TMForceGoodDamagingMove = forceGoodDamagingTMsCheck.Checked,
                    TMGoodDamagingMovePercent = (float)(forceGoodDamagingTMPercent.Value / 100),

                    RandomizeTutorMoves = randomizeTutorMoveCheck.Checked,
                    TutorForceGoodDamagingMove = forceGoodDamagingTutorMoveCheck.Checked,
                    TutorGoodDamagingMovePercent = (float)(forceGoodDamagingTutorMovePercent.Value / 100)
                },
                TeamShufflerSettings = new TeamShufflerSettings
                {
                    RandomizePokemon = randomizeTrainerPokemonCheck.Checked,
                    DontUseLegendaries = noLegendaryOnTrainerCheck.Checked,

                    SetMinimumShadowCatchRate = minimumShadowCatchRateCheck.Checked,
                    ShadowCatchRateMinimum = (float)(shadowCatchMinimum.Value / 100),
                    BoostTrainerLevel = boostTrainerLevelCheck.Checked,
                    BoostTrainerLevelPercent = (float)(boostTrainerLevelPercent.Value / 100),
                    ForceFullyEvolved = forceFullyEvovledLevelCheck.Checked,
                    ForceFullyEvolvedLevel = (int)forceFullyEvolvedLevel.Value,

                    RandomizeHeldItems = randomShadowHeldItemCheck.Checked,
                    BanBadItems = banBadShadowHeldItemsCheck.Checked,

                    RandomizeMovesets = randomizeMovesets.Checked,
                    ForceFourMoves = forceFourMoveCheck.Checked,
                    ForceGoodDamagingMoves = movesetsForceGoodDamagingMoveCheck.Checked,
                    ForceGoodDamagingMovesCount = (int)movesetsForceGoodDamagingMovePercent.Value,
                    MetronomeOnly = movesetsMetronomeOnlyCheck.Checked
                },
                StaticPokemonShufflerSettings = new StaticPokemonShufflerSettings
                {
                    RandomizeMovesets = randomizeMovesets.Checked,
                    ForceFourMoves = forceFourMoveCheck.Checked,

                    Starter = randomStarterCheck.Checked ? StarterRandomSetting.Random
                            : (customStarterCheck.Checked ? StarterRandomSetting.Custom
                            : (randomStarterThreeStageCheck.Checked ? StarterRandomSetting.RandomThreeStage
                            : (randomStarterTwoStageCheck.Checked ? StarterRandomSetting.RandomTwoStage
                            : (randomStarterSingleStageCheck.Checked ? StarterRandomSetting.RandomSingleStage
                            : StarterRandomSetting.Unchanged)))),

                    Starter1 = starterComboBox.Text,
                    Starter2 = starter2ComboBox.Text,

                    Trade = tradeBothRandomCheck.Checked ? TradeRandomSetting.Both
                            : (tradeRandomGivenCheck.Checked ? TradeRandomSetting.Given
                            : TradeRandomSetting.Unchanged)
                },
                BingoCardShufflerSettings = new BingoCardShufflerSettings
                {
                    ForceGoodDamagingMove = bingoUseDamagingMoveCheck.Checked,
                    ForceSTABMove = bingoUseStabMoveCheck.Checked,
                    ForceStrongPokemon = bingoUseStrongPokemon.Checked
                },
            };
        }

        #region Control Event Listeners
        private void reportProgress(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
        private void doneTask(object sender, ProgressChangedEventArgs e)
        {
            MessageBox.Show("Done!");
        }

        private void forceFullyEvovledLevelCheck_CheckedChanged(object sender, EventArgs e)
        {
            forceFullyEvolvedLevel.Enabled = forceFullyEvovledLevelCheck.Checked;
        }
        private void boostTrainerLevelCheck_CheckedChanged(object sender, EventArgs e)
        {
            boostTrainerLevelPercent.Enabled = boostTrainerLevelCheck.Checked;
        }
        private void boostPokeSpotLevelCheck_CheckedChanged(object sender, EventArgs e)
        {
            boostPokeSpotLevelPercent.Enabled = boostPokeSpotLevelCheck.Checked;
        }
        private void boostShadowCatchRateCheck_CheckedChanged(object sender, EventArgs e)
        {
            shadowCatchMinimum.Enabled = minimumShadowCatchRateCheck.Checked;
        }
        private void boostPokeSpotCatchRate_CheckedChanged(object sender, EventArgs e)
        {
            pokeSpotCatchMinimum.Enabled = minimumPokeSpotCatchRate.Checked;
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

        private void customStarterCheck_CheckedChanged(object sender, EventArgs e)
        {
            starterLabel.Enabled = customStarterCheck.Checked;
            starterComboBox.Enabled = customStarterCheck.Checked;
            starter2Label.Enabled = customStarterCheck.Checked;
            starter2ComboBox.Enabled = customStarterCheck.Checked;
        }

        private void movesetsForceGoodDamagingMoveCheck_CheckedChanged(object sender, EventArgs e)
        {
            movesetsForceGoodDamagingMovePercent.Enabled = movesetsForceGoodDamagingMoveCheck.Checked;
        }
        #endregion
    }
}
