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
            Program.LoadPythonProgram(result.path);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
