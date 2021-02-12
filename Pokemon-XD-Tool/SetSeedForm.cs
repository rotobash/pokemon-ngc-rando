using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Randomizer
{
    public partial class SetSeedForm : Form
    {
        public int NewSeed { get; private set; }
        public SetSeedForm()
        {
            InitializeComponent();
        }

        private void SetSeedForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            NewSeed = (int)newSeed.Value;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
