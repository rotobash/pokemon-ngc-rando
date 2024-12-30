namespace Randomizer.AP
{
    partial class PokemonDisplay
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pokemonNameLabel = new System.Windows.Forms.Label();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            hpLabel = new System.Windows.Forms.Label();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            abilityLabel = new System.Windows.Forms.Label();
            moveLabel = new System.Windows.Forms.Label();
            speciesLabel = new System.Windows.Forms.Label();
            move1Label = new System.Windows.Forms.Label();
            move2Label = new System.Windows.Forms.Label();
            move3Label = new System.Windows.Forms.Label();
            move4Label = new System.Windows.Forms.Label();
            isShadowLabel = new System.Windows.Forms.Label();
            abilityDisplayLabel = new System.Windows.Forms.Label();
            levelLabel = new System.Windows.Forms.Label();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // pokemonNameLabel
            // 
            pokemonNameLabel.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(pokemonNameLabel, 2);
            pokemonNameLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            pokemonNameLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            pokemonNameLabel.Location = new System.Drawing.Point(3, 0);
            pokemonNameLabel.Name = "pokemonNameLabel";
            tableLayoutPanel1.SetRowSpan(pokemonNameLabel, 2);
            pokemonNameLabel.Size = new System.Drawing.Size(244, 56);
            pokemonNameLabel.TabIndex = 0;
            pokemonNameLabel.Text = "Pokemon name";
            pokemonNameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar1
            // 
            tableLayoutPanel1.SetColumnSpan(progressBar1, 2);
            progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            progressBar1.Location = new System.Drawing.Point(71, 81);
            progressBar1.Maximum = 255;
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(358, 27);
            progressBar1.TabIndex = 1;
            // 
            // hpLabel
            // 
            hpLabel.AutoSize = true;
            hpLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            hpLabel.Location = new System.Drawing.Point(3, 78);
            hpLabel.Name = "hpLabel";
            hpLabel.Size = new System.Drawing.Size(62, 33);
            hpLabel.TabIndex = 2;
            hpLabel.Text = "HP:";
            hpLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            tableLayoutPanel1.Controls.Add(abilityLabel, 1, 2);
            tableLayoutPanel1.Controls.Add(moveLabel, 0, 4);
            tableLayoutPanel1.Controls.Add(pokemonNameLabel, 0, 0);
            tableLayoutPanel1.Controls.Add(hpLabel, 0, 3);
            tableLayoutPanel1.Controls.Add(progressBar1, 1, 3);
            tableLayoutPanel1.Controls.Add(speciesLabel, 2, 0);
            tableLayoutPanel1.Controls.Add(move1Label, 1, 4);
            tableLayoutPanel1.Controls.Add(move2Label, 2, 4);
            tableLayoutPanel1.Controls.Add(move3Label, 1, 5);
            tableLayoutPanel1.Controls.Add(move4Label, 2, 5);
            tableLayoutPanel1.Controls.Add(isShadowLabel, 2, 1);
            tableLayoutPanel1.Controls.Add(abilityDisplayLabel, 0, 2);
            tableLayoutPanel1.Controls.Add(levelLabel, 2, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.4285717F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.1428566F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2857141F));
            tableLayoutPanel1.Size = new System.Drawing.Size(456, 197);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // abilityLabel
            // 
            abilityLabel.AutoSize = true;
            abilityLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            abilityLabel.Location = new System.Drawing.Point(71, 56);
            abilityLabel.Name = "abilityLabel";
            abilityLabel.Size = new System.Drawing.Size(176, 22);
            abilityLabel.TabIndex = 12;
            abilityLabel.Text = "Ability";
            abilityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // moveLabel
            // 
            moveLabel.AutoSize = true;
            moveLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            moveLabel.Location = new System.Drawing.Point(3, 111);
            moveLabel.Name = "moveLabel";
            moveLabel.Size = new System.Drawing.Size(62, 28);
            moveLabel.TabIndex = 4;
            moveLabel.Text = "Moves:";
            moveLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // speciesLabel
            // 
            speciesLabel.AutoSize = true;
            speciesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            speciesLabel.Location = new System.Drawing.Point(253, 0);
            speciesLabel.Name = "speciesLabel";
            speciesLabel.Size = new System.Drawing.Size(176, 28);
            speciesLabel.TabIndex = 3;
            speciesLabel.Text = "Species";
            speciesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // move1Label
            // 
            move1Label.AutoSize = true;
            move1Label.Dock = System.Windows.Forms.DockStyle.Fill;
            move1Label.Location = new System.Drawing.Point(71, 111);
            move1Label.Name = "move1Label";
            move1Label.Size = new System.Drawing.Size(176, 28);
            move1Label.TabIndex = 6;
            move1Label.Text = "{Move} {pp}/{tot pp}";
            move1Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // move2Label
            // 
            move2Label.AutoSize = true;
            move2Label.Dock = System.Windows.Forms.DockStyle.Fill;
            move2Label.Location = new System.Drawing.Point(253, 111);
            move2Label.Name = "move2Label";
            move2Label.Size = new System.Drawing.Size(176, 28);
            move2Label.TabIndex = 7;
            move2Label.Text = "{Move} {pp}/{tot pp}";
            move2Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // move3Label
            // 
            move3Label.AutoSize = true;
            move3Label.Dock = System.Windows.Forms.DockStyle.Fill;
            move3Label.Location = new System.Drawing.Point(71, 139);
            move3Label.Name = "move3Label";
            move3Label.Size = new System.Drawing.Size(176, 28);
            move3Label.TabIndex = 8;
            move3Label.Text = "{Move} {pp}/{tot pp}";
            move3Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // move4Label
            // 
            move4Label.AutoSize = true;
            move4Label.Dock = System.Windows.Forms.DockStyle.Fill;
            move4Label.Location = new System.Drawing.Point(253, 139);
            move4Label.Name = "move4Label";
            move4Label.Size = new System.Drawing.Size(176, 28);
            move4Label.TabIndex = 9;
            move4Label.Text = "{Move} {pp}/{tot pp}";
            move4Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // isShadowLabel
            // 
            isShadowLabel.AutoSize = true;
            isShadowLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            isShadowLabel.Location = new System.Drawing.Point(253, 28);
            isShadowLabel.Name = "isShadowLabel";
            isShadowLabel.Size = new System.Drawing.Size(176, 28);
            isShadowLabel.TabIndex = 10;
            isShadowLabel.Text = "Shadow";
            isShadowLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // abilityDisplayLabel
            // 
            abilityDisplayLabel.AutoSize = true;
            abilityDisplayLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            abilityDisplayLabel.Location = new System.Drawing.Point(3, 56);
            abilityDisplayLabel.Name = "abilityDisplayLabel";
            abilityDisplayLabel.Size = new System.Drawing.Size(62, 22);
            abilityDisplayLabel.TabIndex = 11;
            abilityDisplayLabel.Text = "Ability:";
            abilityDisplayLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // levelLabel
            // 
            levelLabel.AutoSize = true;
            levelLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            levelLabel.Location = new System.Drawing.Point(253, 56);
            levelLabel.Name = "levelLabel";
            levelLabel.Size = new System.Drawing.Size(176, 22);
            levelLabel.TabIndex = 13;
            levelLabel.Text = "Level";
            levelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PokemonDisplay
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "PokemonDisplay";
            Size = new System.Drawing.Size(456, 197);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label pokemonNameLabel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label hpLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label moveLabel;
        private System.Windows.Forms.Label speciesLabel;
        private System.Windows.Forms.Label move1Label;
        private System.Windows.Forms.Label move2Label;
        private System.Windows.Forms.Label move3Label;
        private System.Windows.Forms.Label move4Label;
        private System.Windows.Forms.Label isShadowLabel;
        private System.Windows.Forms.Label abilityDisplayLabel;
        private System.Windows.Forms.Label abilityLabel;
        private System.Windows.Forms.Label levelLabel;
    }
}
