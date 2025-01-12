using Archipelago.MultiClient.Net;
using Randomizer.AP;
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
using XDCommon;
using System.Runtime.InteropServices;
using Reloaded.Memory;
using Reloaded.Memory.Pointers;
using Reloaded.Memory.Streams;
using Reloaded.Memory.Interfaces;
using Archipelago.MultiClient.Net.Enums;
using DolphinMemoryAccess;
using Archipelago.MultiClient.Net.Models;
using Color = System.Drawing.Color;
using ArchipelagoClient.Controls;
using XDCommon.PokemonDefinitions.XD.SaveData;
using XDCommon.Utility;
using XDCommon.Contracts;
using XDCommon.Shufflers;
using APCommon.Memory;

namespace ArchipelagoClient
{
    public partial class ArchipelagoClientForm : Form
    {
        bool connectedToAP = false;
        ArchipelagoSession apSession = null;
        APDataStructure memoryData = new APDataStructure();
        APLocationReferences locationReferences = new APLocationReferences();
        XDState xdProcess;

        BackgroundWorker dolphinSetupWorker;
        GameManipulator gameManipulator;
        ExtractedGame extractedGame;

        public ArchipelagoClientForm()
        {
            InitializeComponent();
            dolphinSetupWorker = new BackgroundWorker();
            dolphinSetupWorker.DoWork += SetupDolphinBackgroundWorker;
            dolphinSetupWorker.RunWorkerCompleted += OnDolphinSetup;
        }

        private void APClient_Load(object sender, EventArgs e)
        {
        }

        private void FindDolphinExe()
        {
            openDolphinDialog.Filter = "Dolphin Emulator|Dolphin.exe";
            openDolphinDialog.Title = "Select Dolphin Emulator";
            if (openDolphinDialog.ShowDialog() == DialogResult.OK)
            {
                Configuration.DolphinDirectory = openDolphinDialog.FileName ?? string.Empty;
            }
        }

        private void FindGameFile()
        {
            openGameFileDialog.Filter = "Find GameFile|*.iso";
            openGameFileDialog.Title = "Select ISO";
            if (openGameFileDialog.ShowDialog() == DialogResult.OK)
            {
                Configuration.GameFilePath = openGameFileDialog.FileName ?? string.Empty;
            }
        }

        private void GetCredentials()
        {
            var credentialForm = new EnterCredential();
            if (credentialForm.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private bool CreateAPSession()
        {
            try
            {
                apSession = ArchipelagoSessionFactory.CreateSession(Configuration.APUrl ?? string.Empty);
                var loginResult = apSession.TryConnectAndLogin("Pokemon XD: Gale Of Darkness", Configuration.APSlotname ?? string.Empty, ItemsHandlingFlags.AllItems, password: Configuration.APPassword);
                return loginResult.Successful;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetBaseException().Message);
                return false;
            }
        }

        private string CreateAPIso()
        {
            var gameManipInvoke = BeginInvoke(() => gameManipulator);
            var gameManip = EndInvoke(gameManipInvoke) as GameManipulator;
            var newFileName = $"{Path.GetRandomFileName()}.iso";
            // TODO: Randomize the game based on the AP server settings
            gameManip.ISO.DOL = new DOL("ASM");

            foreach (var move in extractedGame.ValidMoves)
            {
                move.AnimationID = 0;
            }

            gameManip.ISOExtractor.RepackISO(gameManip.ISO, newFileName);

            gameManip.Dispose();
            Invoke(() =>
            {
                gameManipulator = new GameManipulator(newFileName);
                extractedGame = new ExtractedGame(gameManipulator.GameExtractor);
                xdProcess = new XDState(gameManipulator.ISO);
                gameManipulator.ReleaseGameFile();
            });
            return newFileName;
        }

        private void SetupDolphinBackgroundWorker(object sender, EventArgs e)
        {
            var shouldRandomize = true;

            var fileToRun = Configuration.GameFilePath; // use AP settings for this instead
            if (shouldRandomize)
            {
                fileToRun = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}{CreateAPIso()}";
            }

            var xdStateInvoke = BeginInvoke(() => xdProcess);
            var xdState = EndInvoke(xdStateInvoke) as XDState;
            xdState.StartNewInstance(Configuration.DolphinDirectory, fileToRun).GetAwaiter().GetResult();
        }

        private void OnDolphinSetup(object sender, EventArgs e)
        {
            if (xdProcess?.Dolphin?.IsRunning == true)
            {
                dolphinPollTimer.Start();
                connectionStatusLabel.Text = $"Connected to Dolphin (Running {(Game)xdProcess.Dolphin.GameTitleCode})";
                connectionStatusLabel.ForeColor = connectedToAP ? Color.Green : Color.Orange;
            }
            else
            {
                MessageBox.Show("Couldn't start Dolphin");
            }
        }


