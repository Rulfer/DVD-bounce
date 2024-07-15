using System.Diagnostics;
using System.Windows.Forms;
using System;
using System.Management;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Python_Loader
{
    /// <summary>
    /// Main handler for my program. This holds a reference to all other handlers.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main GUI of this application.
        /// </summary>
        public static MainGUI Form { get; private set; } = null;
        /// <summary>
        /// Retrieve version of existing Python installation, if any.
        /// </summary>
        public static PythonVersionManager PythonVersionManager { get; private set; } = new PythonVersionManager();

        //public static PythonVersionManager.Python PythonValueCache;
        internal static ProcessHandler ProcessHandler = null;
        internal static PipHandler PipHandler { get; private set; } = new PipHandler();

        #region Cached data
        private static string _pathToMainPy = null;
        private static string WorkingDirectory;
        #endregion

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnApplicationQuit);


#if DEBUG
            WorkingDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            _pathToMainPy = Path.Combine(WorkingDirectory, "dvd/main.py");
#else
            WorkingDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            _pathToMainPy = Path.Combine(WorkingDirectory, "Python/Build/main.py");
#endif

            RedirectConsoleOutput();
            try
            {
                Form = new MainGUI();
                Application.Run(Form);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
            }
        }

        static void OnApplicationQuit(object sender, EventArgs e)
        {
            Debug.WriteLine("Farewell, world.");
            ProcessHandler?.Terminate();
        }

        private static void RedirectConsoleOutput()
        {
            string logFilePath = Path.Combine(WorkingDirectory, "myapp_log.txt");
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

        //public static void OnPythonLocated(PythonVersionManager.Python result)
        //{
        //    if (result.isReady)
        //    {
        //        LoadPIPPatcher();
        //    }

        //    Form.OnPythonDataRetrieved(result);
        //}

        public static void OnEmbeddedPythonReady()
        {
            Form.OnPythonLoaded();
            LoadPIPPatcher();
        }

        /// <summary>
        /// Identify installed packages and installing missing / update existing.
        /// </summary>
        public static void LoadPIPPatcher()
        {
            PipHandler.Update();
        }

        public static void LoadInstallPython()
        {
            string fileName;
#if DEBUG
            //string gitFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            //path = Path.Combine(WorkingDirectory, "python-3.12.0-amd64.exe");
            fileName = "python-3.12.0-amd64.exe";
#else
            //path = Path.Combine(WorkingDirectory, "Python/Installer/python-3.12.0-amd64.exe");
            fileName = "Python/Installer/python-3.12.0-amd64.exe";
#endif
            CloseProcess();

            try
            {
                ProcessHandler = new ProcessHandler(fileName: fileName, workingDirectory: WorkingDirectory, new EventHandler(OnProcessExited));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public static void LoadPythonProgram()
        {
            string path;

            CloseProcess();

            try
            {
#if DEBUG
                _pathToMainPy = Path.Combine(WorkingDirectory, "dvd/main.py");
#else
                _pathToMainPy = Path.Combine(WorkingDirectory, "Python/Build/main.py");

#endif
                //ProcessHandler = new ProcessHandler(optionalData: new ProcessHandler.OptionalData(arguments: _pathToMainPy), onDone: new EventHandler(OnProcessExited), fileName: "py");
                ProcessHandler = new ProcessHandler(workingDirectory: PythonVersionManager.EmbeddedPath, fileName: "py", argument: _pathToMainPy, optionalData: new ProcessHandler.OptionalData(), onDone: new EventHandler(OnProcessExited));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public static void CloseProcess()
        {
            ProcessHandler?.Terminate();
        }

        private static void OnProcessExited(object? sender, EventArgs e)
        {
            CloseProcess();
            Form.OnPythonClosed();
        }
    }
}