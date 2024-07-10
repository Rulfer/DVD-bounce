using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Python_Loader
{
    /// <summary>
    /// Identify installed packages, keep them up to date, and ensure missing packages are installed. 
    /// </summary>
    internal class PipHandler
    {
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
            new ProcessHandler(pythonExePath, OnProcessClosed, optionalParameters);
        }

        private void OnProcessClosed(object sender, EventArgs e)
        {
            Debug.WriteLine(this + " OnProcessClosed()");

        }

        private void OnDataReceived(object sender, DataReceivedEventArgs outLine)
        {
            if (string.IsNullOrEmpty(outLine.Data))
                return;
            Debug.WriteLine(this + " OnDataReceived()");
            Debug.WriteLine(outLine.Data);
        }

        private void OnErrorReceived(object sender, DataReceivedEventArgs outLine)
        {
            if (string.IsNullOrEmpty(outLine.Data))
                return;
            Debug.WriteLine(this + $" OnErrorReceived(): {outLine.Data}");
        }
    }
}
