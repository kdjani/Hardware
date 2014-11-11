using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using ImageProcessing;
using ApplicationConfiguration;
using Windows.UI.Xaml.Controls;
using Windows.Graphics.Imaging;
using System.Reflection;

namespace VideoFolders
{
    public sealed class VideoFolders
    {
        private List<ScanningFolder> knownFolders;
        //private List<bool> knownFoldersStatus;
        private ScanningCommand scanCommand;
        private ScanningState scanState;
        private ScanningFolder currentFolderBeingScanned;
        private Configuration configuration;
        public static FileLibrary fileLibrary;
        DeepFileScanner deepScanner;
        ShallowFileScanner shallowScanner;
        private AnimationLibrary.AnimationLibrary library;

        public FileLibrary ScannedFileLibrary
        {
            get
            {
                return fileLibrary;
            }
        }

        //TODO Future
        // Make global queue of files to be processed with processing modes. Each mode as an enum will have a number that will dictate its priority
        // Top most queue items will be the 
        // Most long displayed items in the selected folder
        // Rest of the items in the selected folder
        // Other files from folders that were not processed for next mode based on mode priority
        // For rest of files, order by last scanned within the same mode outside the selected folder
        // Limit by memory
        // Limit by disk space

        // All these objects need to be serialized

        // For tomorrow
        // Add limit by frames + where to start based on percentage
        // Transfer pallet
        // Change AnimationImage to use thumbnail for some time before switching
        // Add timer in AnimationImage to look for generated image using animation library
        // Add finished processing to animation library
        // Make size of thumbnails smaller
        // Use constants everywhere
        // Add logging
        // Add logging shortcut key combination
        // Addd destroy library button

        public VideoFolders(AnimationLibrary.AnimationLibrary library)
        {
            Initilize(library);
        }

        private async void Initilize(AnimationLibrary.AnimationLibrary library)
        {
            #region start
            string methodName = "Initilize";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));
            
            try
            {
            #endregion
                this.videoFoldersLock = new object();
                
                this.configuration = new Configuration();
                this.knownFolders = new List<ScanningFolder>();

                this.scanState = ScanningState.None;
                this.scanCommand = ScanningCommand.None;
                this.currentFolderBeingScanned = null;
                fileLibrary = new FileLibrary();
                this.deepScanner = new DeepFileScanner(library);
                this.library = library;
                this.deepScanner.Initialize(fileLibrary);
                this.shallowScanner = new ShallowFileScanner();
                this.shallowScanner.Initialize(fileLibrary);

                SetCommand(ScanningCommand.Start, ScanningState.None, ScanningState.Started).Wait();
                this.ScanAllKnownFolders();
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

        private bool IsFolderAlreadyAdded(StorageFolder folder)
        {
            bool folderAlreadyAdded = false;

            #region start
            string methodName = "IsFolderAlreadyAdded";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName, folder.Path));
            
            try
            {
            #endregion

            lock (this.videoFoldersLock)
            {
                foreach (var f in this.knownFolders)
                {
                    if (f.Folder.Path == folder.Path)
                    {
                        folderAlreadyAdded = true;
                    }
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
                Logging.Logger.Info(string.Format("{0}::{1} {2} - Complete", this.GetType().Name, methodName, folderAlreadyAdded));
            }

            if (!bSucceeded)
            {
                Logging.Logger.Critical(string.Format("{0}::{1} - Failed", this.GetType().Name, exception.ToString()));
                throw exception;
            }
            #endregion

            return folderAlreadyAdded;
        }

        private async Task AddFolderToKnownList(StorageFolder folder)
        {
            #region start
            string methodName = "AddFolderToKnownList";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName, folder.Path));

            try
            {
            #endregion

                ScanningFolder scanningFolder = new ScanningFolder(folder);
                await scanningFolder.Initialize(fileLibrary);

                lock (this.videoFoldersLock)
                {
                    this.knownFolders.Insert(0, scanningFolder);
                }

                await fileLibrary.AddFolderToLibrary(scanningFolder);

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

        private async Task AddFolderRecursivelyToKnownList(StorageFolder folder)
        {
            #region start
            string methodName = "AddFolderRecursivelyToKnownList";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName, folder.Path));

