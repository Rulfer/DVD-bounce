using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Python_Loader
{

    internal class ProcessHandler
    {
        internal static ProcessHandler Instance = null;

        /// <summary>
        /// Includes optional parameters for the ProcessHandler.
        /// </summary>
        internal class OptionalData
        {
            /// <summary>
            /// This will set the <see cref="ProcessStartInfo.UseShellExecute"/> value.
            /// </summary>
            internal bool UseShellExecute { get; set; } = false;
            /// <summary>
            /// This will set the <see cref="ProcessStartInfo.CreateNoWindow"/> value.
            /// </summary>
            internal bool CreateNoWindow { get; set; } = true;
            internal DataReceivedEventHandler onDataReceived = null;
            internal DataReceivedEventHandler onErrorReceived = null;

            internal OptionalData(DataReceivedEventHandler onDataReceived = null, DataReceivedEventHandler onErrorReceived = null,
                bool UseShellExecute = false, bool CreateNoWindow = true) 
            {
                this.onDataReceived = onDataReceived;
                this.onErrorReceived = onErrorReceived;
                this.UseShellExecute = UseShellExecute;
                this.CreateNoWindow = CreateNoWindow;
            }
        }


        /// <summary>
        /// If true then this will quit the ongoing process when the app quits.
        /// </summary>
        internal bool InterruptOnAppQuit { get; set; } = false;

        /// <summary>
        /// Currently running Process, if any.
        /// </summary>
        public Process Process { get; private set; } = null;

        private EventHandler OnExit = null;

        internal ProcessHandler(string fileName, string workingDirectory, EventHandler onDone, string argument = null, OptionalData optionalData = null)
        {
            if (Instance != null) 
                Instance.Terminate();

            Instance = this;
            OnExit = onDone;

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = fileName;
            if(workingDirectory != null)
                info.WorkingDirectory = workingDirectory;
            info.UseShellExecute = optionalData?.UseShellExecute ?? info.UseShellExecute;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.CreateNoWindow = optionalData?.CreateNoWindow ?? info.CreateNoWindow;
            if(argument != null)
                info.Arguments = argument;
            Process = new Process();
            Process.StartInfo = info;
            Process.EnableRaisingEvents = true;
            Process.Exited += new EventHandler(OnProcessDone);

            if (optionalData?.onDataReceived != null)
            {
                Process.OutputDataReceived += new DataReceivedEventHandler(optionalData.onDataReceived);
                Debug.WriteLine(this + " subscribed to Process.OutputDataReceived");
            }
            if (optionalData?.onErrorReceived != null)
            {
                Process.ErrorDataReceived += new DataReceivedEventHandler(optionalData.onErrorReceived);
                Debug.WriteLine(this + " subscribed to Process.ErrorDataReceived");
            }

            OnExit = onDone;

            Debug.WriteLine(this + " FileName: " + Process.StartInfo.FileName);
            Debug.WriteLine(this + " WorkingDirectory: " + Process.StartInfo.WorkingDirectory);
            Debug.WriteLine(this + " Arguments: " + Process.StartInfo.Arguments);

            Console.WriteLine(this + " FileName: " + Process.StartInfo.FileName);
            Console.WriteLine(this + " WorkingDirectory: " + Process.StartInfo.WorkingDirectory);
            Console.WriteLine(this + " Arguments: " + Process.StartInfo.Arguments);

            Process.Start();


            Process?.BeginOutputReadLine();
            Process?.BeginErrorReadLine();
        }

        private void OnProcessDone(object sender, System.EventArgs args)
        {
            Terminate();
            OnExit.Invoke(sender, args);
        }

        /// <summary>
        /// Terminate the ongoing process for the active ProcessHandler Instance.
        /// </summary>
        public void Terminate()
        {
            if (Process == null)
                return;

            if(OnExit != null)
                Process.Exited -= OnExit;
            Process.Kill();
            Process = null;
        }
    }
}
