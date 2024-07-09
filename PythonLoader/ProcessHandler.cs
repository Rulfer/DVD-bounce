using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email.DataProvider;

namespace Python_Loader
{
    internal class ProcessHandler
    {
        public static ProcessHandler Instance;

        /// <summary>
        /// If true then this will quit the ongoing process when the app quits.
        /// </summary>
        public bool InterruptOnAppQuit { get; set; } = false;

        /// <summary>
        /// Currently running Process, if any.
        /// </summary>
        public Process Process { get; private set; } = null;

        private EventHandler OnExit = null;

 
        public ProcessHandler(string fileName, EventHandler onDone, string arguments = null)
        {
            if (Instance != null) 
                Instance.Terminate();

            Instance = this;
            OnExit = onDone;

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = fileName;
            if(arguments != null)
                info.Arguments = arguments;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.CreateNoWindow = true;

            Process = new Process();
            Process.StartInfo = info;
            Process.EnableRaisingEvents = true;
            Process.Exited += new EventHandler(OnProcessDone);

            OnExit = onDone;

            Process.Start();
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
