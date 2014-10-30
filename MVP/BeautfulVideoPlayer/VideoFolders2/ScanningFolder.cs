using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace VideoFolders
{
    [DataContractAttribute]
    public class ScanningFolder
    {
        private FolderScanState scanState;
        //private DateTime lastScanned;
        private StorageFolder folder;
        private Dictionary<string, ScanningFile> files;
        //private bool currentlyDisplayed;

        private string folderToken;
        [XmlAttribute]
        [DataMember(Order = 1)]
        public string FolderToken
        {
            get
            {
                return this.folderToken;
            }

            set
            {
                this.folderToken = value;
            }
        }    

        public ScanningFolder(StorageFolder folder)
        {
            Logging.Logger.Info(string.Format("ScanningFolder::ScanningFolder()"));

            scanState = FolderScanState.None;
            this.folder = folder;
            files = new Dictionary<string, ScanningFile>();

            this.FolderToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(folder);

            Logging.Logger.Info(string.Format("ScanningFolder::~ScanningFolder()"));
        }

        public async Task<StorageFolder> GetStorageFolder()
        {
            if (this.folder == null)
            {
                this.folder = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFolderAsync(this.FolderToken);
            }

            return this.folder;
        }

        public async Task<ScanningFile> GetNextFileToBeScanned(bool shallow)
        {
            ScanningFile file = null;
            #region start
            string methodName = "GetNextFileToBeScanned";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));
            
            try
            {
            #endregion

                foreach (KeyValuePair<string, ScanningFile> entry in this.files)
                {
                    Logging.Logger.Info(string.Format("ScanningFolder::GetNextFileToBeScanned - Considering {0}:{1}:{2}", entry.Key, entry.Value.ScanState, entry.Value.Name));

                    FileScanState expectedState = shallow ? FileScanState.ShallowScanned : FileScanState.Scanned;

                    if (entry.Value.ScanState != expectedState && entry.Value.ScanState != FileScanState.Scanned)
                    {
                        file = entry.Value;
                        break;
                    }
                }

                if (file != null)
                {
                    Logging.Logger.Info(string.Format("ScanningFolder::GetNextFileToBeScanned - Chose:{0}", file.Name));
                }
                else
                {
                    Logging.Logger.Info(string.Format("ScanningFolder::GetNextFileToBeScanned - No more files ..."));
                }
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

            return file;
        }

        public async Task SetFileAsScanned(ScanningFile file, bool shallow)
        {
            #region start
            string methodName = "SetFileAsScanned";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));
            
            try
            {
            #endregion

                Logging.Logger.Info(string.Format("ScanningFolder::SetFileAsScanned - {0}", file.Name));

                if (shallow)
                {
                    this.files[file.Name].ScanState = FileScanState.ShallowScanned;
                }
                else
                {
                    this.files[file.Name].ScanState = FileScanState.Scanned;
                }

                bool allFilesScanned = true;

                foreach (KeyValuePair<string, ScanningFile> entry in this.files)
                {
                    FileScanState expectedState = shallow ? FileScanState.ShallowScanned : FileScanState.Scanned;
                    if (entry.Value.ScanState != expectedState)
                    {
                        Logging.Logger.Info(string.Format("ScanningFolder::SetFileAsScanned - File not scanned: {0}{1}", entry.Value.ScanState, entry.Value.Name));
                        allFilesScanned = false;
                    }
                }

                if (allFilesScanned)
                {
                    Logging.Logger.Info(string.Format("ScanningFolder::SetFileAsScanned - All files scanned"));

                    this.SetAsScanned(shallow);
                }
                else
                {
                    Logging.Logger.Info(string.Format("ScanningFolder::SetFileAsScanned - Not all files scanned"));

                    this.scanState = FolderScanState.PartiallyScanned;
                }
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

        public void SetAsScanned(bool shallow)
        {
            if (shallow)
            {
                this.scanState = FolderScanState.ShallowScanned;
            }
            else
            {
                this.scanState = FolderScanState.Scanned;
            }
        }

        private FileLibrary library;

        public async Task<bool> Initialize(FileLibrary fileLibrary)
        {
            #region start
            string methodName = "Initialize";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));

            try
            {
            #endregion
                this.library = fileLibrary;
                List<Tuple<StorageFolder, StorageFile>> tempList = new List<Tuple<StorageFolder, StorageFile>>();
                await FileSystemHelper.GetAllFilesInFolder(this.folder, tempList);

                foreach (Tuple<StorageFolder, StorageFile> file in tempList)
                {
                    Logging.Logger.Info(string.Format("ScanningFolder::Initialize - Adding file: {0}", file.Item2.Name));
                    if (!this.files.ContainsKey(file.Item2.Name))
                    {
                        ScanningFile scanningFile = new ScanningFile(file.Item1, file.Item2);
                        await scanningFile.ScanStorageFile();
                        this.files.Add(file.Item2.Name, scanningFile);
                    }
                }
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

            return true;
        }

        public StorageFolder Folder
        {
            get
            {
                return this.folder;
            }
        }

        [DataMember(Order = 2)]
        public FolderScanState ScanState
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