        private void connectionStatusLabel_Click(object sender, EventArgs e)
        {
            if (xdProcess?.Dolphin?.IsRunning != true)
            {
                if (string.IsNullOrEmpty(Configuration.GameFilePath) || !File.Exists(Configuration.GameFilePath))
                {
                    FindGameFile();
                }

                if (string.IsNullOrEmpty(Configuration.DolphinDirectory) || !File.Exists(Configuration.DolphinDirectory))
                {
                    FindDolphinExe();
                }

                try
                {
                    gameManipulator = new GameManipulator(Configuration.GameFilePath);
                    extractedGame = new ExtractedGame(gameManipulator.GameExtractor);

                    dolphinSetupWorker.RunWorkerAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not open game file. Is it in use?");
                }
            }

            if (!connectedToAP)
            {
                if (string.IsNullOrEmpty(Configuration.APUrl) || string.IsNullOrEmpty(Configuration.APSlotname))
                {
                    GetCredentials();
                }

                if (CreateAPSession())
                {
                    connectedToAP = true;
                    connectionStatusLabel.Text = "Connected to AP";
                    connectionStatusLabel.ForeColor = xdProcess?.Dolphin?.IsRunning == true ? Color.Green : Color.Orange;
                }
            }
        }

        private void dolphinPollTimer_Tick(object sender, EventArgs e)
        {
            var dolphinState = xdProcess.Update();
            DisplayDolphinHealth(dolphinState);

            if (dolphinState != DolphinState.Hooked)
            {
                return;
            }

            var flags = xdProcess.FlagData.StoryProgress();
            storyFlagLabel.Text = flags.ToString();
            inBattleLabel.Text = xdProcess.InBattle ? "Yes" : "No";
            currentRoomLabel.Text = $"{xdProcess.CurrentRoom.Name} - {xdProcess.CurrentRoom.RoomId}".ToString();

            memoryData.ReadFromBytes(xdProcess.Dolphin.ReadData(APDataStructure.APMemoryAddress, APDataStructure.SizeOfAPData));

            var idsToCheck = new List<long>();
            var checkedItems = memoryData.GetCheckedItems();

            if (checkedItems.CheckedNewItems)
            {
                var newCheckedItems = checkedItems.GetNewItemChecks();
                foreach (var item in newCheckedItems)
                {
                    var id = locationReferences.LookupItem(item);
                    if (id > 0)
                    {
                        idsToCheck.Add(id);
                    }
                }
            }

            if (checkedItems.CheckedNewPokemon)
            {
                var newCheckedItems = checkedItems.GetNewPokemonChecks();
                foreach (var item in newCheckedItems)
                {
                    var id = locationReferences.LookupPokemon(item);
                    if (id > 0)
                    {
                        idsToCheck.Add(id);
                    }
                }
            }

            if (checkedItems.CheckedNewBattles)
            {
                var newCheckedItems = checkedItems.GetNewBattleChecks();
                foreach (var item in newCheckedItems)
                {
                    var id = locationReferences.LookupTrainer(item);
                    if (id > 0)
                    {
                        idsToCheck.Add(id);
                    }
                }
            }

            if (checkedItems.CheckedNewPurifications)
            {
                var newCheckedItems = checkedItems.GetNewShadowPurifyChecks();
                foreach (var item in newCheckedItems)
                {
                    var id = locationReferences.LookupPurify(item);
                    if (id > 0)
                    {
                        idsToCheck.Add(id);
                    }
                }
            }

            if (idsToCheck.Count > 0)
            {
                foreach (var id in idsToCheck)
                {
                    var name = locationReferences.nameToId[(int)id];
                    listBox1.Items.Add($"Checked {name}");
                }
                //apSession.Locations.CompleteLocationChecks(idsToCheck.ToArray());
            }
        }

        private void DisplayDolphinHealth(DolphinState dolphinState)
        {
            switch (dolphinState)
            {
                case DolphinState.Hooked:
                    connectionStatusLabel.Text = "Hooked!";
                    connectionStatusLabel.ForeColor = Color.Green;
                    return;
                case DolphinState.NotRunning:
                    connectionStatusLabel.Text = "Dolphin not running";
                    connectionStatusLabel.ForeColor = Color.Orange;
                    return;
                case DolphinState.ProcessRunning:
                    connectionStatusLabel.Text = "Waiting for game to boot";
                    connectionStatusLabel.ForeColor = Color.Orange;
                    return;
                case DolphinState.GameBooted:
                    connectionStatusLabel.Text = "Waiting for save game to load";
                    connectionStatusLabel.ForeColor = Color.Orange;
                    return;
                default:
                    connectionStatusLabel.Text = "Disconnected from Dolphin";
                    connectionStatusLabel.ForeColor = Color.Orange;
                    dolphinPollTimer.Stop();
                    return;
            }
        }

        private void APClient_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (connectedToAP)
            {
                apSession.SetClientState(ArchipelagoClientState.ClientUnknown);
            }
            xdProcess?.Dolphin?.CloseDolphinProcess();
        }
    }
}