            try
            {
            #endregion
                await AddFolderToKnownList(folder);

                List<StorageFolder> subFolders = new List<StorageFolder>();
                await FileSystemHelper.GetAllFoldersInFolder(folder, subFolders);
                foreach (var subFolder in subFolders)
                {
                    await AddFolderToKnownList(subFolder);
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

        public async void AddNewVideoFolder(StorageFolder folder)
        {
            #region start
            string methodName = "AddNewVideoFolder";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));
            
            try
            {
            #endregion

                this.shallowScan = true;

                bool folderAlreadyAdded = IsFolderAlreadyAdded(folder);

                if (!folderAlreadyAdded)
                {
                    await this.StopScan();
                    await AddFolderRecursivelyToKnownList(folder);
                    await this.StartScan();
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
            }
                #endregion
        }

        public async void AddNewVideoFolders(List<StorageFolder> folders)
        {
            #region start
            string methodName = "AddNewVideoFolders";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));

            try
            {
            #endregion

                this.shallowScan = true;

                await this.StopScan();

                foreach (var folder in folders)
                {
                    bool folderAlreadyAdded = IsFolderAlreadyAdded(folder);
                    if (!folderAlreadyAdded)
                    {
                        await AddFolderRecursivelyToKnownList(folder);
                    }
                }

                await this.StartScan();

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
            }
                #endregion
        }

        public async Task<List<StorageFolder>> GetAllViodeFolders()
        {
            List<StorageFolder> list = new List<StorageFolder>();

            #region start
            string methodName = "GetAllViodeFolders";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));
            
            try
            {
            #endregion
                foreach (var x in this.knownFolders)
                {
                    Logging.Logger.Info(string.Format("VideoFolders::GetAllViodeFolders - {0}:{1}", x.ScanState, x.Folder.Name));
                    list.Add(x.Folder);
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

            return list;

        }

        private bool shallowScan = true;

        public async void ScanAllKnownFolders()
        {
            #region start
            string methodName = "ScanAllKnownFolders";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));
            
            try
            {
            #endregion


                while (true)
                {
                    Logging.Logger.Info(string.Format("VideoFolders::ScanAllKnownFolders - Sleeping ..."));

                    // If scan is stopped then sleep for longer times
                    if (this.scanState == ScanningState.Stopped)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(10000));
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(10));
                    }



                        Logging.Logger.Info(string.Format("VideoFolders::GetAllViodeFolders -  Scan State: {0}", this.scanState));
                        Logging.Logger.Info(string.Format("VideoFolders::GetAllViodeFolders - Scan Command: {0}", this.scanCommand));

                        #region switch
                        switch (this.scanState)
                        {
                            case ScanningState.None:
                                {
                                    switch (this.scanCommand)
                                    {
                                        case ScanningCommand.None:
                                            //do nothing self loop
                                            break;
                                        case ScanningCommand.Pause:
                                            this.scanState = ScanningState.Paused;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                        case ScanningCommand.Resume:
                                            this.scanState = ScanningState.Started;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                        case ScanningCommand.Start:
                                            this.scanState = ScanningState.Started;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                        case ScanningCommand.Stop:
                                            this.scanState = ScanningState.Stopped;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                    }
                                }
                                break;
                            case ScanningState.Paused:
                                {
                                    switch (this.scanCommand)
                                    {
                                        case ScanningCommand.None:
                                            // do nothing
                                            break;
                                        case ScanningCommand.Pause:
                                            // self loop
                                            this.scanState = ScanningState.None;
                                            break;
                                        case ScanningCommand.Resume:
                                            this.scanState = ScanningState.Started;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                        case ScanningCommand.Start:
                                            this.scanState = ScanningState.Started;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                        case ScanningCommand.Stop:
                                            this.scanState = ScanningState.Stopped;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                    }
                                }
                                break;
                            case ScanningState.Stopped:
                                {
                                    switch (this.scanCommand)
                                    {
                                        case ScanningCommand.None:
                                            // do nothing
                                            break;
                                        case ScanningCommand.Pause:
                                            this.scanState = ScanningState.Paused;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                        case ScanningCommand.Resume:
                                            this.scanState = ScanningState.Started;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                        case ScanningCommand.Start:
                                            this.scanState = ScanningState.Started;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                        case ScanningCommand.Stop:
                                            // self loop
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                    }
                                }
                                break;
                            case ScanningState.Started:
                                {
                                    switch (this.scanCommand)
                                    {
                                        case ScanningCommand.None:
                                            // do nothing
                                            break;
                                        case ScanningCommand.Pause:
                                            this.scanState = ScanningState.Paused;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                        case ScanningCommand.Resume:
                                            this.scanState = ScanningState.Started;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                        case ScanningCommand.Start:
                                            // self loop
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                        case ScanningCommand.Stop:
                                            this.scanState = ScanningState.Stopped;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                    }
                                }
                                break;
                            case ScanningState.Error:
                                {
                                    switch (this.scanCommand)
                                    {
                                        case ScanningCommand.None:
                                            // do nothing
                                            break;
                                        case ScanningCommand.Pause:
                                            this.scanState = ScanningState.Paused;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                        case ScanningCommand.Resume:
                                            this.scanState = ScanningState.Started;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                        case ScanningCommand.Start:
                                            this.scanState = ScanningState.Started;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                        case ScanningCommand.Stop:
                                            this.scanState = ScanningState.Stopped;
                                            this.scanCommand = ScanningCommand.None;
                                            break;
                                    }
                                }
                                break;
                        }
                        #endregion

                        // Actions here

                        Logging.Logger.Info(string.Format("VideoFolders::GetAllViodeFolders - Scan State: {0}", this.scanState));
                        Logging.Logger.Info(string.Format("VideoFolders::GetAllViodeFolders - Scan Command: {0}", this.scanCommand));

                        if (this.scanState == ScanningState.Started)
                        {
                            if (this.knownFolders.Count == 0 || !fileLibrary.Loaded)
                            {
                                await Task.Delay(TimeSpan.FromMilliseconds(1000));
                            }
                            else
                            {
                                ScanningFile currentFile = null;
                                while (true)
                                {
                                    currentFile = await GetNextFile();
                                    if (currentFile == null)
                                    {
                                        Logging.Logger.Info(string.Format("VideoFolders::GetAllViodeFolders GetNextFile returned null ..."));

                                        if (this.knownFolders.Count != 0)
                                        {
                                            this.shallowScan = !this.shallowScan;
                                            currentFile = await GetNextFile();
                                            if (currentFile == null)
                                            {
                                                Logging.Logger.Info(string.Format("VideoFolders::GetAllViodeFolders GetNextFile returned null after changing scan type ..."));
                                                // We are done with scanning for now
                                                //await Task.Delay(TimeSpan.FromMilliseconds(1000));
                                                await this.StopScan();
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    if (this.shallowScan)
                                    {
                                        bool fileAlreadyAdded = await fileLibrary.AddFileToLibrary(currentFolderBeingScanned, currentFile);
                                        if (!fileAlreadyAdded)
                                        {
                                            this.shallowScanner.AddItemToScan(new Tuple<ScanningFile, ScanningFolder>(currentFile, currentFolderBeingScanned));
                                            await Task.Delay(TimeSpan.FromMilliseconds(100)); 
                                            AddFileToIndex(currentFile);
                                        }
                                        else
                                        {
                                            await this.currentFolderBeingScanned.SetFileAsScanned(currentFile, true);
                                        }
                                    }
                                    else
                                    {
                                        // add file if not added
                                        await fileLibrary.AddFileToLibrary(currentFolderBeingScanned, currentFile);
                                        bool itemExists = false;
                                        string hash = await ComputeMD5(currentFile.Path);
                                        itemExists = this.library.DoesItemExist(hash, AnimationLibrary.AnimationType.Category1Animation);
                                        if (!itemExists)
                                        {
                                            this.deepScanner.AddItemToScan(new Tuple<ScanningFile, ScanningFolder>(currentFile, currentFolderBeingScanned));
                                            await Task.Delay(TimeSpan.FromSeconds(5)); 
                                        }
                                        else
                                        {
                                            await this.currentFolderBeingScanned.SetFileAsScanned(currentFile, false);
                                        }
                                    }
                                }

                                if (this.scanState == ScanningState.Stopped)
                                {
                                    Logging.Logger.Info(string.Format("VideoFolders::GetAllViodeFolders - Scanning is being skipped..."));
                                    //break;
                                }
                            }
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
        }

        private async Task AddFileToIndex(ScanningFile file)
        {
            #region start
            string methodName = "AddFileToIndex";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} {2} - Start", this.GetType().Name, methodName, file.Path));

            try
            {
            #endregion

                var indexer = Windows.Storage.Search.ContentIndexer.GetIndexer();
                var content = new Windows.Storage.Search.IndexableContent();

                string hash = await ComputeMD5(file.Path);

                content.Properties[Windows.Storage.SystemProperties.ItemNameDisplay] = file.FullName;
                content.Properties[Windows.Storage.SystemProperties.Keywords] = file.Path;
                content.Properties[Windows.Storage.SystemProperties.Comment] = file.FullName;
                content.Id = hash;

                await indexer.AddAsync(content);

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

  

        private async Task<bool> StartScan()
        {
            #region start
            string methodName = "StartScan";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));

            try
            {
            #endregion
                await SetCommand(ScanningCommand.Start, ScanningState.Stopped, ScanningState.Started);
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

        private async Task<bool> StopScan()
        {
            #region start
            string methodName = "StopScan";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));
            
            try
            {
            #endregion
                await SetCommand(ScanningCommand.Stop, ScanningState.Started, ScanningState.Stopped);
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

        private void PauseScan()
        {

        }

        private void ResumeScan()
        {

        }

        private object videoFoldersLock;

        private async Task<ScanningFolder> GetNextFolderToBeScanned()
        {
            ScanningFolder returnFolder = null;

            #region start
            string methodName = "GetNextFolderToBeScanned";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));
            
            try
            {
            #endregion
                
                await fileLibrary.SaveToXml();

                lock (this.videoFoldersLock)
                {
                    foreach (var folder in this.knownFolders)
                    {
                        Logging.Logger.Info(string.Format("VideoFolders::GetNextFolderToBeScanned - Considering folder: {0}:{1}", folder.ScanState, folder.Folder.Name));

                        FolderScanState expectedState = this.shallowScan ? FolderScanState.ShallowScanned : FolderScanState.Scanned;

                        if (folder.ScanState != expectedState)
                        {
                            returnFolder = folder;
                            break;
                        }
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

            if (returnFolder != null)
            {
                Logging.Logger.Info(string.Format("VideoFolders::GetNextFolderToBeScanned - Returning folder: {0}", returnFolder.Folder.Name));
            }

            return returnFolder;
        }

        private async Task<ScanningFile> GetNextFile()
        {
            ScanningFile file = null;

            #region start
            string methodName = "GetNextFile";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));
            
            try
            {
            #endregion
                if (this.currentFolderBeingScanned == null)
                {
                    Logging.Logger.Info(string.Format("VideoFolders::GetNextFile - Get next folder"));

                    this.currentFolderBeingScanned = await GetNextFolderToBeScanned();
                }

                if (this.currentFolderBeingScanned != null)
                {
                    Logging.Logger.Info(string.Format("VideoFolders::GetNextFile - Next folder: {0}:{1}", this.currentFolderBeingScanned.ScanState, this.currentFolderBeingScanned.Folder.Name));
                }
                else
                {
                    Logging.Logger.Info(string.Format("VideoFolders::GetNextFile - No more folders"));
                }

                while (this.currentFolderBeingScanned != null)
                {
                    file = await this.currentFolderBeingScanned.GetNextFileToBeScanned(this.shallowScan);

                    if(file == null)
                    {
                        this.currentFolderBeingScanned.SetAsScanned(this.shallowScan);
                        this.currentFolderBeingScanned = await GetNextFolderToBeScanned();
                    }

                    if (file != null)
                    {
                        Logging.Logger.Info(string.Format("VideoFolders::GetNextFile - Next file: {0}", file.Name));
                        break;
                    }
                    else
                    {
                        Logging.Logger.Info(string.Format("VideoFolders::GetNextFile - No more files"));
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

            return file;
        }

        private async Task<bool> SetCommand(ScanningCommand command, ScanningState expectedState, ScanningState expectedFinalState)
        {
            #region start
            string methodName = "SetCommand";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));
            
            try
            {
            #endregion
                Logging.Logger.Info(string.Format("VideoFolders::SetCommand - {0}:{1}", this.scanState, expectedState));

                while (this.scanCommand != ScanningCommand.None)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }

                if (this.scanState == expectedState)
                {
                    scanCommand = command;

                    //while (this.scanState != expectedFinalState)
                    {
                        //Logging.Logger.Info(string.Format("VideoFolders::SetCommand - Waiting..."));
                        //await Task.Delay(TimeSpan.FromSeconds(1));
                    }

                    Logging.Logger.Info(string.Format("VideoFolders::SetCommand - State changed..."));
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

        private async Task<string> ComputeMD5(string str)
        {
            string res = string.Empty;
            #region start
            string methodName = "ComputeMD5";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));
            
            try
            {
            #endregion
                var alg = HashAlgorithmProvider.OpenAlgorithm("MD5");
                IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
                var hashed = alg.HashData(buff);
                res = CryptographicBuffer.EncodeToHexString(hashed);
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
            return res;
        }
    }
}
