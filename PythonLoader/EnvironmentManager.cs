using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using Windows.Media.Ocr;

namespace Python_Loader
{
    /// <summary>
    /// Ensures embedded python is unzipped and that the content for our Python-app is inside the unzipped folder.
    /// </summary>
    public class EnvironmentManager
    {
        private string _zipPath = "python3.12.4.zip";

        internal string EmbeddedPythonPath { get; private set; }
        internal string PipPath { get; private set; }

        private struct FileCache
        {
            public string Path { get; private set; }
            public bool IsDirectory { get; private set; }
        
            public FileCache(string path, bool isDirectory)
            {
                this.Path = path; 
                this.IsDirectory = isDirectory;
            }
        }

        private List<FileCache> _copiedFiles = new List<FileCache>();

        public async void RetrieveVersion()
        {
            Unzip();
            await PreparePip();
            CopyFiles();

            await AwaitFolderCreation();

            Program.OnEmbeddedPythonReady();
        }

        private async Task AwaitFolderCreation()
        {
            if (!_copiedFiles.Any(x => x.IsDirectory))
                return;


            while(true)
            {
                bool allExists = true;
                for (int i = 0; i < _copiedFiles.Count(x => x.IsDirectory); i++)
                {
                    FileInfo info = new FileInfo(_copiedFiles[i].Path);
                    if (info.Directory == null || !info.Directory.Exists)
                    {
                        Debug.WriteLine(this + " " + info.Directory.Name + " does not exist.");
                        allExists = false;
                        break;
                    }
                }

                if (allExists)
                    break;

                await Task.Yield();
            }

            await Task.Yield();
            Debug.WriteLine(this + " All folders found.");
        }

        public void DeleteCopiedFiles()
        {
            if (_copiedFiles == null || _copiedFiles.Count <= 0)
                return;

            List<FileCache> files = _copiedFiles.Any(x => x.IsDirectory == false) ? _copiedFiles.FindAll(x => x.IsDirectory == false) : null;
            List<FileCache> directories = _copiedFiles.Any(x => x.IsDirectory == true) ? _copiedFiles.FindAll(x => x.IsDirectory == true) : null;

            if(files != null)
            {
                foreach (FileCache file in files)
                {
                    File.Delete(file.Path);
                }
            }
            if (directories != null)
            {
                foreach(FileCache directory in directories)
                {
                    Directory.Delete(directory.Path);
                }
            }
        }

        private void Unzip()
        {
            // TODO: Dynamically download from https://www.python.org/ftp/python/3.12.4/python-3.12.4-embed-amd64.zip so we don't have to manually maintain a copy
            // in both project and build
            string zipOrigin = Path.Combine(Program.WorkingDirectory, _zipPath);

            string zipTarget = Path.Combine(Program.WorkingDirectory, "python3.12.4");
            Console.WriteLine(this + " zipOrigin: " + zipOrigin);
            Console.WriteLine(this + " zipTarget: " + zipTarget);
            if (!Directory.Exists(zipTarget))
            {
                ZipFile.ExtractToDirectory(zipOrigin, zipTarget);
            }

            EmbeddedPythonPath = Path.Combine(zipTarget, "python.exe");
            PipPath = Path.Combine(zipTarget, "Scripts\\pip.exe");
        }

        #region Pip
        /// <summary>
        /// Ensure that Pip is downloaded and ready to be used in the directory it currently resides.
        /// </summary>
        /// <returns></returns>
        private async Task PreparePip()
        {
            FileInfo pipInfo = new FileInfo(PipPath);
            //if (!pipInfo.Exists)
            //{
            //    // Download and create Pip.
            //    Debug.WriteLine(this + " Pip not installed");
            //    await DownloadPip();
            //}

            // Verify installation
            Debug.WriteLine(this + " Verify pip installation");
            bool result = await VerifyPipInstallation();

            if (!result) 
            {
                Debug.WriteLine(this + " Pip not installed.");
                bool hasError = true;
                while(hasError)
                {
                    result = await DownloadPip();
                    hasError = result;
                }
            }
        }

