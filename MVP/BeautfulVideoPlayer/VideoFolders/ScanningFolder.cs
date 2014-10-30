using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace VideoFolders
{
    class ScanningFolder
    {
        private FolderScanState scanState;
        private DateTime lastScanned;
        private StorageFolder folder;
        private Dictionary<string, ScanningFile> files;
        private bool currentlyDisplayed;

        public ScanningFolder(StorageFolder folder)
        {
            scanState = FolderScanState.None;
            this.folder = folder;
            files = new Dictionary<string, ScanningFile>();
        }

        public StorageFile GetNextFileToBeScanned()
        {
            foreach (KeyValuePair<string, ScanningFile> entry in this.files)
            { 
                if(entry.Value.ScanState != FileScanState.Scanned)
                {
                    return entry.Value.File;
                }
            }

            return null;
        }

        public void SetFileAsScanned(StorageFile file)
        {
            this.files[file.Name].ScanState =  FileScanState.Scanned;

            bool allFilesScanned = true;

            foreach (KeyValuePair<string, ScanningFile> entry in this.files)
            {
                if(entry.Value.ScanState != FileScanState.Scanned)
                {
                    allFilesScanned = false;
                }
            }

            if(allFilesScanned)
            {
                this.scanState = FolderScanState.Scanned;
            }
            else
            {
                this.scanState = FolderScanState.PartiallyScanned;
            }
        }

        public async Task<bool> Initialize()
        {
            var files = await this.folder.GetFilesAsync();

            foreach(StorageFile file in files)
            {
                this.files.Add(file.Name, new ScanningFile(file));
            }

            return true;
        }

        public StorageFolder Folder
        {
            get
            {
                return this.folder;
            }
        }

        public FolderScanState ScanState
        {
            get
            {
                return this.scanState;
            }
        }
    }
}
