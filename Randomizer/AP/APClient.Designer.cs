namespace Randomizer
{
    partial class APClient
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            listBox1 = new System.Windows.Forms.ListBox();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            tabControl1 = new System.Windows.Forms.TabControl();
            partyPage = new System.Windows.Forms.TabPage();
            partyTabPage = new System.Windows.Forms.TabControl();
            partyPokemon1TabPage = new System.Windows.Forms.TabPage();
            partyPokemon2TabPage = new System.Windows.Forms.TabPage();
            partyPokemon3TabPage = new System.Windows.Forms.TabPage();
            partyPokemon4TabPage = new System.Windows.Forms.TabPage();
            partyPokemon5TabPage = new System.Windows.Forms.TabPage();
            partyPokemon6TabPage = new System.Windows.Forms.TabPage();
            pcPage = new System.Windows.Forms.TabPage();
            inventoryPage = new System.Windows.Forms.TabPage();
            inventoryListBox = new System.Windows.Forms.ListBox();
            pcInventoryPage = new System.Windows.Forms.TabPage();
            storyFlags = new System.Windows.Forms.TabPage();
            statusStrip1 = new System.Windows.Forms.StatusStrip();
            connectionStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            dolphinPollTimer = new System.Windows.Forms.Timer(components);
            openGameFileDialog = new System.Windows.Forms.OpenFileDialog();
            openDolphinDialog = new System.Windows.Forms.OpenFileDialog();
            tableLayoutPanel1.SuspendLayout();
            tabControl1.SuspendLayout();
            partyPage.SuspendLayout();
            partyTabPage.SuspendLayout();
            inventoryPage.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // listBox1
            // 
            tableLayoutPanel1.SetColumnSpan(listBox1, 2);
            listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new System.Drawing.Point(13, 293);
            listBox1.Name = "listBox1";
            listBox1.Size = new System.Drawing.Size(735, 223);
            listBox1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(listBox1, 0, 2);
            tableLayoutPanel1.Controls.Add(tabControl1, 0, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(761, 550);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // tabControl1
            // 
            tableLayoutPanel1.SetColumnSpan(tabControl1, 2);
            tabControl1.Controls.Add(partyPage);
            tabControl1.Controls.Add(pcPage);
            tabControl1.Controls.Add(inventoryPage);
            tabControl1.Controls.Add(pcInventoryPage);
            tabControl1.Controls.Add(storyFlags);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(13, 64);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(735, 223);
            tabControl1.TabIndex = 1;
            // 
            // partyPage
            // 
            partyPage.Controls.Add(partyTabPage);
            partyPage.Location = new System.Drawing.Point(4, 24);
            partyPage.Name = "partyPage";
            partyPage.Padding = new System.Windows.Forms.Padding(3);
            partyPage.Size = new System.Drawing.Size(727, 195);
            partyPage.TabIndex = 0;
            partyPage.Text = "Party";
            partyPage.UseVisualStyleBackColor = true;
            // 
            // partyTabPage
            // 
            partyTabPage.Controls.Add(partyPokemon1TabPage);
            partyTabPage.Controls.Add(partyPokemon2TabPage);
            partyTabPage.Controls.Add(partyPokemon3TabPage);
            partyTabPage.Controls.Add(partyPokemon4TabPage);
            partyTabPage.Controls.Add(partyPokemon5TabPage);
            partyTabPage.Controls.Add(partyPokemon6TabPage);
            partyTabPage.Dock = System.Windows.Forms.DockStyle.Fill;
            partyTabPage.Location = new System.Drawing.Point(3, 3);
            partyTabPage.Name = "partyTabPage";
            partyTabPage.SelectedIndex = 0;
            partyTabPage.Size = new System.Drawing.Size(721, 189);
            partyTabPage.TabIndex = 0;
            // 
            // partyPokemon1TabPage
            // 
            partyPokemon1TabPage.Location = new System.Drawing.Point(4, 24);
            partyPokemon1TabPage.Name = "partyPokemon1TabPage";
            partyPokemon1TabPage.Padding = new System.Windows.Forms.Padding(3);
            partyPokemon1TabPage.Size = new System.Drawing.Size(713, 161);
            partyPokemon1TabPage.TabIndex = 0;
            partyPokemon1TabPage.Text = "Pokemon 1";
            partyPokemon1TabPage.UseVisualStyleBackColor = true;
            // 
            // partyPokemon2TabPage
            // 
            partyPokemon2TabPage.Location = new System.Drawing.Point(4, 24);
            partyPokemon2TabPage.Name = "partyPokemon2TabPage";
            partyPokemon2TabPage.Padding = new System.Windows.Forms.Padding(3);
            partyPokemon2TabPage.Size = new System.Drawing.Size(713, 161);
            partyPokemon2TabPage.TabIndex = 1;
            partyPokemon2TabPage.Text = "Pokemon 2";
            partyPokemon2TabPage.UseVisualStyleBackColor = true;
            // 
            // partyPokemon3TabPage
            // 
            partyPokemon3TabPage.Location = new System.Drawing.Point(4, 24);
            partyPokemon3TabPage.Name = "partyPokemon3TabPage";
            partyPokemon3TabPage.Size = new System.Drawing.Size(713, 161);
            partyPokemon3TabPage.TabIndex = 2;
            partyPokemon3TabPage.Text = "Pokemon 3";
            partyPokemon3TabPage.UseVisualStyleBackColor = true;
            // 
            // partyPokemon4TabPage
            // 
            partyPokemon4TabPage.Location = new System.Drawing.Point(4, 24);
            partyPokemon4TabPage.Name = "partyPokemon4TabPage";
            partyPokemon4TabPage.Size = new System.Drawing.Size(713, 161);
            partyPokemon4TabPage.TabIndex = 3;
            partyPokemon4TabPage.Text = "Pokemon 4";
            partyPokemon4TabPage.UseVisualStyleBackColor = true;
            // 
            // partyPokemon5TabPage
            // 
            partyPokemon5TabPage.Location = new System.Drawing.Point(4, 24);
            partyPokemon5TabPage.Name = "partyPokemon5TabPage";
            partyPokemon5TabPage.Size = new System.Drawing.Size(713, 161);
            partyPokemon5TabPage.TabIndex = 4;
            partyPokemon5TabPage.Text = "Pokemon 5";
            partyPokemon5TabPage.UseVisualStyleBackColor = true;
            // 
            // partyPokemon6TabPage
            // 
            partyPokemon6TabPage.Location = new System.Drawing.Point(4, 24);
            partyPokemon6TabPage.Name = "partyPokemon6TabPage";
            partyPokemon6TabPage.Size = new System.Drawing.Size(713, 161);
            partyPokemon6TabPage.TabIndex = 5;
            partyPokemon6TabPage.Text = "Pokemon 6";
            partyPokemon6TabPage.UseVisualStyleBackColor = true;
            // 
            // pcPage
            // 
            pcPage.Location = new System.Drawing.Point(4, 24);
            pcPage.Name = "pcPage";
            pcPage.Padding = new System.Windows.Forms.Padding(3);
            pcPage.Size = new System.Drawing.Size(727, 195);
            pcPage.TabIndex = 1;
            pcPage.Text = "PC";
            pcPage.UseVisualStyleBackColor = true;
            // 
            // inventoryPage
            // 
            inventoryPage.Controls.Add(inventoryListBox);
            inventoryPage.Location = new System.Drawing.Point(4, 24);
            inventoryPage.Name = "inventoryPage";
            inventoryPage.Size = new System.Drawing.Size(727, 195);
            inventoryPage.TabIndex = 2;
            inventoryPage.Text = "Inventory";
            inventoryPage.UseVisualStyleBackColor = true;
            // 
            // inventoryListBox
            // 
            inventoryListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            inventoryListBox.FormattingEnabled = true;
            inventoryListBox.ItemHeight = 15;
            inventoryListBox.Location = new System.Drawing.Point(0, 0);
            inventoryListBox.Name = "inventoryListBox";
            inventoryListBox.Size = new System.Drawing.Size(727, 195);
            inventoryListBox.TabIndex = 0;
            // 
            // pcInventoryPage
            // 
            pcInventoryPage.Location = new System.Drawing.Point(4, 24);
            pcInventoryPage.Name = "pcInventoryPage";
            pcInventoryPage.Size = new System.Drawing.Size(727, 195);
            pcInventoryPage.TabIndex = 3;
            pcInventoryPage.Text = "PC Inventory";
            pcInventoryPage.UseVisualStyleBackColor = true;
            // 
            // storyFlags
            // 
            storyFlags.Location = new System.Drawing.Point(4, 24);
            storyFlags.Name = "storyFlags";
            storyFlags.Size = new System.Drawing.Size(727, 195);
            storyFlags.TabIndex = 4;
            storyFlags.Text = "Story";
            storyFlags.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { connectionStatusLabel });
            statusStrip1.Location = new System.Drawing.Point(0, 528);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new System.Drawing.Size(761, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // connectionStatusLabel
            // 
            connectionStatusLabel.ForeColor = System.Drawing.Color.Red;
            connectionStatusLabel.Name = "connectionStatusLabel";
            connectionStatusLabel.Size = new System.Drawing.Size(88, 17);
            connectionStatusLabel.Text = "Not Connected";
            connectionStatusLabel.Click += connectionStatusLabel_Click;
            // 
            // dolphinPollTimer
            // 
            dolphinPollTimer.Interval = 500;
            dolphinPollTimer.Tick += dolphinPollTimer_Tick;
            // 
            // openGameFileDialog
            // 
            openGameFileDialog.FileName = "*.iso";
            // 
            // openDolphinDialog
            // 
            openDolphinDialog.FileName = "openFileDialog1";
            // 
            // APClient
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(761, 550);
            Controls.Add(statusStrip1);
            Controls.Add(tableLayoutPanel1);
            Name = "APClient";
            Text = "APClient";
            FormClosed += APClient_FormClosed;
            Load += APClient_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            partyPage.ResumeLayout(false);
            partyTabPage.ResumeLayout(false);
            inventoryPage.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage partyPage;
        private System.Windows.Forms.TabPage pcPage;
        private System.Windows.Forms.TabPage inventoryPage;
        private System.Windows.Forms.TabPage pcInventoryPage;
        private System.Windows.Forms.TabPage storyFlags;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel connectionStatusLabel;
        private System.Windows.Forms.Timer dolphinPollTimer;
        private System.Windows.Forms.OpenFileDialog openGameFileDialog;
        private System.Windows.Forms.OpenFileDialog openDolphinDialog;
        private System.Windows.Forms.TabControl partyTabPage;
        private System.Windows.Forms.TabPage partyPokemon1TabPage;
        private System.Windows.Forms.TabPage partyPokemon2TabPage;
        private System.Windows.Forms.TabPage partyPokemon3TabPage;
        private System.Windows.Forms.TabPage partyPokemon4TabPage;
        private System.Windows.Forms.TabPage partyPokemon5TabPage;
        private System.Windows.Forms.TabPage partyPokemon6TabPage;
        private System.Windows.Forms.ListBox inventoryListBox;
    }
}