        private async Task<bool> DownloadPip()
        {
            bool isDone = false;
            bool hasError = false;
            EventHandler onDone = new EventHandler((sender, args) =>
            {
                isDone = true;
            });
            DataReceivedEventHandler onDataReceived = new DataReceivedEventHandler((sender, args) =>
            {
                if (string.IsNullOrEmpty(args.Data))
                    return;
                Debug.WriteLine(this + " onDataReceived: " + args.Data);
                hasError = false;
            });
            DataReceivedEventHandler onErrorReceived = new DataReceivedEventHandler((sender, args) =>
            {
                if (string.IsNullOrEmpty(args.Data))
                    return;
                Debug.WriteLine(this + " onErrorReceived: " + args.Data);
                hasError = true;
            });

            ProcessHandler.OptionalData optionalParameters = new ProcessHandler.OptionalData(
                onDataReceived: onDataReceived,
                onErrorReceived: onErrorReceived);
            Program.ProcessHandler = new ProcessHandler(fileName: EmbeddedPythonPath, workingDirectory: null, onDone: onDone, argument: "-m get-pip", optionalParameters);

            while(!isDone)
            {
                await Task.Delay(100);
            }

            if(hasError)
            {
                MessageBox.Show("Pip failed to install. I will keep retrying until it works.");
            }

            return hasError;
        }

        private async Task<bool> VerifyPipInstallation()
        {
            FileInfo pipInfo = new FileInfo(PipPath);
            if (!pipInfo.Exists)
                return false;

            bool isDone = false;
            bool isInstalled = false;

            EventHandler onDone = new EventHandler((sender, args) =>
            {
                isDone = true;
            });
            DataReceivedEventHandler onDataReceived = new DataReceivedEventHandler((sender, args) =>
            {
                if (string.IsNullOrEmpty(args.Data))
                    return;
                Debug.WriteLine(this + " onDataReceived: " +  args.Data);
                isInstalled = true;
            });

            ProcessHandler.OptionalData optionalParameters = new ProcessHandler.OptionalData(
                onDataReceived: onDataReceived);
            Program.ProcessHandler = new ProcessHandler(fileName: PipPath, workingDirectory: null, onDone: onDone, argument: "list", optionalParameters);

            while(isDone == false)
            {
                // Wait for a bit and then check again.
                await Task.Delay(100);
            }

            await Task.Yield();
            Debug.WriteLine(this + $" Pip is {(isInstalled ? "installed" : "not installed")}");
            return isInstalled;
        }

        #endregion
        private void CopyFiles()
        {
            string pythonEmbedFolder = new FileInfo(EmbeddedPythonPath).Directory.FullName;
            string pythonAppFolder = new FileInfo(Program.PathToPythonAppExecutable).Directory.FullName;
            Debug.WriteLine(this + " pythonEmbedFolder: " + pythonEmbedFolder);
            Debug.WriteLine(this + " pythonAppFolder: " + pythonAppFolder);
            CopyRecursive(pythonAppFolder, pythonEmbedFolder);
        }

        private void CopyRecursive(string sourceDir, string targetDir)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.TopDirectoryOnly))
            {
                FileInfo fileInfo = new FileInfo(dirPath);
                if (fileInfo.Name == ".idea")
                    continue;
                if (fileInfo.Name == "__pycache__")
                    continue;
                string newDir = Path.Combine(targetDir, fileInfo.Name);
                if(!Directory.Exists(newDir))
                    Directory.CreateDirectory(newDir);
                _copiedFiles.Add(new FileCache(path: newDir, isDirectory: true));
                CopyRecursive(Path.Combine(sourceDir, fileInfo.Name), Path.Combine(targetDir, fileInfo.Name));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string origin in Directory.GetFiles(sourceDir, "*.*", SearchOption.TopDirectoryOnly))
            {
                FileInfo fileInfo = new FileInfo(origin);

                string target = Path.Combine(targetDir, fileInfo.Name);
                Debug.WriteLine(this + " file origin: " + origin);
                Debug.WriteLine(this + " file destination: " + target);
                //File.Copy(origin, Path.Combine(targetDir, fileInfo.Name), true);
                File.Copy(origin, target, true);

                _copiedFiles.Add(new FileCache(path: target, isDirectory: false));
            }
        }
    }
}
