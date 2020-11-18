using System.Diagnostics;
using System.Windows.Forms;

namespace NotepadApp
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        //Open my github link
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/isamuwatanabe");
        }
    }
}
