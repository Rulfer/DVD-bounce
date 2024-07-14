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
using System.IO;
using System.IO.Compression;

namespace Python_Loader
{
    public class PythonVersionManager
    {
        private CancellationTokenSource _source;

        private const int REQUIRED_VERSION = 312;
        private const string _pythonZipPath = "PythonVersions/python3.12.4.zip";

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
        // How to extract zip files
        https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-compress-and-extract-files

            string workingDir;

#if DEBUG
            workingDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
#else
            workingDir = Directory.GetCurrentDirectory().FullName;
#endif

            string zipPath = Path.Combine(workingDir, _pythonZipPath);
            string zipTarget = Path.Combine(workingDir, "PythonVersions/Python3.12.4");
            ZipFile.ExtractToDirectory(zipPath, zipTarget);

            _source = new CancellationTokenSource();
            Python result = await Task.Run(() => IsInstalled(_source.Token));
            
            Program.OnPythonDataRetrieved(result);
            //Program.LoadPythonProgram(result.path);
        }

        private string PathToExe(string version)
        {
            string localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string subFolder = $"Programs/Python/Python{version}/python.exe";
            return Path.Combine(localAppDataFolder, subFolder);
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

                    result.isInstalled = true;
                    result.version = version;
                    result.path = PathToExe(version.ToString());
                    Console.WriteLine("Version: " + result.version);
                    Console.WriteLine("path: " + result.path);
                    return result;

                }


                // Additional check for Python installations via Microsoft App Store
                string appStorePythonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "WindowsApps");
                if (Directory.Exists(appStorePythonPath))
                {
                    var pythonExes = Directory.GetFiles(appStorePythonPath, "python*.exe");
                    if (pythonExes.Length > 0)
                    {
                        result.isInstalled = true;
                        result.path = pythonExes.First(); // Take the first Python executable found
                        string versionOutput = await GetPythonVersion(result.path, token);
                        if (!string.IsNullOrEmpty(versionOutput))
                        {
                            string[] versions = versionOutput.Split(".");
                            long version = int.Parse(versions[0] + versions[1]);
                            result.version = version;
                        }
                        return result;
                    }
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

        private static async Task<string> GetPythonVersion(string pythonPath, CancellationToken token)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    string output = await process.StandardOutput.ReadToEndAsync();
                    process.WaitForExit();
                    return output.Replace("Python ", "").Trim();
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
