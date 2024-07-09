namespace Python_Loader
{
    partial class MainGUI
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtStatus = new TextBox();
            textBox1 = new TextBox();
            txtLocalVersion_Header = new TextBox();
            textBox2 = new TextBox();
            txtLocalVersion = new TextBox();
            btnInstallPython = new Button();
            btnLaunchProgram = new Button();
            btnCloseProgram = new Button();
            SuspendLayout();
            // 
            // txtStatus
            // 
            txtStatus.AccessibleRole = AccessibleRole.None;
            txtStatus.Location = new Point(307, 61);
            txtStatus.Name = "txtStatus";
            txtStatus.ReadOnly = true;
            txtStatus.Size = new Size(204, 23);
            txtStatus.TabIndex = 0;
            txtStatus.Text = "Checking status";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 167);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(130, 23);
            textBox1.TabIndex = 1;
            textBox1.Text = "Recommended version: 312";
            // 
            // txtLocalVersion_Header
            // 
            txtLocalVersion_Header.Location = new Point(12, 206);
            txtLocalVersion_Header.Name = "txtLocalVersion_Header";
            txtLocalVersion_Header.ReadOnly = true;
            txtLocalVersion_Header.Size = new Size(130, 23);
            txtLocalVersion_Header.TabIndex = 2;
            txtLocalVersion_Header.Text = "Local version:";
            // 
            // textBox2
            // 
            textBox2.Enabled = false;
            textBox2.Location = new Point(148, 167);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(80, 23);
            textBox2.TabIndex = 3;
            textBox2.Text = "312";
            // 
            // txtLocalVersion
            // 
            txtLocalVersion.Location = new Point(148, 206);
            txtLocalVersion.Name = "txtLocalVersion";
            txtLocalVersion.ReadOnly = true;
            txtLocalVersion.Size = new Size(80, 23);
            txtLocalVersion.TabIndex = 4;
            txtLocalVersion.WordWrap = false;
            // 
            // btnInstallPython
            // 
            btnInstallPython.Location = new Point(224, 310);
            btnInstallPython.Name = "btnInstallPython";
            btnInstallPython.Size = new Size(134, 51);
            btnInstallPython.TabIndex = 5;
            btnInstallPython.Text = "Install Python";
            btnInstallPython.UseVisualStyleBackColor = true;
            // 
            // btnLaunchProgram
            // 
            btnLaunchProgram.Location = new Point(444, 310);
            btnLaunchProgram.Name = "btnLaunchProgram";
            btnLaunchProgram.Size = new Size(134, 51);
            btnLaunchProgram.TabIndex = 6;
            btnLaunchProgram.Text = "Launch program";
            btnLaunchProgram.UseVisualStyleBackColor = true;
            // 
            // btnCloseProgram
            // 
            btnCloseProgram.Location = new Point(334, 310);
            btnCloseProgram.Name = "btnCloseProgram";
            btnCloseProgram.Size = new Size(134, 51);
            btnCloseProgram.TabIndex = 7;
            btnCloseProgram.Text = "Close Python";
            btnCloseProgram.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnCloseProgram);
            Controls.Add(btnLaunchProgram);
            Controls.Add(btnInstallPython);
            Controls.Add(txtLocalVersion);
            Controls.Add(textBox2);
            Controls.Add(txtLocalVersion_Header);
            Controls.Add(textBox1);
            Controls.Add(txtStatus);
            Name = "Form1";
            Text = "Form1";
            Load += Form_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public TextBox txtStatus;
        private TextBox textBox1;
        private TextBox txtLocalVersion_Header;
        private TextBox txtLocalVersion;
        public TextBox textBox2;
        private Button btnInstallPython;
        private Button btnLaunchProgram;
        private Button btnCloseProgram;
    }
}
