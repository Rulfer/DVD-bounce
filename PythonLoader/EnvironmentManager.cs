using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;

namespace Python_Loader
{
    /// <summary>
    /// Ensures embedded python is unzipped and that the content for our Python-app is inside the unzipped folder.
    /// </summary>
    public class EnvironmentManager
    {
        private string _zipPath = "EmbeddedPython/python3.12.4.zip";

        internal string EmbeddedPath { get; private set; }

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
            string workingDir;
#if DEBUG
            workingDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
#else
            workingDir = Directory.GetCurrentDirectory();
#endif

            string zipOrigin = Path.Combine(workingDir, _zipPath);
            string zipTarget = Path.Combine(workingDir, "EmbeddedPython\\Python3.12.4");
            Console.WriteLine(this + " zipOrigin: " + zipOrigin);
            Console.WriteLine(this + " zipTarget: " + zipTarget);
            if (!Directory.Exists(zipTarget))
            {
                ZipFile.ExtractToDirectory(zipOrigin, zipTarget);
            }

            EmbeddedPath = zipTarget;
        }

        private void CopyFiles()
        {
            string appFolder = Directory.GetParent(Program.PathToPythonAppExecutable).FullName;
            //CopyFilesRecursively(appFolder, EmbeddedPath);
            CopyRecursive(appFolder, EmbeddedPath);
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

                string newDir = Directory.CreateDirectory(Path.Combine(targetDir, fileInfo.Name)).FullName;
                _copiedFiles.Add(new FileCache(path: newDir, isDirectory: true));
                CopyRecursive(Path.Combine(sourceDir, fileInfo.Name), Path.Combine(targetDir, fileInfo.Name));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourceDir, "*.*", SearchOption.TopDirectoryOnly))
            {
                FileInfo fileInfo = new FileInfo(newPath);

                //string fileName = newPath.Replace(sourcePath, "");
                string newTarget = Path.Combine(targetDir, fileInfo.Name);
                //Debug.WriteLine(this + " fileName: " + fileName);
                Debug.WriteLine(this + " newTarget: " + newTarget);
                Debug.WriteLine(this + " newPath: " + newPath);
                //File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
                File.Copy(newPath, Path.Combine(targetDir, fileInfo.Name), true);

                _copiedFiles.Add(new FileCache(path: newTarget, isDirectory: false));
            }
        }

        //private void CopyFilesRecursively(string sourcePath, string targetPath)
        //{
        //    //Now Create all of the directories
        //    foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        //    {
        //        FileInfo fileInfo = new FileInfo(dirPath);
        //        if (fileInfo.Name == ".idea")
        //            continue;
        //        if (fileInfo.Name == "__pycache__")
        //            continue;



        //        string newDir = Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath)).FullName;
        //        _copiedFiles.Add(new FileCache(path: newDir, isDirectory: true));
        //    }

        //    //Copy all the files & Replaces any files with the same name
        //    foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        //    {
        //        string fileName = newPath.Replace(sourcePath, "");
        //        string newTarget = Path.Combine(sourcePath, fileName);
        //        Debug.WriteLine(this + " fileName: " + fileName);
        //        Debug.WriteLine(this + " newTarget: " + newTarget);
        //        Debug.WriteLine(this + " newPath: " + newPath);
        //        //File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        //        File.Copy(newPath, newTarget, true);
                
        //        _copiedFiles.Add(new FileCache(path: newTarget, isDirectory: false));
        //    }
        //}
    }
}
