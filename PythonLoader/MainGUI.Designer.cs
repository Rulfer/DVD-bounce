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
            groupBox1 = new GroupBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // txtStatus
            // 
            txtStatus.AccessibleRole = AccessibleRole.None;
            txtStatus.BackColor = SystemColors.InactiveBorder;
            txtStatus.BorderStyle = BorderStyle.None;
            txtStatus.Location = new Point(303, 79);
            txtStatus.Name = "txtStatus";
            txtStatus.ReadOnly = true;
            txtStatus.ShortcutsEnabled = false;
            txtStatus.Size = new Size(204, 16);
            txtStatus.TabIndex = 0;
            txtStatus.Text = "Checking status";
            txtStatus.TextAlign = HorizontalAlignment.Center;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(-2, 22);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(130, 23);
            textBox1.TabIndex = 1;
            textBox1.Text = "Recommended version: 312";
            // 
            // txtLocalVersion_Header
            // 
            txtLocalVersion_Header.Location = new Point(-2, 61);
            txtLocalVersion_Header.Name = "txtLocalVersion_Header";
            txtLocalVersion_Header.ReadOnly = true;
            txtLocalVersion_Header.Size = new Size(130, 23);
            txtLocalVersion_Header.TabIndex = 2;
            txtLocalVersion_Header.Text = "Local version:";
            // 
            // textBox2
            // 
            textBox2.Enabled = false;
            textBox2.Location = new Point(134, 22);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(80, 23);
            textBox2.TabIndex = 3;
            textBox2.Text = "3.12.3";
            // 
            // txtLocalVersion
            // 
            txtLocalVersion.Location = new Point(134, 61);
            txtLocalVersion.Name = "txtLocalVersion";
            txtLocalVersion.ReadOnly = true;
            txtLocalVersion.Size = new Size(80, 23);
            txtLocalVersion.TabIndex = 4;
            txtLocalVersion.WordWrap = false;
            // 
            // btnInstallPython
            // 
            btnInstallPython.Location = new Point(334, 201);
            btnInstallPython.Name = "btnInstallPython";
            btnInstallPython.Size = new Size(134, 51);
            btnInstallPython.TabIndex = 5;
            btnInstallPython.Text = "Install Python";
            btnInstallPython.UseVisualStyleBackColor = true;
            // 
            // btnLaunchProgram
            // 
            btnLaunchProgram.Location = new Point(334, 201);
            btnLaunchProgram.Name = "btnLaunchProgram";
            btnLaunchProgram.Size = new Size(134, 51);
            btnLaunchProgram.TabIndex = 6;
            btnLaunchProgram.Text = "Launch program";
            btnLaunchProgram.UseVisualStyleBackColor = true;
            // 
            // btnCloseProgram
            // 
            btnCloseProgram.Location = new Point(215, 377);
            btnCloseProgram.Name = "btnCloseProgram";
            btnCloseProgram.Size = new Size(154, 51);
            btnCloseProgram.TabIndex = 7;
            btnCloseProgram.Text = "Close Python";
            btnCloseProgram.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(txtLocalVersion_Header);
            groupBox1.Controls.Add(txtLocalVersion);
            groupBox1.Location = new Point(23, 140);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(246, 100);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "groupBox1";
            // 
            // MainGUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(574, 450);
            Controls.Add(groupBox1);
            Controls.Add(btnCloseProgram);
            Controls.Add(btnLaunchProgram);
            Controls.Add(btnInstallPython);
            Controls.Add(txtStatus);
            Name = "MainGUI";
            Text = "Form1";
            Load += Form_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
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
        private GroupBox groupBox1;
    }
}
