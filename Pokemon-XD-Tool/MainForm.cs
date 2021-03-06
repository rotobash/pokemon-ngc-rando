﻿using AutoUpdater;
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
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using XDCommon;
using XDCommon.Contracts;
using XDCommon.Utility;

namespace Randomizer
{
    public partial class MainForm : Form
    {
        const string ToolName = "NGC Randomizer";
        ISO iso;
        ISOExtractor isoExtractor;
        IGameExtractor gameExtractor;

        int seed = -1;

        public MainForm()
        {
            InitializeComponent();

            backgroundWorker.DoWork += StartRandomizing;
            backgroundWorker.ProgressChanged += reportProgress;
            backgroundWorker.RunWorkerCompleted += doneTask;
        }

        private void loadIsoButton_Click(object sender, EventArgs e)
        {
            openISODialog.Filter = "Colosseum or XD Game File|*.iso;*.zip";
            openISODialog.CheckFileExists = true;
            if (openISODialog.ShowDialog() == DialogResult.OK)
            {
                iso?.Dispose();
                isoExtractor?.Dispose();

                progressMessageLabel.Text = "Reading ISO...";

                if (OpenFile())
                {
                    iso = isoExtractor.ExtractISO();
                    switch (iso.Game)
                    {
                        case Game.Colosseum:
                        {
                            gamePictureBox.Image = new Bitmap("Images/colo-logo.jpg");
                            gameExtractor = new ColoExtractor();
                            starterComboBox.SelectedItem = "ESPEON";
                            starter2ComboBox.SelectedItem = "UMBREON";
                            // disable XD only options
                            foreach (Control ctrl in battleBingoTabPage.Controls) ctrl.Enabled = false;
                            pokeSpotGroupBox.Enabled = false;
                            break;
                        }
                        case Game.XD:
                        {
                            gamePictureBox.Image = new Bitmap("Images/xd-logo.jpg");
                            gameExtractor = new XDExtractor(iso);
                            starterComboBox.SelectedItem = "EEVEE";
                            starter2Label.Visible = false;
                            starter2ComboBox.Visible = false;
                            break;
                        }
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
                if (openISODialog.FileName.EndsWith("zip"))
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
                    var extractStream = $"{Configuration.ExtractDirectory}/{openISODialog.SafeFileName}.iso".GetNewStream();
                    using var fileStream = File.OpenRead(openISODialog.FileName);
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
                    isoExtractor = new ISOExtractor(openISODialog.FileName);
                }
                return true;
            }
            catch
            {
                // aha!
                MessageBox.Show("Game not recognized!");
                progressMessageLabel.Text = "Failed";
                iso?.Dispose();
                isoExtractor?.Dispose();
                iso = null;
                isoExtractor = null;
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
                saveISODialog.FileName = $"{iso.Game}-{iso.Region}.iso";
                saveISODialog.Filter = "Randomized game file|*.iso";
                if (saveISODialog.ShowDialog() == DialogResult.OK)
                {
                    backgroundWorker.RunWorkerAsync(saveISODialog.FileName);
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
                var isoInvoke = BeginInvoke(new Func<ISO>(() => iso));
                var settings = EndInvoke(settingsInvoke) as Settings;
                var randomizer = EndInvoke(randoInvoke) as Randomizer;
                var extractor = EndInvoke(extractorInvoke) as ISOExtractor;
                var gameFile = EndInvoke(isoInvoke) as ISO;

                Logger.CreateNewLogFile(path);

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
                randomizer.RandomizePokeSpots(settings.PokeSpotShufflerSettings);

                backgroundWorker.ReportProgress(80);
                progressMessageLabel.BeginInvoke(new Action(() => progressMessageLabel.Text = "Packing ISO..."));

                Logger.Flush();

                if (!path.EndsWith(".iso"))
                {
                    path = $"{path}.iso";
                }
                extractor.RepackISO(gameFile, path);
            }

            progressMessageLabel.BeginInvoke(new Action(() => progressMessageLabel.Text = "Finished."));
            backgroundWorker.ReportProgress(100);
        }

        #region Settings Save/Load
        // sorry
        private Settings CreateRandoSettings()
        {

            var options = new RandomMoveSetOptions
            {
                RandomizeMovesets = randomizeMovesets.Checked,
                MetronomeOnly = movesetsMetronomeOnlyCheck.Checked,
                BanShadowMoves = banShadowMovesCheck.Checked,
                ForceGoodMoves = movesetsForceGoodDamagingMoveCheck.Checked,
                MinimumGoodMoves = (int)movesetsForceGoodDamagingMovePercent.Value,
                PreferType = randomizeMovesetsPreferType.Checked,
                BanEarlyDragonRage = banEarlyDragonRageCheck.Checked,
                UseLevelUpMoves = pokemonShareMovesetsCheck.Checked,
                ForceFourMoves = forceFourMoveCheck.Checked,
            };

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

                    MoveSetOptions = options
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
                    BanBattleCDs = banBattleCDsCheck.Checked,
                    BanBadItems = banBadItemsCheck.Checked,

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
                    ShadowCatchRateMinimum = (int)shadowCatchMinimum.Value,
                    BoostTrainerLevel = boostTrainerLevelCheck.Checked,
                    BoostTrainerLevelPercent = (float)(boostTrainerLevelPercent.Value / 100),
                    ForceFullyEvolved = forceFullyEvovledLevelCheck.Checked,
                    ForceFullyEvolvedLevel = (int)forceFullyEvolvedLevel.Value,

                    RandomizeHeldItems = randomHeldItemCheck.Checked,
                    BanBadItems = banBadItemsCheck.Checked,
                    BanBattleCDs = banBattleCDsCheck.Checked,

                    MoveSetOptions = options
                },
                StaticPokemonShufflerSettings = new StaticPokemonShufflerSettings
                {
                    MoveSetOptions = options,

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
                            : (tradeRequestedRandomCheck.Checked ? TradeRandomSetting.Requested
                            : TradeRandomSetting.Unchanged)),
                    UsePokeSpotPokemonInTrade = tradeUsePokeSpotCheck.Checked
                },
                BingoCardShufflerSettings = new BingoCardShufflerSettings
                {
                    RandomizeBattleBingoMoveSets = randomizeBattleBingoMovesetsCheck.Checked,
                    RandomizeBattleBingoPokemon = randomizeBattleBingoPokemonCheck.Checked,
                    ForceGoodDamagingMove = bingoUseDamagingMoveCheck.Checked,
                    ForceSTABMove = bingoUseStabMoveCheck.Checked,
                    ForceStrongPokemon = bingoUseStrongPokemon.Checked,
                    BanShadowMoves = bingoBanShadowMovesCheck.Checked
                },
                PokeSpotShufflerSettings = new PokeSpotShufflerSettings
                {
                    RandomizeHeldItems = randomHeldItemCheck.Checked,
                    RandomizePokeSpotPokemon = randomizePokeSpotsCheck.Checked,
                    SetMinimumCatchRate = minimumPokeSpotCatchRate.Checked,
                    MinimumCatchRate = (int)pokeSpotCatchMinimum.Value,
                    BoostPokeSpotLevel = boostPokeSpotLevelCheck.Checked,
                    BoostPokeSpotLevelPercent = (float)(boostPokeSpotLevelPercent.Value / 100),
                    BanBadHeldItems = banBadItemsCheck.Checked,
                    EasyBonsly = easyBonslyCheck.Checked
                }
            };
        }

