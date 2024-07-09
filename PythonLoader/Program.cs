using System.Diagnostics;
using System.Windows.Forms;
using System;
using System.Management;
using System.IO;

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

        private static PythonVersionManager.Python _pythonValueCache;
        private static ProcessHandler ProcessHandler = null;

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
            _pythonValueCache = new PythonVersionManager.Python();
            _pythonValueCache.isInterrupted = result.isInterrupted; 
            _pythonValueCache.isInstalled = result.isInstalled; 
            _pythonValueCache.version = result.version; 
            _pythonValueCache.path = result.path;

            Form.OnPythonDataRetrieved(result);
        }

        public static void LoadInstallPython()
        {
            string path;
#if DEBUG
            string gitFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            path = Path.Combine(gitFolder, "python-3.12.0-amd64.exe");
#else
            path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Python/Installer/python-3.12.0-amd64.exe");
#endif
            CloseProcess();

            try
            {
                ProcessHandler = new ProcessHandler(path, new EventHandler(OnProcessExited));
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
            string gitFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
            path = Path.Combine(gitFolder, "dvd/main.py");
#else
            path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Python/Build/main.py");
#endif
            Debug.WriteLine("python.exe path: " + _pythonValueCache.path);
            Debug.WriteLine("DVD path: " + path);
            Console.WriteLine("python.exe path: " + _pythonValueCache.path);
            Console.WriteLine("DVD path: " + path);
            Console.WriteLine("Directory.GetCurrentDirectory(): " + Directory.GetCurrentDirectory());

            CloseProcess();

            try
            {
                ProcessHandler = new ProcessHandler(arguments: path, onDone: new EventHandler(OnProcessExited), fileName: _pythonValueCache.path);
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