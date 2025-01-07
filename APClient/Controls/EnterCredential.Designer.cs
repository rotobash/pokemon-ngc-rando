namespace Randomizer.AP
{
    partial class EnterCredential
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
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            urlTextBox = new System.Windows.Forms.TextBox();
            slotNameTextBox = new System.Windows.Forms.TextBox();
            passwordTextBox = new System.Windows.Forms.TextBox();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.Controls.Add(label1, 1, 0);
            tableLayoutPanel1.Controls.Add(label2, 1, 1);
            tableLayoutPanel1.Controls.Add(label3, 1, 2);
            tableLayoutPanel1.Controls.Add(okButton, 1, 3);
            tableLayoutPanel1.Controls.Add(cancelButton, 2, 3);
            tableLayoutPanel1.Controls.Add(urlTextBox, 2, 0);
            tableLayoutPanel1.Controls.Add(slotNameTextBox, 2, 1);
            tableLayoutPanel1.Controls.Add(passwordTextBox, 2, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.Size = new System.Drawing.Size(558, 174);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = System.Windows.Forms.DockStyle.Fill;
            label1.Location = new System.Drawing.Point(66, 10);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(209, 38);
            label1.TabIndex = 0;
            label1.Text = "URL:";
            label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = System.Windows.Forms.DockStyle.Fill;
            label2.Location = new System.Drawing.Point(66, 48);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(209, 38);
            label2.TabIndex = 1;
            label2.Text = "Slot Name:";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = System.Windows.Forms.DockStyle.Fill;
            label3.Location = new System.Drawing.Point(66, 86);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(209, 38);
            label3.TabIndex = 2;
            label3.Text = "Password (optional):";
            label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // okButton
            // 
            okButton.Dock = System.Windows.Forms.DockStyle.Fill;
            okButton.Location = new System.Drawing.Point(66, 127);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(209, 34);
            okButton.TabIndex = 3;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Dock = System.Windows.Forms.DockStyle.Fill;
            cancelButton.Location = new System.Drawing.Point(281, 127);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(209, 34);
            cancelButton.TabIndex = 4;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // urlTextBox
            // 
            urlTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            urlTextBox.Location = new System.Drawing.Point(281, 13);
            urlTextBox.Name = "urlTextBox";
            urlTextBox.Size = new System.Drawing.Size(209, 23);
            urlTextBox.TabIndex = 5;
            urlTextBox.Text = "localhost:33838";
            urlTextBox.TextChanged += urlTextBox_TextChanged;
            // 
            // slotNameTextBox
            // 
            slotNameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            slotNameTextBox.Location = new System.Drawing.Point(281, 51);
            slotNameTextBox.Name = "slotNameTextBox";
            slotNameTextBox.Size = new System.Drawing.Size(209, 23);
            slotNameTextBox.TabIndex = 6;
            slotNameTextBox.TextChanged += slotNameTextBox_TextChanged;
            // 
            // passwordTextBox
            // 
            passwordTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            passwordTextBox.Location = new System.Drawing.Point(281, 89);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.Size = new System.Drawing.Size(209, 23);
            passwordTextBox.TabIndex = 7;
            passwordTextBox.TextChanged += passwordTextBox_TextChanged;
            // 
            // EnterCredential
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(558, 174);
            Controls.Add(tableLayoutPanel1);
            Name = "EnterCredential";
            Text = "Enter AP Credential";
            Load += EnterCredential_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.TextBox slotNameTextBox;
        private System.Windows.Forms.TextBox passwordTextBox;
    }
}