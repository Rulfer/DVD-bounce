using Python_Loader.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
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

            public Package(string Name, string Version)
            {
                this.Name = Name;
                this.ExpectedVersion = new Version(Version);
            }

            public void SetLocalVersion(string version)
            {
                this.LocalVersion = new Version(version);
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

                //if (LocalVersion.Major < ExpectedVersion.Major)
                //    return true;
                //if (LocalVersion.Minor < ExpectedVersion.Minor)
                //    return true;
                //if (LocalVersion.Patch < ExpectedVersion.Patch)
                //    return true;

                //return false;
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
            //string pythonExePath = Program.PythonExePath;
            ProcessHandler.OptionalData optionalParameters = new ProcessHandler.OptionalData(
                onDataReceived: OnDataReceived,
                onErrorReceived: OnErrorReceived);
            //Debug.WriteLine(this + $" pythonExePath is {pythonExePath}");
            //Debug.WriteLine(this + $" arguments are {optionalParameters.Arguments}");
            //Program.ProcessHandler = new ProcessHandler(fileName: "py"", OnProcessClosed, optionalParameters);
            Program.ProcessHandler = new ProcessHandler(fileName: "py", workingDirectory: Program.PythonVersionManager.EmbeddedPath, OnProcessClosed, argument: "-m pip list", optionalParameters);
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
                string argument = "-m pip install " + optionalParameter + "" + _packages[reference].Name;
                Debug.WriteLine(this + " Install package with argument: " + argument);
                Console.WriteLine(this + " Install package with argument: " + argument);

                //string pythonExePath = Program.PythonValueCache.path;
                ProcessHandler.OptionalData optionalParameters = new ProcessHandler.OptionalData(
                    onDataReceived: OnUpdateDataReceived,
                    onErrorReceived: OnUpdateErrorReceived,
                    CreateNoWindow: false);
                //Debug.WriteLine(this + $" pythonExePath is {pythonExePath}");
                //Debug.WriteLine(this + $" arguments are {optionalParameters.Arguments}");
                //Program.ProcessHandler = new ProcessHandler(fileName: "py", onDone: OnUpdadeProcessDone, optionalData: optionalParameters);
                Program.ProcessHandler = new ProcessHandler(fileName: "py", workingDirectory: Program.PythonVersionManager.EmbeddedPath, argument: argument, onDone: OnUpdadeProcessDone, optionalData: optionalParameters);
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