        private void LoadSettings(Settings settings)
        {
            // pokemon settings
            switch (settings.PokemonTraitShufflerSettings.RandomizeBaseStats)
            {
                case 1:
                    baseStatsShuffleCheck.Checked = true;
                    break;
                case 2:
                    baseStatsRandomCheck.Checked = true;
                    break;
                default:
                    baseStatsUnchangedCheck.Checked = true;
                    break;
            }

            standardizeExpCurveCheck.Checked = settings.PokemonTraitShufflerSettings.StandardizeEXPCurves;
            bstFollowEvolutionCheck.Checked = settings.PokemonTraitShufflerSettings.BaseStatsFollowEvolution;
            updateBSTCheck.Checked = settings.PokemonTraitShufflerSettings.UpdateBaseStats;

            randomizeAbilitiesCheck.Checked = settings.PokemonTraitShufflerSettings.RandomizeAbilities;
            allowWonderGuardCheck.Checked = settings.PokemonTraitShufflerSettings.AllowWonderGuard;
            abilitiesFollowEvolutionCheck.Checked = settings.PokemonTraitShufflerSettings.AbilitiesFollowEvolution;
            banBadAbilitiesCheck.Checked = settings.PokemonTraitShufflerSettings.BanNegativeAbilities;

            randomizeTypesCheck.Checked = settings.PokemonTraitShufflerSettings.RandomizeTypes;
            typesFollowEvolutionCheck.Checked = settings.PokemonTraitShufflerSettings.TypesFollowEvolution;

            randomizeEvolutionsCheck.Checked = settings.PokemonTraitShufflerSettings.RandomizeEvolutions;
            evolutionSimilarStrengthCheck.Checked = settings.PokemonTraitShufflerSettings.EvolutionHasSimilarStrength;
            evolutionSameTypeCheck.Checked = settings.PokemonTraitShufflerSettings.EvolutionHasSameType;
            threeStageMaxCheck.Checked = settings.PokemonTraitShufflerSettings.ThreeStageEvolution;
            easyEvolutionsCheck.Checked = settings.PokemonTraitShufflerSettings.EasyEvolutions;
            fixImpossibleEvolutionsCheck.Checked = settings.PokemonTraitShufflerSettings.FixImpossibleEvolutions;

            switch (settings.PokemonTraitShufflerSettings.TMCompatibility)
            {
                case MoveCompatibility.Full:
                    tmFullCompatibilityCheck.Checked = true;
                    break;
                case MoveCompatibility.Random:
                    tmCompatibilityRandomCheck.Checked = true;
                    break;
                case MoveCompatibility.RandomPreferType:
                    tmCompatibilityPreferTypeCheck.Checked = true;
                    break;
                default:
                    tmCompatibilityUnchangedCheck.Checked = true;
                    break;
            }

            switch (settings.PokemonTraitShufflerSettings.TutorCompatibility)
            {
                case MoveCompatibility.Full:
                    tutorFullCompatibilityCheck.Checked = true;
                    break;
                case MoveCompatibility.Random:
                    tutorCompatibilityRandomCheck.Checked = true;
                    break;
                case MoveCompatibility.RandomPreferType:
                    tutorCompatibilityPreferTypeCheck.Checked = true;
                    break;
                default:
                    tutorCompatibilityUnchangedCheck.Checked = true;
                    break;
            }

            noEXPCheck.Checked = settings.PokemonTraitShufflerSettings.NoEXP;

            // moves
            movePowerCheck.Checked = settings.MoveShufflerSettings.RandomMovePower;
            moveAccCheck.Checked = settings.MoveShufflerSettings.RandomMoveAcc;
            movePPCheck.Checked = settings.MoveShufflerSettings.RandomMovePP;
            moveTypeCheck.Checked = settings.MoveShufflerSettings.RandomMoveTypes;
            moveCategoryCheck.Checked = settings.MoveShufflerSettings.RandomMoveCategory;

            // items
            randomizeOverworldItemsCheck.Checked = settings.ItemShufflerSettings.RandomizeItems;
            randomizeItemQuantityCheck.Checked = settings.ItemShufflerSettings.RandomizeItemQuantity;
            randomizeMartItems.Checked = settings.ItemShufflerSettings.RandomizeMarts;
            banBattleCDsCheck.Checked = settings.ItemShufflerSettings.BanBattleCDs;
            martsSellEvoStonesCheck.Checked = settings.ItemShufflerSettings.MartsSellEvoStones;
            martsSellXItemsCheck.Checked = settings.ItemShufflerSettings.MartsSellXItems;

            // tms/tutors
            randomizeTMsCheck.Checked = settings.ItemShufflerSettings.RandomizeTMs;
            forceGoodDamagingTMsCheck.Checked = settings.ItemShufflerSettings.TMForceGoodDamagingMove;
            forceGoodDamagingTMPercent.Value = (int)(settings.ItemShufflerSettings.TMGoodDamagingMovePercent * 100);

            randomizeTutorMoveCheck.Checked = settings.ItemShufflerSettings.RandomizeTutorMoves;
            forceGoodDamagingTutorMoveCheck.Checked = settings.ItemShufflerSettings.TutorForceGoodDamagingMove;
            forceGoodDamagingTutorMovePercent.Value = (int)(settings.ItemShufflerSettings.TutorGoodDamagingMovePercent * 100);

            // trainers
            randomizeTrainerPokemonCheck.Checked = settings.TeamShufflerSettings.RandomizePokemon;
            noLegendaryOnTrainerCheck.Checked = settings.TeamShufflerSettings.DontUseLegendaries;

            minimumShadowCatchRateCheck.Checked = settings.TeamShufflerSettings.SetMinimumShadowCatchRate;
            shadowCatchMinimum.Value = Math.Clamp(settings.TeamShufflerSettings.ShadowCatchRateMinimum, 0, 255);
            boostTrainerLevelCheck.Checked = settings.TeamShufflerSettings.BoostTrainerLevel;
            boostTrainerLevelPercent.Value = (int)(settings.TeamShufflerSettings.BoostTrainerLevelPercent * 100);
            forceFullyEvovledLevelCheck.Checked = settings.TeamShufflerSettings.ForceFullyEvolved;
            forceFullyEvolvedLevel.Value = settings.TeamShufflerSettings.ForceFullyEvolvedLevel;

            randomHeldItemCheck.Checked = settings.TeamShufflerSettings.RandomizeHeldItems;
            banBadItemsCheck.Checked = settings.TeamShufflerSettings.BanBadItems;

            // movesets
            randomizeMovesets.Checked = settings.TeamShufflerSettings.MoveSetOptions.RandomizeMovesets;
            movesetsMetronomeOnlyCheck.Checked = settings.TeamShufflerSettings.MoveSetOptions.MetronomeOnly;
            randomizeMovesetsPreferType.Checked = settings.TeamShufflerSettings.MoveSetOptions.PreferType;
            banShadowMovesCheck.Checked = settings.TeamShufflerSettings.MoveSetOptions.BanShadowMoves;
            banEarlyDragonRageCheck.Checked = settings.TeamShufflerSettings.MoveSetOptions.BanEarlyDragonRage;
            movesetsForceGoodDamagingMoveCheck.Checked = settings.TeamShufflerSettings.MoveSetOptions.ForceGoodMoves;
            movesetsForceGoodDamagingMovePercent.Value = settings.TeamShufflerSettings.MoveSetOptions.MinimumGoodMoves;
            pokemonShareMovesetsCheck.Checked = settings.TeamShufflerSettings.MoveSetOptions.UseLevelUpMoves;
            forceFourMoveCheck.Checked = settings.TeamShufflerSettings.MoveSetOptions.ForceFourMoves;

            // starters
            switch (settings.StaticPokemonShufflerSettings.Starter)
            {
                case StarterRandomSetting.Custom:
                    customStarterCheck.Checked = true;
                    break;
                case StarterRandomSetting.Random:
                    randomStarterCheck.Checked = true;
                    break;
                case StarterRandomSetting.RandomThreeStage:
                    randomStarterThreeStageCheck.Checked = true;
                    break;
                case StarterRandomSetting.RandomTwoStage:
                    randomStarterTwoStageCheck.Checked = true;
                    break;
                case StarterRandomSetting.RandomSingleStage:
                    randomStarterSingleStageCheck.Checked = true;
                    break;
                default:
                    unchangedStarterCheck.Checked = true;
                    break;
            }

            starterComboBox.Text = settings.StaticPokemonShufflerSettings.Starter1;
            starter2ComboBox.Text = settings.StaticPokemonShufflerSettings.Starter2;

            tradeUsePokeSpotCheck.Checked = settings.StaticPokemonShufflerSettings.UsePokeSpotPokemonInTrade;
            switch (settings.StaticPokemonShufflerSettings.Trade)
            {
                case TradeRandomSetting.Given:
                    tradeRandomGivenCheck.Checked = true;
                    break;
                case TradeRandomSetting.Both:
                    tradeBothRandomCheck.Checked = true;
                    break;
                case TradeRandomSetting.Requested:
                    tradeRequestedRandomCheck.Checked = true;
                    break;
                default:
                    tradeUnchangedCheck.Checked = true;
                    break;
            }

            randomizeBattleBingoPokemonCheck.Checked = settings.BingoCardShufflerSettings.RandomizeBattleBingoPokemon;
            randomizeBattleBingoMovesetsCheck.Checked = settings.BingoCardShufflerSettings.RandomizeBattleBingoMoveSets;
            bingoUseDamagingMoveCheck.Checked = settings.BingoCardShufflerSettings.ForceGoodDamagingMove;
            bingoUseStabMoveCheck.Checked = settings.BingoCardShufflerSettings.ForceSTABMove;
            bingoUseStrongPokemon.Checked = settings.BingoCardShufflerSettings.ForceStrongPokemon;
            bingoBanShadowMovesCheck.Checked = settings.BingoCardShufflerSettings.BanShadowMoves;

            randomizePokeSpotsCheck.Checked = settings.PokeSpotShufflerSettings.RandomizePokeSpotPokemon;
            easyBonslyCheck.Checked = settings.PokeSpotShufflerSettings.EasyBonsly;

            minimumPokeSpotCatchRate.Checked = settings.PokeSpotShufflerSettings.SetMinimumCatchRate;
            pokeSpotCatchMinimum.Value = settings.PokeSpotShufflerSettings.MinimumCatchRate;

            boostPokeSpotLevelCheck.Checked = settings.PokeSpotShufflerSettings.BoostPokeSpotLevel;
            boostPokeSpotLevelPercent.Value = (int)(settings.PokeSpotShufflerSettings.BoostPokeSpotLevelPercent * 100);
        }
        #endregion

