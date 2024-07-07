using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;

namespace Python_Loader
{
    public class PythonVersionManager
    {
        private CancellationTokenSource _source;

        private const int REQUIRED_VERSION = 312;

        public struct Python
        {
            public bool isInstalled;
            /// <summary>
            /// True if the thread was closed.
            /// </summary>
            public bool isInterrupted;
            public long version;
            public string path;
        }

        public async void RetrieveVersion()
        {
            _source = new CancellationTokenSource();
            Python result = await Task.Run(() => IsInstalled(_source.Token));
            
            Program.Form.OnPythonDataRetrieved(result);
            //Program.LoadPythonProgram(result.path);
        }

        private string PathToExe(string version)
        {
            string localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string subFolder = $"Python/Python{version}/python.exe";
            return Path.Combine(localAppDataFolder, subFolder);
        }

        private string FindPythonExecutable()
        {
            string pythonKey = @"SOFTWARE\Python\PythonCore";

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(pythonKey))
            {
                if (key != null)
                {
                    string[] subKeyNames = key.GetSubKeyNames();

                    foreach (string subKeyName in subKeyNames)
                    {
                        Debug.WriteLine("subKeyName: " + subKeyName);
                        Console.WriteLine("subKeyName: " + subKeyName);
                        using (RegistryKey subKey = key.OpenSubKey(subKeyName))
                        {
                            string installPath = subKey?.GetValue("InstallPath")?.ToString();
                            Debug.WriteLine("installPath: "+ installPath);
                            Console.WriteLine("installPath: "+ installPath);
                            if (!string.IsNullOrEmpty(installPath))
                            {
                                string pythonExePath = Path.Combine(installPath, "python.exe");
                                if (File.Exists(pythonExePath))
                                {
                                    return pythonExePath;
                                }
                            }
                        }
                    }
                }
            }

            // Python executable not found
            MessageBox.Show("Python executable not found.");
            return null;
        }

        private async Task<Python> IsInstalled(CancellationToken token)
        {
            Program.Form.txtStatus.Invoke((MethodInvoker)delegate { Program.Form.txtStatus.Text = "Identifying Python"; });
            // Program.Form.txtStatus.Text = "Identifying Python";
            Python result = new Python();
            result.isInstalled = false;
            result.isInterrupted = false;
            try
            {
                // Create a new management scope and connect to the local computer
                ManagementScope scope = new ManagementScope(@"\\.\root\cimv2");
                scope.Connect();

                // Create a new query for installed products
                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Product WHERE Name LIKE 'Python%'");

                // Execute the query
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                // Get the results
                ManagementObjectCollection queryCollection = searcher.Get();

                if(queryCollection == null || queryCollection.Count <= 0)
                {
                    Console.WriteLine("Query is empty.");
                    result.isInstalled = false;
                    return result;
                }

                // Iterate through the results and display information about each installed program
                foreach (ManagementObject m in queryCollection)
                {
                    string rawVersion = (string)m.GetPropertyValue("Version");
                    if (!rawVersion.Contains("."))
                        continue;


                    string[] versions = rawVersion.Split(".");
                    long version = int.Parse(versions[0] + versions[1]);

                    result.version = version;
                    result.path = PathToExe(version.ToString());
                    return result;

                }

                return result;
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Cancelled python installed checked.");
                Console.WriteLine("Cancelled python installed checked.");
                result.isInterrupted = true;
                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine("An error occurred: " + e.Message);
                Console.WriteLine("An error occurred: " + e.Message);
                return result;
            }
        }
    }
}
