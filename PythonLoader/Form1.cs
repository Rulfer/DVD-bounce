using System.Diagnostics;

namespace Python_Loader
{
    public partial class MainGUI : Form
    {
        public MainGUI()
        {
            InitializeComponent();

            btnInstallPython.Hide();
            btnLaunchProgram.Hide();
            btnCloseProgram.Hide();

            btnLaunchProgram.Click += OnLaunchProgramClicked;
            btnInstallPython.Click += OnInstallPythonClicked;
            btnCloseProgram.Click += OnCloseProgramClicked;
        }

        private void Form_Load(object sender, EventArgs e)
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

            txtStatus.Text = "PIP Processing";

            //btnInstallPython.Show();
            //btnLaunchProgram.Show();
            //btnCloseProgram.Hide();
        }

        public void OnPythonClosed()
        {
            Invoke(new Action(() =>
            {
                btnInstallPython.Show();
                btnLaunchProgram.Show();
                btnCloseProgram.Hide();
            }));
        }

        #region UI events
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
        #endregion
    }
}