        #region Control Event Listeners
        private void reportProgress(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
        private void doneTask(object sender, RunWorkerCompletedEventArgs e)
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

        private void randomizeMovesets_CheckedChanged(object sender, EventArgs e)
        {
            if (!randomizeMovesetsPreferType.Checked)
            {
                banShadowMovesCheck.Enabled = randomizeMovesets.Checked;
                pokemonShareMovesetsCheck.Enabled = randomizeMovesets.Checked;
                banEarlyDragonRageCheck.Enabled = randomizeMovesets.Checked;
            }
        }

        private void randomizeMovesetsPreferType_CheckedChanged(object sender, EventArgs e)
        {
            if (!randomizeMovesets.Checked)
            {
                banShadowMovesCheck.Enabled = randomizeMovesetsPreferType.Checked;
                pokemonShareMovesetsCheck.Enabled = randomizeMovesetsPreferType.Checked;
                banEarlyDragonRageCheck.Enabled = randomizeMovesetsPreferType.Checked;
            }
        }

        private void setSeedButton_Click(object sender, EventArgs e)
        {
            var seedForm = new SetSeedForm();
            if (seedForm.ShowDialog() == DialogResult.OK)
            {
                seed = seedForm.NewSeed;
            }
        }

        private void saveSettingsButton_Click(object sender, EventArgs e)
        {
            if (saveSettingsDialog.ShowDialog() == DialogResult.OK)
            {
                var settings = CreateRandoSettings();
                var json = JsonSerializer.Serialize(settings);
                File.WriteAllText(saveSettingsDialog.FileName, json);
            }
        }

        private void loadSettingButton_Click(object sender, EventArgs e)
        {
            if (openSettingsDialog.ShowDialog() == DialogResult.OK)
            {
                var json = File.ReadAllText(openSettingsDialog.FileName);
                var settings = JsonSerializer.Deserialize<Settings>(json);
                LoadSettings(settings);
            }
        }
        private void randomizeBattleBingoMovesetsCheck_CheckedChanged(object sender, EventArgs e)
        {
            bingoUseDamagingMoveCheck.Enabled = randomizeBattleBingoMovesetsCheck.Checked;
            bingoUseStabMoveCheck.Enabled = randomizeBattleBingoMovesetsCheck.Checked;
            bingoBanShadowMovesCheck.Enabled = randomizeBattleBingoMovesetsCheck.Checked;
        }

        private void randomizeBattleBingoPokemonCheck_CheckedChanged(object sender, EventArgs e)
        {
            bingoUseStrongPokemon.Enabled = randomizeBattleBingoPokemonCheck.Checked;
        }
        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var release = SelfUpdater.CheckForUpdate(ToolName, version.Major, version.Minor, version.Build);
            if (release != null)
            {
                if (MessageBox.Show("There's a new update available! Would you like to update now?", "Hey you!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var updateForm = new Updater(release);
                    updateForm.ShowDialog();
                    Close();
                }
            }
        }
    }
}
