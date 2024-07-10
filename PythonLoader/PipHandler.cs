using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;

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
            public Version? LocalVersion;

            public Package(string Name, string Version)
            {
                this.Name = Name;
                this.ExpectedVersion = new Version(ConvertVersion(Version));
            }

            public void SetLocalVersion(string version)
            {
                this.LocalVersion = new Version(ConvertVersion(version));
            }

            public bool UpdateRequired(out string optionalParameter)
            {
                if (!LocalVersion.HasValue)
                {
                    optionalParameter = null;
                    return true;
                }

                optionalParameter = "--upgrade";

                if (LocalVersion.Value.Major < ExpectedVersion.Major)
                    return true;
                if (LocalVersion.Value.Minor < ExpectedVersion.Minor)
                    return true;
                if (LocalVersion.Value.Patch < ExpectedVersion.Patch)
                    return true;

                return false;
            }
        }

        private struct Version
        {
            public int Major;
            public int Minor;
            public int Patch;

            public Version((int major, int minor, int patch) version)
            {
                this.Major = version.major;
                this.Minor = version.minor;
                this.Patch = version.patch;
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
            string pythonExePath = Program.PythonValueCache.path;
            ProcessHandler.OptionalData optionalParameters = new ProcessHandler.OptionalData(
                arguments: "-m pip list",
                onDataReceived: OnDataReceived,
                onErrorReceived: OnErrorReceived);
            Debug.WriteLine(this + $" pythonExePath is {pythonExePath}");
            Debug.WriteLine(this + $" arguments are {optionalParameters.Arguments}");
            Program.ProcessHandler = new ProcessHandler(pythonExePath, OnProcessClosed, optionalParameters);
        }

        #region Retrieve installations
        private void OnProcessClosed(object sender, EventArgs e)
        {
            string optionalParameter = "";

            foreach (Package package in _packages)
            {
                Debug.WriteLine(this + $" Package: {package.Name}=(Local: {package.LocalVersion.Value.Major}.{package.LocalVersion.Value.Minor}.{package.LocalVersion.Value.Patch}), " +
                    $"(Expected: {package.ExpectedVersion.Major}.{package.ExpectedVersion.Minor}.{package.ExpectedVersion.Patch})");
            }

            int reference = Array.FindIndex(_packages, x => x.UpdateRequired(out optionalParameter));

            if (reference != -1)
            {
                // Update required packages. We just start from the bottom of the array and move upwards.
                // Add whitespace in case there are parameters
                optionalParameter = optionalParameter != null ? optionalParameter + " " : "";
                string argument = "-m pip install " + optionalParameter + "" + _packages[reference].Name;
                Debug.WriteLine(this + " Install package with argument: " + argument);

                string pythonExePath = Program.PythonValueCache.path;
                ProcessHandler.OptionalData optionalParameters = new ProcessHandler.OptionalData(
                    arguments: argument,
                    onDataReceived: OnUpdateDataReceived,
                    onErrorReceived: OnUpdateErrorReceived);
                Debug.WriteLine(this + $" pythonExePath is {pythonExePath}");
                Debug.WriteLine(this + $" arguments are {optionalParameters.Arguments}");
                Program.ProcessHandler = new ProcessHandler(pythonExePath, OnUpdadeProcessDone, optionalParameters);
            }
            else
            {
                Program.Form.OnPythonClosed();
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

        }

        private void OnUpdateErrorReceived(object sender, DataReceivedEventArgs outLine)
        {
            if (string.IsNullOrEmpty(outLine.Data))
                return;
            Debug.WriteLine(this + $" OnErrorReceived(): {outLine.Data}");
        }
        #endregion

        static internal (int major, int minor, int patch) ConvertVersion(string version)
        {
            if (!version.Contains("."))
            {
                // Assume the package only contains a major version.
                return (int.Parse(version), 0, 0);
            }
            
            string[] parts = version.Split('.');
            int major = int.Parse(parts[0]);
            int minor = parts.Length > 1 ? int.Parse(parts[1]) : 0;
            int patch = parts.Length > 2 ? int.Parse(parts[2]) : 0;

            return (major, minor, patch);
        }
    }
}
