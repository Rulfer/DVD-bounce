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
        public static EnvironmentManager EnvironmentManager { get; private set; } = new EnvironmentManager();

        internal static ProcessHandler ProcessHandler = null;
        internal static PipHandler PipHandler { get; private set; } = new PipHandler();

        #region Cached data
        public static string PathToPythonAppExecutable = null;
        public static string WorkingDirectory;
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
            PathToPythonAppExecutable = Path.Combine(WorkingDirectory, "dvd/main.py");
#else
            WorkingDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            PathToPythonAppExecutable = Path.Combine(WorkingDirectory, "Python/Build/main.py");
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
            EnvironmentManager?.DeleteCopiedFiles();
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
            EnvironmentManager.RetrieveVersion();
            //LoadPythonProgram();
        }

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

        public static void LoadPythonProgram()
        {
            string path;

            CloseProcess();

            try
            {
                DataReceivedEventHandler onDataReceived = new DataReceivedEventHandler((object sender, DataReceivedEventArgs args) =>
                {
                    if (args.Data != null)
                    {
                        Debug.WriteLine("onDataReceived: " + args.Data);
                    }
                });
                DataReceivedEventHandler onErrorReceived = new DataReceivedEventHandler((object sender, DataReceivedEventArgs args) =>
                {
                    if (args.Data != null)
                    {
                        Debug.WriteLine("onErrorReceived: " + args.Data);
                    }
                });
                ProcessHandler.OptionalData optionalData = new ProcessHandler.OptionalData(onDataReceived: onDataReceived, onErrorReceived: onErrorReceived);

                ProcessHandler = new ProcessHandler(workingDirectory: null, fileName: Path.Combine(EnvironmentManager.EmbeddedPath, "python.exe"), argument: Path.Combine(EnvironmentManager.EmbeddedPath, "main.py"), optionalData: optionalData, onDone: new EventHandler(OnProcessExited));
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