using ArchipelagoClient.Controls;
using XDCommon;

namespace Randomizer.AP
{
    public partial class EnterCredential : Form
    {
        public Credential Credential { get; set; }
        public EnterCredential()
        {
            InitializeComponent();
            Credential = new Credential();
        }

        private void urlTextBox_TextChanged(object sender, EventArgs e)
        {
            Credential.Url = urlTextBox.Text;
        }

        private void slotNameTextBox_TextChanged(object sender, EventArgs e)
        {
            Credential.Slotname = slotNameTextBox.Text;
        }

        private void passwordTextBox_TextChanged(object sender, EventArgs e)
        {
            Credential.Password = passwordTextBox.Text;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Configuration.APUrl = Credential.Url ?? string.Empty;
            Configuration.APSlotname = Credential.Slotname ?? string.Empty;
            Configuration.APPassword = Credential.Password ?? string.Empty;

            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void EnterCredential_Load(object sender, EventArgs e)
        {
            Credential.Url = Configuration.APUrl;
            Credential.Slotname = Configuration.APSlotname;
            Credential.Password = Configuration.APPassword;

            urlTextBox.Text = Credential.Url;
            slotNameTextBox.Text = Credential.Slotname;
            passwordTextBox.Text = Credential.Password;
        }
    }
}
