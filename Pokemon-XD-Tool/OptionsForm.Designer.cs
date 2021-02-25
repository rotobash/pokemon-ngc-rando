
namespace Randomizer
{
    partial class OptionsForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.verboseLogCheck = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.fileStreamCheck = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.extractDirLabel = new System.Windows.Forms.Label();
            this.browseForDirButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.pokemonBSTSelector = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.movePower = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.infoToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pokemonBSTSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.movePower)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.Controls.Add(this.verboseLogCheck, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.label5, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label4, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.fileStreamCheck, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.pokemonBSTSelector, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.label6, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.movePower, 2, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(319, 393);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // verboseLogCheck
            // 
            this.verboseLogCheck.AutoSize = true;
            this.verboseLogCheck.Location = new System.Drawing.Point(129, 290);
            this.verboseLogCheck.Name = "verboseLogCheck";
            this.verboseLogCheck.Size = new System.Drawing.Size(42, 19);
            this.verboseLogCheck.TabIndex = 8;
            this.verboseLogCheck.Text = "On";
            this.verboseLogCheck.UseVisualStyleBackColor = true;
            this.verboseLogCheck.Visible = false;
            this.verboseLogCheck.CheckedChanged += new System.EventHandler(this.verboseLogCheck_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 287);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 15);
            this.label5.TabIndex = 3;
            this.label5.Text = "Log To File:";
            this.label5.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "Use File Streams";
            // 
            // fileStreamCheck
            // 
            this.fileStreamCheck.AutoSize = true;
            this.fileStreamCheck.Location = new System.Drawing.Point(129, 42);
            this.fileStreamCheck.Name = "fileStreamCheck";
            this.fileStreamCheck.Size = new System.Drawing.Size(42, 19);
            this.fileStreamCheck.TabIndex = 7;
            this.fileStreamCheck.Text = "On";
            this.infoToolTip.SetToolTip(this.fileStreamCheck, resources.GetString("fileStreamCheck.ToolTip"));
            this.fileStreamCheck.UseVisualStyleBackColor = true;
            this.fileStreamCheck.CheckedChanged += new System.EventHandler(this.memoryStreamCheck_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Extract Directory:";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.extractDirLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.browseForDirButton, 0, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(129, 104);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(169, 56);
            this.tableLayoutPanel2.TabIndex = 6;
            // 
            // extractDirLabel
            // 
            this.extractDirLabel.AutoSize = true;
            this.extractDirLabel.Location = new System.Drawing.Point(3, 0);
            this.extractDirLabel.Name = "extractDirLabel";
            this.extractDirLabel.Size = new System.Drawing.Size(0, 15);
            this.extractDirLabel.TabIndex = 0;
            // 
            // browseForDirButton
            // 
            this.browseForDirButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browseForDirButton.Enabled = false;
            this.browseForDirButton.Location = new System.Drawing.Point(3, 31);
            this.browseForDirButton.Name = "browseForDirButton";
            this.browseForDirButton.Size = new System.Drawing.Size(163, 22);
            this.browseForDirButton.TabIndex = 1;
            this.browseForDirButton.Text = "Browse";
            this.browseForDirButton.UseVisualStyleBackColor = true;
            this.browseForDirButton.Click += new System.EventHandler(this.browseForDirButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 225);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 30);
            this.label3.TabIndex = 1;
            this.label3.Text = "Good Pokemon BST:";
            // 
            // pokemonBSTSelector
            // 
            this.pokemonBSTSelector.Location = new System.Drawing.Point(129, 228);
            this.pokemonBSTSelector.Maximum = new decimal(new int[] {
            680,
            0,
            0,
            0});
            this.pokemonBSTSelector.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.pokemonBSTSelector.Name = "pokemonBSTSelector";
            this.pokemonBSTSelector.Size = new System.Drawing.Size(169, 23);
            this.pokemonBSTSelector.TabIndex = 5;
            this.pokemonBSTSelector.Value = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.pokemonBSTSelector.ValueChanged += new System.EventHandler(this.threadCountSelector_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 163);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 30);
            this.label6.TabIndex = 9;
            this.label6.Text = "Good Damging Move Power:";
            // 
            // movePower
            // 
            this.movePower.Location = new System.Drawing.Point(129, 166);
            this.movePower.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.movePower.Name = "movePower";
            this.movePower.Size = new System.Drawing.Size(169, 23);
            this.movePower.TabIndex = 10;
            this.movePower.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.movePower.ValueChanged += new System.EventHandler(this.movePower_ValueChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 393);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "OptionsForm";
            this.Text = "Options";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pokemonBSTSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.movePower)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown pokemonBSTSelector;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label extractDirLabel;
        private System.Windows.Forms.Button browseForDirButton;
        private System.Windows.Forms.CheckBox fileStreamCheck;
        private System.Windows.Forms.CheckBox verboseLogCheck;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown movePower;
        private System.Windows.Forms.ToolTip infoToolTip;
    }
}