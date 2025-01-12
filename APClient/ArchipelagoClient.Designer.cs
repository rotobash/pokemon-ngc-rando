namespace ArchipelagoClient
{
    partial class ArchipelagoClientForm
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
            listBox1 = new ListBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            label1 = new Label();
            storyFlagLabel = new Label();
            label2 = new Label();
            inBattleLabel = new Label();
            label3 = new Label();
            currentRoomLabel = new Label();
            statusStrip1 = new StatusStrip();
            connectionStatusLabel = new ToolStripStatusLabel();
            dolphinPollTimer = new System.Windows.Forms.Timer(components);
            openGameFileDialog = new OpenFileDialog();
            openDolphinDialog = new OpenFileDialog();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // listBox1
            // 
            tableLayoutPanel1.SetColumnSpan(listBox1, 2);
            listBox1.Dock = DockStyle.Fill;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(13, 64);
            listBox1.Name = "listBox1";
            tableLayoutPanel1.SetRowSpan(listBox1, 2);
            listBox1.Size = new Size(735, 452);
            listBox1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(listBox1, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(10);
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(761, 550);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(label1, 0, 0);
            tableLayoutPanel2.Controls.Add(storyFlagLabel, 1, 0);
            tableLayoutPanel2.Controls.Add(label2, 0, 1);
            tableLayoutPanel2.Controls.Add(inBattleLabel, 1, 1);
            tableLayoutPanel2.Controls.Add(label3, 0, 2);
            tableLayoutPanel2.Controls.Add(currentRoomLabel, 1, 2);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(13, 13);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 3;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel2.Size = new Size(364, 45);
            tableLayoutPanel2.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(60, 15);
            label1.TabIndex = 3;
            label1.Text = "Story flag:";
            // 
            // storyFlagLabel
            // 
            storyFlagLabel.AutoSize = true;
            storyFlagLabel.Location = new Point(185, 0);
            storyFlagLabel.Name = "storyFlagLabel";
            storyFlagLabel.Size = new Size(45, 15);
            storyFlagLabel.TabIndex = 2;
            storyFlagLabel.Text = "Not set";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 15);
            label2.Name = "label2";
            label2.Size = new Size(53, 15);
            label2.TabIndex = 4;
            label2.Text = "In Battle:";
            // 
            // inBattleLabel
            // 
            inBattleLabel.AutoSize = true;
            inBattleLabel.Location = new Point(185, 15);
            inBattleLabel.Name = "inBattleLabel";
            inBattleLabel.Size = new Size(23, 15);
            inBattleLabel.TabIndex = 5;
            inBattleLabel.Text = "No";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 30);
            label3.Name = "label3";
            label3.Size = new Size(85, 15);
            label3.TabIndex = 6;
            label3.Text = "Current Room:";
            // 
            // currentRoomLabel
            // 
            currentRoomLabel.AutoSize = true;
            currentRoomLabel.Location = new Point(185, 30);
            currentRoomLabel.Name = "currentRoomLabel";
            currentRoomLabel.Size = new Size(46, 15);
            currentRoomLabel.TabIndex = 7;
            currentRoomLabel.Text = "Not Set";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { connectionStatusLabel });
            statusStrip1.Location = new Point(0, 528);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(761, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // connectionStatusLabel
            // 
            connectionStatusLabel.ForeColor = Color.Red;
            connectionStatusLabel.Name = "connectionStatusLabel";
            connectionStatusLabel.Size = new Size(88, 17);
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
            // ArchipelagoClientForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(761, 550);
            Controls.Add(statusStrip1);
            Controls.Add(tableLayoutPanel1);
            Name = "ArchipelagoClientForm";
            Text = "APClient";
            FormClosed += APClient_FormClosed;
            Load += APClient_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel connectionStatusLabel;
        private System.Windows.Forms.Timer dolphinPollTimer;
        private System.Windows.Forms.OpenFileDialog openGameFileDialog;
        private System.Windows.Forms.OpenFileDialog openDolphinDialog;
        private System.Windows.Forms.Label storyFlagLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label inBattleLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label currentRoomLabel;
    }
}