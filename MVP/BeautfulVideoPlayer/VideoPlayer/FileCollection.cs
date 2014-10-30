namespace IncrementalLoadingSample
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using IncrementalLoadingSample.Data;
    using System.Collections.Generic;
    using System;
    using System.Collections;
    using Windows.Storage;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;
    using System.Linq;
    using VideoFolders;

    public class FileCollection
    {

        private VideoFolders scanner;
        public FileCollection(VideoFolders scanner)
            : base()
        {
            this.scanner = scanner;
            this.fileList = new ObservableCollection<ScanningFile>();
        }

        public void Sort()
        {
            List<ScanningFile> sorted = this.fileList.OrderByDescending(x => x.DateCreated).ToList();
            for (int i = 0; i < this.fileList.Count(); i++)
                this.fileList.Move(this.fileList.IndexOf(sorted[i]), i);
        }

        public async void UpdateFolder(StorageFolder storageFolder)
        {
            await GetFiles(storageFolder);
        }
        public ScanningFile GetNextFile(ScanningFile currentFile)
        {
            ScanningFile file = this.sortedFileList.GetNextFile(currentFile);
            if (file != null)
            {
                return file;
            }
            else
            {
                return null;
            }
        }

        public ScanningFile GetPreviousFile(ScanningFile currentFile)
        {
            ScanningFile file = this.sortedFileList.GetPreviousFile(currentFile);
            if (file != null)
            {
                return file;
            }
            else
            {
                return null;
            }
        }

        private SortedList sortedFileList;
        private ObservableCollection<ScanningFile> fileList;

        public ObservableCollection<ScanningFile> FileList
        {
            get
            {
                return this.fileList;
            }
        }

        private async Task GetFiles(StorageFolder storageFolder)
        {
            #region start
            string methodName = "GetFiles";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));

            try
            {
                #endregion

                this.sortedFileList = this.scanner.ScannedFileLibrary.GetFilesForFolder(storageFolder, Sorting.None);
                this.fileList = sortedFileList.GetFileList();
               
                #region end
            }
            catch (Exception e)
            {
                bSucceeded = false;
                exception = e;
            }
            finally
            {
                Logging.Logger.Info(string.Format("{0}::{1} - Complete", this.GetType().Name, methodName));
            }

            if (!bSucceeded)
            {
                Logging.Logger.Critical(string.Format("{0}::{1} - Failed", this.GetType().Name, exception.ToString()));
                await Task.Delay(TimeSpan.FromSeconds(5));
                throw exception;
            }
                #endregion
        }
    }
}