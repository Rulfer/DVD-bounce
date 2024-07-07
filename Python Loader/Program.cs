using System.Diagnostics;
using System.Windows.Forms;
using System;
using System.Management;

namespace Python_Loader
{
    public static class Program
    {
        public static Form1 Form { get; private set; } = null;
        public static PythonVersionManager PythonVersionManager { get; private set; } = new PythonVersionManager();

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
            var projectFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            Debug.WriteLine(projectFolder);
            Console.WriteLine(projectFolder);
            Form = new Form1();
            Application.Run(Form);

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

        public static void LoadPythonProgram(string exePath)
        {
            string path = @"F:\PersonligeProsjekter\DVD-bounce\dvd\main.py";

            try
            {
                // Create a new process to start the Python script
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "python.exe"; // If python is added to PATH
                psi.Arguments = $"\"{path}\"";
                //psi.FileName = path;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.CreateNoWindow = true;

                // Start the process
                using (Process process = Process.Start(psi))
                {
                    // Read output streams
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    // Wait for the process to exit
                    process.WaitForExit();

                    // Display output and error messages
                    MessageBox.Show("Output: " + output + "\nError: " + error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}