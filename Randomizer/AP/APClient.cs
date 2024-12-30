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
using System.Buffers.Binary;
using XDCommon.PokemonDefinitions.XD.SaveData;
using XDCommon.Utility;

namespace Randomizer
{
    public partial class APClient : Form
    {
        private const long XDSaveDataRAMPointerAddress = 0x80444d5c;

        bool connectedToAP = false;
        ArchipelagoSession apSession = null;
        Dolphin dolphinProcess;
        long baseSaveDataPointer;

        BackgroundWorker dolphinSetupWorker;
        SaveDataLayout saveData;
        PokemonDisplay[] pokemonDisplays = new PokemonDisplay[6];
        GameManipulator gameManipulator;
        ExtractedGame extractedGame;

        public APClient()
        {
            InitializeComponent();
            dolphinSetupWorker = new BackgroundWorker();
            dolphinSetupWorker.DoWork += SetupDolphinBackgroundWorker;
            dolphinSetupWorker.RunWorkerCompleted += OnDolphinSetup;
        }

        private void APClient_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 6; i++)
            {
                var pokemonDisplay = new PokemonDisplay 
                {
                    AutoSize = true,
                    Dock = DockStyle.Fill
                };

                partyTabPage.TabPages[i].Controls.Add(pokemonDisplay);
                pokemonDisplays[i] = pokemonDisplay;
            }
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

        private void SetupDolphinBackgroundWorker(object sender, EventArgs e)
        {
            var dolphinProcInvoke = BeginInvoke(() => dolphinProcess);
            var dolphinProc = EndInvoke(dolphinProcInvoke) as Dolphin;

            var started = dolphinProc.SetupDolphin().GetAwaiter().GetResult();
        }

        private void OnDolphinSetup(object sender, EventArgs e)
        {
            if (dolphinProcess?.IsRunning == true)
            {
                dolphinPollTimer.Start();
                connectionStatusLabel.Text = $"Connected to Dolphin (Running {dolphinProcess.RunningGameCode})";
                connectionStatusLabel.ForeColor = connectedToAP ? Color.Green : Color.Yellow;
            }
            else
            {
                MessageBox.Show("Couldn't start Dolphin");
            }
        }


        private void connectionStatusLabel_Click(object sender, EventArgs e)
        {
            if (dolphinProcess?.IsRunning != true)
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
                    gameManipulator.ReleaseGameFile();

                    dolphinProcess = new Dolphin(Configuration.DolphinDirectory, Configuration.GameFilePath);
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
                    //connectionStatusLabel.ForeColor = dolphinProcess?.IsRunning == true ? Color.Green : Color.Yellow;
                }
            }
        }

        private bool LoadBaseAddress()
        {
            var buf = dolphinProcess.ReadData(XDSaveDataRAMPointerAddress, 4);
            baseSaveDataPointer = BinaryPrimitives.ReadUInt32BigEndian(buf);

            return baseSaveDataPointer != 0;
        }

        private void dolphinPollTimer_Tick(object sender, EventArgs e)
        {
            if (CheckDolphinHealth())
            {
                saveData = SaveDataLayout.LoadFromMemory(dolphinProcess, baseSaveDataPointer);
                for (int i = 0; i < 6; i++)
                {
                    if (saveData.Party[i].Species < extractedGame.PokemonList.Length)
                    {
                        pokemonDisplays[i].UpdatePokemon(saveData.Party[i], extractedGame.PokemonList[saveData.Party[i].Species], extractedGame.MoveList);
                    }
                }

                var newDisplay = new List<string>
                {
                    "----------------- Items -----------------"
                };
                foreach (var item in saveData.Inventory)
                {
                    if (item.Index < extractedGame.ItemList.Length)
                    {
                        newDisplay.Add($"{extractedGame.ItemList[item.Index].Name} x{item.Quantity}");
                    }
                }

                newDisplay.Add("----------------- Key Items -----------------");
                foreach (var item in saveData.KeyItemInventory)
                {
                    var adjustedIndex = item.Index - 150;
                    if (adjustedIndex < extractedGame.ItemList.Length)
                    {
                        newDisplay.Add($"{extractedGame.ItemList[adjustedIndex].Name} x{item.Quantity}");
                    }
                }

                newDisplay.Add("----------------- TM Items -----------------");
                foreach (var item in saveData.TMItemInventory)
                {
                    if (item.Index < extractedGame.ItemList.Length)
                    {
                        newDisplay.Add($"{extractedGame.ItemList[item.Index].Name} x{item.Quantity}");
                    }
                }

                if (!newDisplay.SequenceEqual(inventoryListBox.Items.Cast<string>()))
                {
                    inventoryListBox.Items.Clear();
                    inventoryListBox.Items.AddRange(newDisplay.Cast<object>().ToArray());
                }
            }
        }

        private bool CheckDolphinHealth()
        {
            if (dolphinProcess?.IsRunning != true)
            {
                baseSaveDataPointer = 0;
                connectionStatusLabel.Text = "Disconnected from Dolphin";
                connectionStatusLabel.ForeColor = Color.Yellow;
                dolphinPollTimer.Stop();
                return false;
            }
            else
            {
                if (baseSaveDataPointer == 0 && !LoadBaseAddress())
                {
                    connectionStatusLabel.Text = "Waiting for game to boot";
                    return false;
                }

                var saveDataStart = dolphinProcess.ReadData(baseSaveDataPointer + 1, 15);
                if (saveDataStart.All(b => b == 0))
                {
                    connectionStatusLabel.Text = "Waiting for save game to load";
                    return false;
                }
                else
                {
                    connectionStatusLabel.Text = "Listening...";
                }
            }

            return true;
        }

        private void APClient_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (connectedToAP)
            {
                apSession.SetClientState(ArchipelagoClientState.ClientUnknown);
            }
            dolphinProcess?.CloseDolphinProcess();
        }
    }
}
