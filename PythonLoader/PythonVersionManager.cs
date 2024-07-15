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
using System.Drawing.Text;
using Python_Loader.Helpers;

namespace Python_Loader
{
    public class PythonVersionManager
    {
        private CancellationTokenSource _source;

        private const int REQUIRED_VERSION = 312;
        private Helpers.Version RequiredPythonVersion = new Helpers.Version("3.12.3");

        public struct Python
        {
            public bool isInstalled;
            /// <summary>
            /// True if the thread was closed.
            /// </summary>
            internal Helpers.Version version;
            internal bool isReady;
        }

        /// <summary>
        /// If this is false then Windows tried to open the Windows App Store instead of returning any data, indicating Python isn't added to PATH.
        /// </summary>
        private string _receivedData = null;

        private string _zipPath = "EmbeddedPython/python3.12.4.zip";

        internal string EmbeddedPath { get; private set; }

        public void RetrieveVersion()
        {
            // TODO: Dynamically download from https://www.python.org/ftp/python/3.12.4/python-3.12.4-embed-amd64.zip so we don't have to manually maintain a copy
            // in both project and build
            string workingDir;
#if DEBUG
            workingDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
#else
            workingDir = Directory.GetCurrentDirectory();
#endif

            string zipOrigin = Path.Combine(workingDir, _zipPath);
            string zipTarget = Path.Combine(workingDir, "EmbeddedPython/Python3.12.4");
            Console.WriteLine(this + " zipOrigin: " + zipOrigin);
            Console.WriteLine(this + " zipTarget: " + zipTarget);
            if(!Directory.Exists(zipTarget))
            {
                ZipFile.ExtractToDirectory(zipOrigin, zipTarget);
            }

            //EmbeddedPath = Path.Combine(zipTarget, "py");
            EmbeddedPath = zipTarget;
            Program.OnEmbeddedPythonReady();
        }

        //public void RetrieveVersion()
        //{
        //    ProcessHandler.OptionalData optionalData = new ProcessHandler.OptionalData(
        //        onDataReceived: new DataReceivedEventHandler(OnDataReceived), 
        //        onErrorReceived: new DataReceivedEventHandler(OnErrorReceived),
        //        arguments: "-3 --version");
        //    Program.ProcessHandler = new ProcessHandler(fileName: "py", onDone: new EventHandler(OnDone), optionalData: optionalData);
        //}

        //private void OnDone(object? sender, EventArgs e)
        //{
        //    Debug.WriteLine(this + " OnDone: " + (_receivedData == null ? "No data received" : _receivedData));
        //    Python result = new Python();
        //    result.isInstalled = _receivedData != null && _receivedData.Length > 0;
        //    result.version = GetVersion(_receivedData);

        //    if(!result.isInstalled)
        //    {
        //        // Require installations
        //        MessageBox.Show("Python not added to PATH (Environmental variables). Please add it to PATH, or start a new installation using this program.");
        //        result.isReady = false;
        //    }
        //    else if(!result.version.IsHigherVersion(RequiredPythonVersion, out bool isEqual))
        //    {
        //        result.isReady = isEqual;

        //        // Update required, unless isEqual is true.
        //        if (!isEqual)
        //        {
        //            MessageBox.Show("Python requires an update.\nPlease install the update via this program.");
        //            result.isReady = false;
        //        }
        //    }
        //    else
        //    {
        //        // Python is installed and up to date.
        //        result.isReady = true;
        //    }

        //    Program.OnPythonLocated(result);
        //}

        //private void OnDataReceived(object sender, DataReceivedEventArgs e)
        //{
        //    // Maybe called? Not on windows 11 at least.
        //    if(e.Data == null || e.Data.Length <= 0)
        //        return;

        //    _receivedData = e.Data;
        //    Debug.WriteLine(this + " OnDataReceived: " + e.Data);
        //}

        //private void OnErrorReceived(object sender, DataReceivedEventArgs e)
        //{
        //    // Called when python is installed.
        //    if(e.Data == null || e.Data.Length <= 0)
        //        return;

        //    _receivedData = e.Data;
        //    Debug.WriteLine(this + " OnErrorReceived: " + e.Data);
        //}

        //private Helpers.Version GetVersion(string input)
        //{
        //    if (input == null || !input.Contains(" ") || !input.Contains("."))
        //        return new Helpers.Version();

        //    // Result is expected to be Python X.Y.Z
        //    string version = input.Split(" ").Last();
        //    return new Helpers.Version(version);
        //}
    }
}
