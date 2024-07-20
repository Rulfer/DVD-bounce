using Python_Loader.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Version = Python_Loader.Helpers.Version;

namespace Python_Loader
{
    /// <summary>
    /// Identify installed packages, keep them up to date, and ensure missing packages are installed. 
    /// </summary>
    internal class PipHandler
    {
        private class Package
        {
            public string Name;
            public Version ExpectedVersion;

            /// <summary>
            /// If this is null then no local version could be found.
            /// </summary>
            public Version LocalVersion;

            public Package(string Name, string ExpectedVersion)
            {
                this.Name = Name;
                this.ExpectedVersion = new Version(ExpectedVersion);
            }

            public void SetLocalVersion(string LocalVersion)
            {
                this.LocalVersion = new Version(LocalVersion);
            }

            public bool UpdateRequired(out string optionalParameter)
            {
                if (LocalVersion == null)
                {
                    optionalParameter = null;
                    return true;
                }

                optionalParameter = "--upgrade";

                bool outdated = ExpectedVersion.IsHigherVersion(LocalVersion, out bool isEqual);

                if (outdated && isEqual)
                    outdated = false;

                return outdated;
            }
        }

        private Package[] _packages =
        {
            new Package("pip", "24.0"),
            new Package("pillow", "10.4.0")
        };

        /// <summary>
        /// Update existing packages and install missing.
        /// </summary>
        internal void Update()
        {
            Debug.WriteLine(this + " Update()");
            Console.WriteLine(this + " Update()");
            ProcessHandler.OptionalData optionalParameters = new ProcessHandler.OptionalData(
                onDataReceived: OnDataReceived,
                onErrorReceived: OnErrorReceived);
            //Program.ProcessHandler = new ProcessHandler(fileName: Path.Combine(Program.EnvironmentManager.EmbeddedPythonPath, "Scripts\\pip.exe"), workingDirectory: null, OnProcessClosed, argument: "list", optionalParameters);
            Program.ProcessHandler = new ProcessHandler(fileName: Program.EnvironmentManager.PipPath, workingDirectory: null, OnProcessClosed, argument: "list", optionalParameters);
        }

        #region Retrieve installations
        private void OnProcessClosed(object sender, EventArgs e)
        {
            string optionalParameter = "";

            foreach (Package package in _packages)
            {
                if (package.LocalVersion == null)
                {
                    Debug.WriteLine(this + $" Package: {package.Name}=(NOT INSTALLED), " +
                        $"(Expected: {package.ExpectedVersion.Major}.{package.ExpectedVersion.Minor}.{package.ExpectedVersion.Patch})");
                    Console.WriteLine(this + $" Package: {package.Name}=(NOT INSTALLED), " +
                        $"(Expected: {package.ExpectedVersion.Major}.{package.ExpectedVersion.Minor}.{package.ExpectedVersion.Patch})");
                }

                else
                {
                    Debug.WriteLine(this + $" Package: {package.Name}=(Local: {package.LocalVersion.Major}.{package.LocalVersion.Minor}.{package.LocalVersion.Patch}), " +
                        $"(Expected: {package.ExpectedVersion.Major}.{package.ExpectedVersion.Minor}.{package.ExpectedVersion.Patch})");
                    Console.WriteLine(this + $" Package: {package.Name}=(Local: {package.LocalVersion.Major}.{package.LocalVersion.Minor}.{package.LocalVersion.Patch}), " +
                        $"(Expected: {package.ExpectedVersion.Major}.{package.ExpectedVersion.Minor}.{package.ExpectedVersion.Patch})");
                }
            }
            int reference = Array.FindIndex(_packages, x => x.UpdateRequired(out optionalParameter));

            if (reference != -1)
            {
                // Update required packages. We just start from the bottom of the array and move upwards.
                // Add whitespace in case there are parameters
                optionalParameter = optionalParameter != null ? optionalParameter + " " : "";
                string argument = "install " + optionalParameter + "" + _packages[reference].Name;
                Debug.WriteLine(this + " Install package with argument: " + argument);
                Console.WriteLine(this + " Install package with argument: " + argument);

                ProcessHandler.OptionalData optionalParameters = new ProcessHandler.OptionalData(
                    onDataReceived: OnUpdateDataReceived,
                    onErrorReceived: OnUpdateErrorReceived,
                    CreateNoWindow: true);
                //Program.ProcessHandler = new ProcessHandler(fileName: Path.Combine(Program.EnvironmentManager.EmbeddedPythonPath, "Scripts\\pip.exe"), workingDirectory: null, OnProcessClosed, argument: argument, optionalParameters);
                Program.ProcessHandler = new ProcessHandler(fileName: Program.EnvironmentManager.PipPath, workingDirectory: null, OnProcessClosed, argument: argument, optionalParameters);

            }
            else
            {
                Program.Form.OnPipReady();
            }
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs outLine)
        {
            if (string.IsNullOrEmpty(outLine.Data))
                return;
            Debug.WriteLine(this + " OnDataReceived: " + outLine.Data);
            string[] split = outLine.Data.Split().Where(x => x != string.Empty).ToArray(); 
            string package = split[0];
            string version = split[1];

            int reference = Array.FindIndex(_packages, x => x.Name == package);
            if (reference == -1)
                return;

            _packages[reference].SetLocalVersion(version);
        }

        private void OnErrorReceived(object sender, DataReceivedEventArgs outLine)
        {
            if (string.IsNullOrEmpty(outLine.Data))
                return;
            Debug.WriteLine(this + $" OnErrorReceived(): {outLine.Data}");
        }
        #endregion

        #region Update packages
        private void OnUpdadeProcessDone(object sender, EventArgs e)
        {
            Update();
        }

        private void OnUpdateDataReceived(object sender, DataReceivedEventArgs outLine)
        {
            if (string.IsNullOrEmpty(outLine.Data))
                return;
            Debug.WriteLine(this + $" OnUpdateDataReceived(): {outLine.Data}");
            Console.WriteLine(this + $" OnUpdateDataReceived(): {outLine.Data}");

            string output = outLine.Data;
            if (output.ToLower().Contains("successfully installed"))
            {
                output = output.Replace("Successfully installed", "");
                output = output.Replace(" ", "");
                string packageName = output.Split("-").First();
                string version = output.Split("-").Last();
                int reference = Array.FindIndex(_packages, x => x.Name == packageName);
                _packages[reference].SetLocalVersion(version);
            }
        }

        private void OnUpdateErrorReceived(object sender, DataReceivedEventArgs outLine)
        {
            if (string.IsNullOrEmpty(outLine.Data))
                return;
            Debug.WriteLine(this + $" OnErrorReceived(): {outLine.Data}");
            Console.WriteLine(this + $" OnErrorReceived(): {outLine.Data}");
        }
        #endregion
    }
}
