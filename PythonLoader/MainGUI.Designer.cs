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
            btnLaunchProgram = new Button();
            btnCloseProgram = new Button();
            LabelProcess = new TransparentLabel();
            btnQuit = new Button();
            SuspendLayout();
            // 
            // btnLaunchProgram
            // 
            btnLaunchProgram.Anchor = AnchorStyles.None;
            btnLaunchProgram.Location = new Point(58, 134);
            btnLaunchProgram.Name = "btnLaunchProgram";
            btnLaunchProgram.Size = new Size(150, 50);
            btnLaunchProgram.TabIndex = 6;
            btnLaunchProgram.Text = "Launch program";
            btnLaunchProgram.UseVisualStyleBackColor = true;
            // 
            // btnCloseProgram
            // 
            btnCloseProgram.Anchor = AnchorStyles.None;
            btnCloseProgram.Location = new Point(58, 134);
            btnCloseProgram.Name = "btnCloseProgram";
            btnCloseProgram.Size = new Size(150, 50);
            btnCloseProgram.TabIndex = 7;
            btnCloseProgram.Text = "Close Python";
            btnCloseProgram.UseVisualStyleBackColor = true;
            // 
            // LabelProcess
            // 
            LabelProcess.Anchor = AnchorStyles.None;
            LabelProcess.BackColor = Color.Transparent;
            LabelProcess.CausesValidation = false;
            LabelProcess.Location = new Point(32, 76);
            LabelProcess.Name = "LabelProcess";
            LabelProcess.Size = new Size(200, 32);
            LabelProcess.TabIndex = 9;
            LabelProcess.Text = "Hello, world";
            LabelProcess.TextAlign = ContentAlignment.TopCenter;
            // 
            // btnQuit
            // 
            btnQuit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnQuit.BackColor = Color.Red;
            btnQuit.BackgroundImageLayout = ImageLayout.None;
            btnQuit.FlatAppearance.BorderSize = 0;
            btnQuit.FlatStyle = FlatStyle.Flat;
            btnQuit.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnQuit.ForeColor = Color.Transparent;
            btnQuit.Location = new Point(220, -2);
            btnQuit.Name = "btnQuit";
            btnQuit.Size = new Size(36, 25);
            btnQuit.TabIndex = 10;
            btnQuit.Text = "X";
            btnQuit.UseVisualStyleBackColor = false;
            btnQuit.Click += btnQuit_Click;
            // 
            // MainGUI
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 224, 192);
            ClientSize = new Size(255, 305);
            Controls.Add(btnQuit);
            Controls.Add(LabelProcess);
            Controls.Add(btnCloseProgram);
            Controls.Add(btnLaunchProgram);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainGUI";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Python Loader";
            Load += Form_Load;
            ResumeLayout(false);
        }

        #endregion
        private Button btnLaunchProgram;
        private Button btnCloseProgram;
        public TransparentLabel LabelProcess;
        private Button btnQuit;
    }
}
