using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace VideoFolders
{
    class ScanningFile
    {
        private FileScanState scanState;
        private DateTime lastScanned;
        private StorageFile file;
        private DateTime displayedTime;

        public ScanningFile(StorageFile file)
        {
            scanState = FileScanState.None;
            this.file = file;
        }

        public StorageFile File
        {
            get
            {
                return this.file;
            }
        }

        public FileScanState ScanState
        {
            get
            {
                return this.scanState;
            }

            set
            {
                this.scanState = value;
            }
        }
    }
}
