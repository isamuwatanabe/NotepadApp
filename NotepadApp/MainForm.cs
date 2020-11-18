using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace NotepadApp
{
    public partial class MainForm : Form
    {
        StringReader reader;

        //Variables to verify if the file needs to save or already exist
        bool needToSave = false, alreadyExist = false;

        //Variables to store the file name and file location
        string fileName, fileLocation = null;

        //Variable to the start position in the find function
        int findPos = 0;

        //This is to change the dialog language
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        static extern System.UInt16 SetThreadUILanguage(System.UInt16 LangId);


        public MainForm()
        {
            InitializeComponent();
            
            //Set the default name of the archive
            fileName = "untitled.txt";
            this.Text = fileName;

            pasteToolStripMenuItem.Enabled = Clipboard.GetDataObject().GetDataPresent(DataFormats.Text);
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            //Verify if the file needs to save(new one or already existing file)
            if(richTextBox.Text.Length > 0)
            {
                needToSave = true;

                //Enable the find and find next
                if (!findToolStripMenuItem.Enabled)
                    findToolStripMenuItem.Enabled = true;

                if (!findNextToolStripMenuItem.Enabled)
                    findNextToolStripMenuItem.Enabled = true;
            }
            else if (alreadyExist)
            {
                //If there's no text, disable find
                findToolStripMenuItem.Enabled = false;

                needToSave = true;
            }
            else
            {
                //If there's no text, disable find
                findToolStripMenuItem.Enabled = false;

                needToSave = false;
            }

            //Enable the undo if you type something
            undoToolStripMenuItem.Enabled = true;

            //Disable de redo if something is typed after the undo
            if (redoToolStripMenuItem.Enabled)
                redoToolStripMenuItem.Enabled = false;
        }

        private void richTextBox_SelectionChanged(object sender, EventArgs e)
        {
            
            if (statusBarToolStripMenuItem.Enabled)
            {
                int index = richTextBox.SelectionStart;

                //Get the line number
                int line = richTextBox.GetLineFromCharIndex(index);

                int firstChar = richTextBox.GetFirstCharIndexFromLine(line);

                //Get the column number
                int column = index - firstChar;

                //Set the tatus bar
                toolStripStatusLabel1.Text = "Ln " + (line + 1) + ", " + "Col " + (column + 1);
            }

            //Verification to enable cut, copy and delete
            cutToolStripMenuItem.Enabled = richTextBox.SelectedText.Length > 0 ? true : false;
            copyToolStripMenuItem.Enabled = richTextBox.SelectedText.Length > 0 ? true : false;
            deleteToolStripMenuItem.Enabled = richTextBox.SelectedText.Length > 0 ? true : false;

            //Set the start find position
            findPos = richTextBox.SelectionStart;
        }

        private void richTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //Set the undo and redo to be word by word or if enter is pressed
            RichTextBox rtb = (RichTextBox)sender;
            if (e.KeyCode == Keys.Space | e.KeyCode == Keys.Enter)
            {
                this.SuspendLayout();
                rtb.Undo();
                rtb.Redo();
                this.ResumeLayout();
            }

            //When the esc button is pressed, disable find
            if (e.KeyCode == Keys.Escape & findLabel.Enabled)
            {
                FindAndReplaceHide();
            }
        }

        //--------------------FILE MENU--------------------

        //Function to create a new file
        private void New()
        {
            //Check if needs to save
            if (needToSave)
            {
                DialogResult result = MessageBox.Show("Do you want to save changes ?", "Notepad", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if(result == DialogResult.Yes)
                {
                    //Save the file and create a new file after
                    Save();
                    richTextBox.Clear();
                    richTextBox.Focus();
                }
                //If not, just clear the text area and create a new file
                else if (result == DialogResult.No)
                {
                    fileName = "untitled.txt";
                    this.Text = fileName;

                    richTextBox.Clear();
                    richTextBox.Focus();

                    alreadyExist = false;
                    needToSave = false;
                }
            }
            //Don't need to save, clear the text area and create a new file
            else
            {
                fileName = "untitled.txt";
                this.Text = fileName;

                richTextBox.Clear();
                richTextBox.Focus();

                alreadyExist = false;
                needToSave = false;
            }

            
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            New();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            New();
        }


        //Function to open a file
        private void Open()
        {

            //Set dialog language to english
            SetThreadUILanguage(1033);

            //Set the file filters for the open file dialog
            this.openFileDialog.Filter = "(*.txt)|*.txt|All Files(*.*)|*.*";

            this.openFileDialog.FileName = "";

            //As to save before opening another file
            if (needToSave)
            {
                DialogResult result = MessageBox.Show("Do you want to save changes ?", "Notepad", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Save();                  
                }
            }

            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //Create a filestream for file reading and set the streamreader to read the file
                    FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                    StreamReader streamReader = new StreamReader(fileStream);

                    //Set to read from the beginning
                    streamReader.BaseStream.Seek(0, SeekOrigin.Begin);

                    //Clean the text area
                    this.richTextBox.Text = "";

                    //Read the whole file
                    string txt = streamReader.ReadToEnd();

                    //Update the text area
                    richTextBox.Text = txt;

                    streamReader.Close();

                    //Get the file name
                    fileName = System.IO.Path.GetFileName(openFileDialog.FileName);

                    //Get the file location
                    fileLocation = openFileDialog.FileName;

                    //If you are opening a file, it exists
                    alreadyExist = true;

                    //Show the file name
                    this.Text = fileName;

                    //Don't need to save, you just opened the file
                    needToSave = false;
                }
                catch(Exception ex)
                {
                    //If any opening error occurs, show the error message
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            Open();
        }


        //Function to save the file
        private void Save()
        {
            //Set the filters for the save dialog
            saveFileDialog.Filter = "(*.txt)|*.txt|All Files(*.*)|*.*";

            //Set the default file name in dialog
            saveFileDialog.FileName = fileName;

            //Set dialog language to english
            SetThreadUILanguage(1033);

            try
            {
                //If the file already exists, don't show the dialog, just save
                if (alreadyExist)
                {
                    //Set the streamwriter to overwrite the previous text
                    StreamWriter streamWriter = new StreamWriter(fileLocation, false);

                    //Clear the buffer
                    streamWriter.Flush();

                    //Set to write from the beginning
                    streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);

                    //Write in the file
                    streamWriter.Write(this.richTextBox.Text);

                    streamWriter.Flush();

                    streamWriter.Close();

                    //Just saved, don't need to save
                    needToSave = false;
                }
                //If the file is not already saved, show the dialog
                else if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Create a filestream for file writing and set the streamreader to write the file
                    FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter streamWriter = new StreamWriter(fileStream);

                    //Clear the buffer
                    streamWriter.Flush();

                    //Set to write from the beginning
                    streamWriter.BaseStream.Seek(0, SeekOrigin.Begin);

                    //Write in the file
                    streamWriter.Write(this.richTextBox.Text);

                    streamWriter.Flush();
                    streamWriter.Close();

                    //Get the file name
                    fileName = System.IO.Path.GetFileName(saveFileDialog.FileName);

                    //Get the file location
                    fileLocation = saveFileDialog.FileName;

                    //Just saved, it exists
                    alreadyExist = true;

                    //Show the file name
                    this.Text = fileName;

                    //Just saved, don't need to save
                    needToSave = false;
                }
            }
            catch (Exception ex)
            {
                //If any saving error occurs, show the error message
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Set it to false to show the save dialog
            alreadyExist = false;

            Save();
        }

        //Configure page function
        private void configurePage()
        {
            //View the print page
            try
            {
                string strText = this.richTextBox.Text;
                reader = new StringReader(strText);
                PrintPreviewDialog printPreviewDialog1 = new PrintPreviewDialog();
                var prn = printPreviewDialog1;
                prn.Document = this.printDocument;
                prn.Text = "Print Page";
                prn.WindowState = FormWindowState.Normal;
                prn.PrintPreviewControl.Zoom = 1;
                prn.FormBorderStyle = FormBorderStyle.Fixed3D;
                prn.ShowIcon = false;
                prn.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void configurePageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            configurePage();
        }


        //Function to print  the file
        private void Print()
        {

            printDialog.Document = printDocument;
            string txt = richTextBox.Text;
            reader = new StringReader(txt);

            if(printDialog.ShowDialog() == DialogResult.OK)
            {
                this.printDocument.Print();
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Print();
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            Print();
        }

        //Function to configure the print page
        private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            float pageLines = 0;
            float posY = 0;
            int cont = 0;

            //Define the margins and the minimun value
            float marginLeft = e.MarginBounds.Left - 50;
            float marginTop = e.MarginBounds.Top - 50;

            if (marginLeft < 5)
            {
                marginLeft = 20;
            }

            if (marginTop < 5)
            {
                marginTop = 20;
            }

            //Define the font
            string line = null;
            Font font = this.richTextBox.Font;
            SolidBrush brush = new SolidBrush(Color.Black);

            //Calculate the number of lines per page using the margins measurements
            pageLines = e.MarginBounds.Height / font.GetHeight(e.Graphics);

            //Print each line using a stringreader
            line = reader.ReadLine();

            while (cont < pageLines)
            {
                //Calculate the position of the next line based in the font height according to the printing device
                posY = (marginTop + (cont * font.GetHeight(e.Graphics)));

                //Draw the next line in richtextbox
                e.Graphics.DrawString(line, font, brush, marginLeft, posY, new StringFormat());

                //Count the line and adds 1
                cont++;
                line = reader.ReadLine();
            }

            //If there's more lines, print another page
            if (line != null)
            {
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
            }

            brush.Dispose();
        }

        //Function to close the application
        private bool Exit()
        {
            bool cancel = false;

            //Save before exit
            if (needToSave)
            {
                DialogResult result = MessageBox.Show("Do you want to save changes ?", "Notepad", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    Save();
                    Application.Exit();
                }
                else if(result == DialogResult.No)
                {
                    needToSave = false;
                    Application.Exit();
                }
                else
                {
                    cancel = true;
                }                               
            }
            else
            {
                Application.Exit();
            }

            return cancel;
        }

        //The same thing as before, but for the form close button
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Exit())
            {
                e.Cancel = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }


        //--------------------EDIT MENU--------------------

        //Verification to enable some options in the edit menu
        private void editToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {           
            pasteToolStripMenuItem.Enabled = Clipboard.GetDataObject().GetDataPresent(DataFormats.Text);
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.Undo();

            //If undo, enable redo
            redoToolStripMenuItem.Enabled = true;
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.Redo();

            //If redo, enable undo
            undoToolStripMenuItem.Enabled = true;
        }

        //Cut function
        private void Cut()
        {
            richTextBox.Cut();
            pasteToolStripButton.Enabled = true;
        }
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cut();
        }

        //Copy function
        private void Copy()
        {
            //If there's something selected, copy it
            if(richTextBox.SelectionLength > 0)
            {
                richTextBox.Copy();
                pasteToolStripButton.Enabled = true;
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            Copy();
        }

        //Paste function
        private void Paste()
        {
            richTextBox.Paste();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Paste();
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            Paste();
        }

        //Delete function
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Remove the selected text
            richTextBox.Text = richTextBox.Text.Remove(richTextBox.SelectionStart, richTextBox.SelectionLength);
        }

        //Function to disable and hide the visuals of find/replace
        private void FindAndReplaceHide()
        {
            if (replaceLabel.Enabled)
            {
                richTextBox.Location = new Point(12, 52);
                richTextBox.Height += 51;

                replaceLabel.Visible = false;
                replaceLabel.Enabled = false;

                replaceTextBox.Visible = false;
                replaceTextBox.Enabled = false;

                replaceButton.Visible = false;
                replaceButton.Enabled = false;

                replaceAllButton.Visible = false;
                replaceAllButton.Enabled = false;

                findNextToolStripMenuItem.Enabled = true;
            }
            else
            {
                richTextBox.Location = new Point(12, 52);
                richTextBox.Height += 27;

                replaceToolStripMenuItem.Enabled = true;
            }

            findLabel.Visible = false;
            findLabel.Enabled = false;

            findTextBox.Visible = false;
            findTextBox.Enabled = false;

            findNextButton.Visible = false;
            findNextButton.Enabled = false;

            findCheckBox.Visible = false;
            findCheckBox.Enabled = false;

            findExitbutton.Visible = false;
            findExitbutton.Enabled = false;
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //When find is enabled, disable the replace
            replaceToolStripMenuItem.Enabled = false;

            //Activate it and show the visual
            if (!findLabel.Enabled)
            {
                richTextBox.Location = new Point(12, 79);
                richTextBox.Height -= 27;

                findLabel.Visible = true;
                findLabel.Enabled = true;

                findTextBox.Visible = true;
                findTextBox.Enabled = true;

                findNextButton.Visible = true;
                findNextButton.Enabled = true;

                findCheckBox.Visible = true;
                findCheckBox.Enabled = true;

                findExitbutton.Visible = true;
                findExitbutton.Enabled = true;
            }          
        }

        private void findExitbutton_Click(object sender, EventArgs e)
        {
            //When the x button is pressed, bring form back to normal
            FindAndReplaceHide();
        }

        //Find next function
        //searchText = string that you want to find | matchCase = If it is case sensitive or not | replaceAll = If it is the replace all function, to not show the dialog
        public bool FindNext(string searchText, bool matchCase, bool replaceAll)
        {
            try
            {
                //Give focus
                this.Focus();
                richTextBox.Focus();

                //Set the type of comparison
                StringComparison type = matchCase == true ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                //Find the position of the desired string
                findPos = richTextBox.Text.IndexOf(searchText, findPos, type);

                //Select the string
                richTextBox.Select(findPos, searchText.Length);

                //Set the position to after this string
                findPos += searchText.Length;

                //Return true is sucessfull
                return true;
            }
            catch
            {
                //If is replace all, don't show the dialogf
                if(replaceAll)
                    return false;
                
                //If not, show the dialog
                MessageBox.Show("Search text: '" + findTextBox.Text + "' could not be found", "Text Not Found", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                
                //Return false if it's not sucessfull
                return false;
            }
        }

        private void findNextButton_Click(object sender, EventArgs e)
        {
            FindNext(findTextBox.Text, findCheckBox.Checked, false);
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindNext(findTextBox.Text, findCheckBox.Checked, false);
        }

        private void findTextBox_TextChanged(object sender, EventArgs e)
        {
            //If the text in the find textbox is changed, the start position will be where the cursor is 
            findPos = richTextBox.SelectionStart;
        }

        //Replace functions
        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //When replace is activated, disable find
            findNextToolStripMenuItem.Enabled = false;

            //Enable and show find
            if (!replaceLabel.Enabled)
            {
                richTextBox.Location = new Point(12, 103);
                richTextBox.Height -= 51;

                findLabel.Visible = true;
                findLabel.Enabled = true;

                findTextBox.Visible = true;
                findTextBox.Enabled = true;

                findNextButton.Visible = true;
                findNextButton.Enabled = true;

                findCheckBox.Visible = true;
                findCheckBox.Enabled = true;

                findExitbutton.Visible = true;
                findExitbutton.Enabled = true;

                replaceLabel.Visible = true;
                replaceLabel.Enabled = true;

                replaceTextBox.Visible = true;
                replaceTextBox.Enabled = true;

                replaceButton.Visible = true;
                replaceButton.Enabled = true;

                replaceAllButton.Visible = true;
                replaceAllButton.Enabled = true;
            }
        }

        private void replaceButton_Click(object sender, EventArgs e)
        {
            //If there is something selected, replace it
            if(richTextBox.SelectedText.Length > 0)
            {
                richTextBox.SelectedText = richTextBox.SelectedText.Replace(richTextBox.SelectedText, replaceTextBox.Text);
            }
            //If not, find the string
            else
            {
                FindNext(findTextBox.Text, findCheckBox.Checked, false);
            }
        }

        private void replaceAllButton_Click(object sender, EventArgs e)
        {
            //Set it to 0 to replace since the beginning
            findPos = 0;

            //While it still finding, do the replace
            while(FindNext(findTextBox.Text, findCheckBox.Checked, true))
            {
                richTextBox.SelectedText = richTextBox.SelectedText.Replace(richTextBox.SelectedText, replaceTextBox.Text);
            }
        }


        //Inputbox for the go to(I was going to use the visual basic one, but i didn't like the design that much)

        //Variable for receiving the inputbox input
        private string inputBoxInput;

        public string InputBoxInput { get => inputBoxInput; set => inputBoxInput = value; }

        //Go To function
        private void goToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Create and show the inputbox
            InputBox inputBox = new InputBox(this, richTextBox.Lines.Length);
            inputBox.ShowDialog();

            try
            {
                //Get the input and convert it to int
                if (Convert.ToInt32(inputBoxInput) > 0)
                {
                    int line = Convert.ToInt32(inputBoxInput);

                    //If the line doesn't exists
                    if (line > richTextBox.Lines.Length)
                    {
                        MessageBox.Show("The line number is greater than the total number of lines");
                    }
                    else
                    {
                        //Go to the line
                        this.richTextBox.Select(this.richTextBox.GetFirstCharIndexFromLine(line - 1), 0);
                        this.richTextBox.ScrollToCaret();
                    }
                }
                
            }
            catch(Exception ex)
            {
                //If any error occurs, show error message
                MessageBox.Show("Error" + ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Select all function
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.SelectAll();
        }

        //Get time date function
        private void timeDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox.Text = richTextBox.Text.Insert(richTextBox.SelectionStart, DateTime.Now.ToString());
        }


        ///--------------------FORMAT MENU--------------------

        //Word wrap function
        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //If the status bar is enabled, disable it an uncheck it in the menu
            if (statusBarToolStripMenuItem.Checked)
            {
                statusStrip.Visible = false;
                statusBarToolStripMenuItem.Checked = false;
            }

            //If is already enabled, disable it
            if (richTextBox.WordWrap == true)
            {
                richTextBox.WordWrap = false;
                wordWrapToolStripMenuItem.Checked = false;
                goToToolStripMenuItem.Enabled = true;
            }
            else
            {
                richTextBox.WordWrap = true;
                wordWrapToolStripMenuItem.Checked = true;
                goToToolStripMenuItem.Enabled = false;
            }
        }

        //Change font function
        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Set dialog language to english
            SetThreadUILanguage(1033);

            if (fontDialog.ShowDialog() == DialogResult.OK)
            {
                richTextBox.Font = fontDialog.Font;
            }
        }

        //Bold font function
        private void Bold()
        {
            string fontName = null;
            float fontSize = 0;

            //Variables to verify if it is already bold, italic and underline
            bool b, i, u;

            //Get the font name and size
            fontName = richTextBox.Font.Name;
            fontSize = richTextBox.Font.Size;

            //Get the font style
            b = richTextBox.SelectionFont.Bold;
            i = richTextBox.SelectionFont.Italic;
            u = richTextBox.SelectionFont.Underline;

            richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Regular);

            //Add the bold style to the font without removing any other used style
            if (b == false)
            {
                if (i == true & u == true)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Bold | FontStyle.Italic | FontStyle.Underline);
                }
                else if (i == false & u == true)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Bold | FontStyle.Underline);
                }
                else if (i == true & u == false)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Bold | FontStyle.Italic);
                }
                else
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Bold);
                }

            }
            //Remove the bold style to the font without removing any other used style
            else
            {
                if (i == true & u == true)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Italic | FontStyle.Underline);
                }
                else if (i == false & u == true)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Underline);
                }
                else if (i == true & u == false)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Italic);
                }

            }
        }

        private void boldToolStripButton_Click(object sender, EventArgs e)
        {
            Bold();
        }


        //Italic font function
        private void Italic()
        {
            string fontName = null;
            float fontSize = 0;

            //Variables to verify if it is already bold, italic and underline
            bool b, i, u = false;

            //Get the font name and size
            fontName = richTextBox.Font.Name;
            fontSize = richTextBox.Font.Size;

            //Get the font style
            b = richTextBox.SelectionFont.Bold;
            i = richTextBox.SelectionFont.Italic;
            u = richTextBox.SelectionFont.Underline;

            richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Regular);

            //Add the italic style to the font without removing any other used style
            if (i == false)
            {
                if (b == true & u == true)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Italic | FontStyle.Bold | FontStyle.Underline);
                }
                else if (b == false & u == true)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Italic | FontStyle.Underline);
                }
                else if (b == true & u == false)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Bold | FontStyle.Italic);
                }
                else
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Italic);
                }

            }
            //Remove the italic style to the font without removing any other used style
            else
            {
                if (b == true & u == true)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Bold | FontStyle.Underline);
                }
                else if (b == false & u == true)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Underline);
                }
                else if (b == true & u == false)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Bold);
                }

            }

        }

        private void italicToolStripButton_Click(object sender, EventArgs e)
        {
            Italic();
        }

        //Underline font function
        private void Uderline()
        {
            string fontName = null;
            float fontSize = 0;

            //Variables to verify if it is already bold, italic and underline
            bool b, i, u = false;

            //Get the font name and size
            fontName = richTextBox.Font.Name;
            fontSize = richTextBox.Font.Size;

            //Get the font style
            b = richTextBox.SelectionFont.Bold;
            i = richTextBox.SelectionFont.Italic;
            u = richTextBox.SelectionFont.Underline;

            richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Regular);

            //Add the underline style to the font without removing any other used style
            if (u == false)
            {
                if (b == true & i == true)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Underline | FontStyle.Bold | FontStyle.Italic);
                }
                else if (b == false & i == true)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Italic | FontStyle.Underline);
                }
                else if (b == true & i == false)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Bold | FontStyle.Underline);
                }
                else
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Underline);
                }

            }
            //Remove the italic style to the font without removing ny other used style
            else
            {
                if (b == true & i == true)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Bold | FontStyle.Italic);
                }
                else if (b == false & i == true)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Italic);
                }
                else if (b == true & i == false)
                {
                    richTextBox.SelectionFont = new Font(fontName, fontSize, FontStyle.Bold);
                }

            }

        }

        private void underlineToolStripButton_Click(object sender, EventArgs e)
        {
            Uderline();
        }


        //Aling left function
        private void AlignLeft()
        {
            richTextBox.SelectionAlignment = HorizontalAlignment.Left;
        }

        private void leftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AlignLeft();
        }

        private void leftToolStripButton_Click(object sender, EventArgs e)
        {
            AlignLeft();
        }

        //Align right function
        private void AlignRight()
        {
            richTextBox.SelectionAlignment = HorizontalAlignment.Right;
        }

        private void rightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AlignRight();
        }

        private void rightToolStripButton_Click(object sender, EventArgs e)
        {
            AlignRight();
        }

        //Align center function
        private void AlignCenter()
        {
            richTextBox.SelectionAlignment = HorizontalAlignment.Center;
        }

        private void centerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AlignCenter();
        }

        private void centerToolStripButton_Click(object sender, EventArgs e)
        {
            AlignCenter();
        }


        //--------------------SHOW MENU--------------------

        //Function for the status bar
        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //If word wrap is enabled, disable it and uncheck it in the menu
            if (wordWrapToolStripMenuItem.Checked)
            {
                richTextBox.WordWrap = false;
                wordWrapToolStripMenuItem.Checked = false;
            }

            //If is already enabled, disable it
            if (statusStrip.Visible)
            {               
                statusBarToolStripMenuItem.Checked = false;
                statusStrip.Visible = false;
            }
            else
            {
                statusBarToolStripMenuItem.Checked = true;
                statusStrip.Visible = true;
            }
        }


        //--------------------HELP MENU--------------------

        //lol
        private void showHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.google.com");
        }

        //About me
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }
    }
}