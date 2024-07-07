using System.Diagnostics;
using System.Windows.Forms;
using System;
using System.Management;
using System.IO;

namespace Python_Loader
{
    public static class Program
    {
        public static Form1 Form { get; private set; } = null;
        public static PythonVersionManager PythonVersionManager { get; private set; } = new PythonVersionManager();

        private static PythonVersionManager.Python _python;
        private static Process _process = null;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            RedirectConsoleOutput();
            Console.WriteLine("Hello, world");
            try
            {
                Form = new Form1();
                Application.Run(Form);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }

        }

        private static void RedirectConsoleOutput()
        {
            string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "myapp_log.txt");
            StreamWriter logFile = new StreamWriter(logFilePath);
            logFile.AutoFlush = true;
            Console.SetOut(logFile);
            Console.SetError(logFile);
        }

        public static void OnFormLoaded()
        {
            Console.WriteLine("OnFormLoaded");
            Debug.WriteLine("OnFormLoaded");
            PythonVersionManager.RetrieveVersion();
            //LoadPythonProgram();
        }

        public static void OnPythonDataRetrieved(PythonVersionManager.Python result)
        {
            _python = new PythonVersionManager.Python();
            _python.isInterrupted = result.isInterrupted; 
            _python.isInstalled = result.isInstalled; 
            _python.version = result.version; 
            _python.path = result.path;

            Form.OnPythonDataRetrieved(result);
        }

        public static void LoadInstallPython()
        {
            string path;
#if DEBUG
            string gitFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            path = Path.Combine(gitFolder, "python-3.12.0-amd64.exe");
#else
            path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Python/Installer/python-3.12.0-amd64.exe");
#endif
            CloseProcess();

            try
            {
                // Create a new process to start the Python script
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = path;
                //psi.Arguments = path;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.CreateNoWindow = true;


                _process = new Process();
                _process.Exited += new EventHandler(OnProcessExited);
                _process.StartInfo = psi;
                _process.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public static void LoadPythonProgram()
        {
            //string path = @"F:\PersonligeProsjekter\DVD-bounce\dvd\main.py";
            //string path = @"F:\PersonligeProsjekter\DVD-bounce\PythonLoader\dvd\main.py";
            string path;
#if DEBUG
            string gitFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            path = Path.Combine(gitFolder, "dvd/main.py");
#else
            path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Python/Build/main.py");
#endif
            Debug.WriteLine("python.exe path: " + _python.path);
            Debug.WriteLine("DVD path: " + path);
            Console.WriteLine("python.exe path: " + _python.path);
            Console.WriteLine("DVD path: " + path);
            Console.WriteLine("Directory.GetCurrentDirectory(): " + Directory.GetCurrentDirectory());

            CloseProcess();

            try
            {
                // Create a new process to start the Python script
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = _python.path;
                psi.Arguments = path;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.CreateNoWindow = true;


                _process = new Process();
                _process.Exited += new EventHandler(OnProcessExited);
                _process.StartInfo = psi;
                _process.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public static void CloseProcess()
        {
            if (_process == null)
                return;

            _process.Exited -= OnProcessExited;
            _process.Kill();
            //_process.Close();
            //_process.Dispose();
        }

        private static void OnProcessExited(object sender, System.EventArgs e)
        {
            _process = null;
            Form.OnPythonClosed();
        }
    }
}