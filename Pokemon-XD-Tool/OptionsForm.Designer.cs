﻿
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.threadCountSelector = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.extractDirLabel = new System.Windows.Forms.Label();
            this.browseForDirButton = new System.Windows.Forms.Button();
            this.memoryStreamCheck = new System.Windows.Forms.CheckBox();
            this.verboseLogCheck = new System.Windows.Forms.CheckBox();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.threadCountSelector)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.threadCountSelector, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.memoryStreamCheck, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.verboseLogCheck, 1, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(319, 359);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Extract Directory:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Thread Count:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 177);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 30);
            this.label4.TabIndex = 2;
            this.label4.Text = "Use Memory Streams";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 248);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 15);
            this.label5.TabIndex = 3;
            this.label5.Text = "Verbose Logging";
            // 
            // threadCountSelector
            // 
            this.threadCountSelector.Location = new System.Drawing.Point(114, 109);
            this.threadCountSelector.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.threadCountSelector.Name = "threadCountSelector";
            this.threadCountSelector.Size = new System.Drawing.Size(202, 23);
            this.threadCountSelector.TabIndex = 5;
            this.threadCountSelector.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.threadCountSelector.ValueChanged += new System.EventHandler(this.threadCountSelector_ValueChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.extractDirLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.browseForDirButton, 0, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(114, 38);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(200, 65);
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
            this.browseForDirButton.Location = new System.Drawing.Point(3, 35);
            this.browseForDirButton.Name = "browseForDirButton";
            this.browseForDirButton.Size = new System.Drawing.Size(194, 27);
            this.browseForDirButton.TabIndex = 1;
            this.browseForDirButton.Text = "Browse";
            this.browseForDirButton.UseVisualStyleBackColor = true;
            this.browseForDirButton.Click += new System.EventHandler(this.browseForDirButton_Click);
            // 
            // memoryStreamCheck
            // 
            this.memoryStreamCheck.AutoSize = true;
            this.memoryStreamCheck.Location = new System.Drawing.Point(114, 180);
            this.memoryStreamCheck.Name = "memoryStreamCheck";
            this.memoryStreamCheck.Size = new System.Drawing.Size(42, 19);
            this.memoryStreamCheck.TabIndex = 7;
            this.memoryStreamCheck.Text = "On";
            this.memoryStreamCheck.UseVisualStyleBackColor = true;
            this.memoryStreamCheck.CheckedChanged += new System.EventHandler(this.memoryStreamCheck_CheckedChanged);
            // 
            // verboseLogCheck
            // 
            this.verboseLogCheck.AutoSize = true;
            this.verboseLogCheck.Location = new System.Drawing.Point(114, 251);
            this.verboseLogCheck.Name = "verboseLogCheck";
            this.verboseLogCheck.Size = new System.Drawing.Size(42, 19);
            this.verboseLogCheck.TabIndex = 8;
            this.verboseLogCheck.Text = "On";
            this.verboseLogCheck.UseVisualStyleBackColor = true;
            this.verboseLogCheck.CheckedChanged += new System.EventHandler(this.verboseLogCheck_CheckedChanged);
            // 
            // OptionsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 359);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "OptionsForm";
            this.Text = "OptionsForm";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.threadCountSelector)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown threadCountSelector;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label extractDirLabel;
        private System.Windows.Forms.Button browseForDirButton;
        private System.Windows.Forms.CheckBox memoryStreamCheck;
        private System.Windows.Forms.CheckBox verboseLogCheck;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}