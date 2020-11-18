namespace NotepadApp
{
    partial class InputBox
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
            this.label1 = new System.Windows.Forms.Label();
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.inputGoButton = new System.Windows.Forms.Button();
            this.inputCancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Line number:";
            // 
            // inputTextBox
            // 
            this.inputTextBox.Location = new System.Drawing.Point(11, 25);
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(241, 20);
            this.inputTextBox.TabIndex = 1;
            this.inputTextBox.Text = "1";
            this.inputTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.inputTextBox_KeyPress);
            // 
            // inputGoButton
            // 
            this.inputGoButton.Location = new System.Drawing.Point(96, 62);
            this.inputGoButton.Name = "inputGoButton";
            this.inputGoButton.Size = new System.Drawing.Size(75, 23);
            this.inputGoButton.TabIndex = 2;
            this.inputGoButton.Text = "Go to";
            this.inputGoButton.UseVisualStyleBackColor = true;
            this.inputGoButton.Click += new System.EventHandler(this.inputGoButton_Click);
            // 
            // inputCancelButton
            // 
            this.inputCancelButton.Location = new System.Drawing.Point(177, 62);
            this.inputCancelButton.Name = "inputCancelButton";
            this.inputCancelButton.Size = new System.Drawing.Size(75, 23);
            this.inputCancelButton.TabIndex = 3;
            this.inputCancelButton.Text = "Cancel";
            this.inputCancelButton.UseVisualStyleBackColor = true;
            this.inputCancelButton.Click += new System.EventHandler(this.inputCancelButton_Click);
            // 
            // InputBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 97);
            this.Controls.Add(this.inputCancelButton);
            this.Controls.Add(this.inputGoButton);
            this.Controls.Add(this.inputTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputBox";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Go to Line";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Button inputGoButton;
        private System.Windows.Forms.Button inputCancelButton;
    }
}