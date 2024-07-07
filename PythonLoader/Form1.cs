using System.Diagnostics;

namespace Python_Loader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            btnInstallPython.Hide();
            btnLaunchProgram.Hide();
            btnCloseProgram.Hide();

            btnLaunchProgram.Click += OnLaunchProgramClicked;
            btnInstallPython.Click += OnInstallPythonClicked;
            btnCloseProgram.Click += OnCloseProgramClicked;
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            txtStatus.Text = "Hello!";
            Program.OnFormLoaded();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {

        }

        public void OnPythonDataRetrieved(PythonVersionManager.Python result)
        {
            if (result.isInstalled)
                txtLocalVersion.Text = result.version.ToString();
            else
                txtLocalVersion.Text = "Not installed";

            btnInstallPython.Show();
            btnLaunchProgram.Show();
            btnCloseProgram.Hide();
        }

        public void OnPythonClosed()
        {
            btnInstallPython.Show();
            btnLaunchProgram.Show();
            btnCloseProgram.Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void OnInstallPythonClicked(object? sender, EventArgs e)
        {
            btnInstallPython.Hide();
            btnLaunchProgram.Hide();

            Program.LoadInstallPython();
        }

        private void OnLaunchProgramClicked(object? sender, EventArgs e)
        {
            Program.LoadPythonProgram();
            btnInstallPython.Hide();
            btnLaunchProgram.Hide();
            btnCloseProgram.Show();
        }

        private void OnCloseProgramClicked(object? sender, EventArgs e)
        {
            Program.CloseProcess();
            OnPythonClosed();
        }
    }
}
