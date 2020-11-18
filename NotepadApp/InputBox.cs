using System;
using System.Windows.Forms;

namespace NotepadApp
{
    public partial class InputBox : Form
    {
        //Create a instance of the main form
        MainForm mainFormInstance;
        int nlines;

        public InputBox(MainForm form, int lines)
        {
            InitializeComponent();

            mainFormInstance = form;

            //get the number of lines of the text area
            nlines = lines;
        }

        private void inputGoButton_Click(object sender, EventArgs e)
        {
            //Send the inputbox input to a variable in the main form
            mainFormInstance.InputBoxInput = inputTextBox.Text;

            //refresh the main form
            mainFormInstance.Refresh();

            this.Close();
        }

        private void inputCancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void inputTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //This is for the texbox allow only numbers
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;            
            }
        }
    }
}
