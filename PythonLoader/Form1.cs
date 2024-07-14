using Python_Loader.Helpers;
using System.Diagnostics;

namespace Python_Loader
{
    public partial class MainGUI : Form
    {
        public MainGUI()
        {
            InitializeComponent();

            btnInstallPython.HideInvoke();
            btnLaunchProgram.HideInvoke();
            btnCloseProgram.HideInvoke();

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
            string localVersion = result.isInstalled ? result.version.ToString() : "Not installed";
            txtLocalVersion.TextInvoke(localVersion);
            txtStatus.TextInvoke(result.isReady ? "PIP Processing" : "Please install python / update");

            if (!result.isInstalled || !result.isReady)
                btnInstallPython.ShowInvoke();
            if (result.isReady)
                btnLaunchProgram.ShowInvoke();

            //btnCloseProgram.Hide();
        }

        public void OnPythonClosed()
        {
            btnInstallPython.ShowInvoke();
            btnLaunchProgram.ShowInvoke();
            btnCloseProgram.HideInvoke();
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        #endregion

        #region Overriding for thread safety
        public new string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (this.InvokeRequired)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        base.Text = value;
                    });
                }
                else
                {
                    base.Text = value;
                }
            }
        }

        #endregion
    }
}
