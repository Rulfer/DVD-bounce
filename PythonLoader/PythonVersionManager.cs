using System;
using System.Collections.Generic;
using System.IO.Compression;

namespace Python_Loader
{
    public class PythonVersionManager
    {
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

            EmbeddedPath = zipTarget;
            Program.OnEmbeddedPythonReady();
        }
    }
}
