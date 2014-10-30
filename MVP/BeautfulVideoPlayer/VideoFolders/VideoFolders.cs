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
        private List<bool> knownFoldersStatus;
        private ScanningCommand scanCommand;
        private ScanningState scanState;
        private ScanningFolder currentFolderBeingScanned;
        private ImageExtractor imageExtractor;
        private Configuration configuration;
        private AnimationLibrary.AnimationLibrary library;

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
            Initilize();
        }

        private async void Initilize()
        {
            await this.LogException(
                "Initilize",
                new Action(
                    async () =>
                    {
                        this.imageExtractor = new ImageExtractor();
                        this.configuration = new Configuration();
                        this.knownFolders = new List<ScanningFolder>();

                        this.scanState = ScanningState.None;
                        this.scanCommand = ScanningCommand.None;
                        this.currentFolderBeingScanned = null;
                        this.library = library;

                        SetCommand(ScanningCommand.Start, ScanningState.None, ScanningState.Started);
                        this.ScanAllKnownFolders();
                     }));
        }

        public async void AddNewVideoFolder(StorageFolder folder)
        {
            await this.LogException(
                "AddNewVideoFolder",
                new Action(
                    async () =>
                    {
                        await this.StopScan();

                        ScanningFolder scanningFolder = new ScanningFolder(folder);
                        await scanningFolder.Initialize();
                        this.knownFolders.Insert(0, scanningFolder);
            
                        await this.StartScan();
                    }));
        }

        public async Task<FoldersCollection> GetAllViodeFolders()
        {
            List<StorageFolder> list = new List<StorageFolder>();

            await this.LogException(
                "GetAllViodeFolders",
                new Action(
                    async () =>
                    {
                        foreach (var x in this.knownFolders)
                        {
                            list.Add(x.Folder);
                        }
                     }));

            return new FoldersCollection();// TODO list.ToList()

        }

        public async void ScanAllKnownFolders()
        {
            await this.LogException(
                "ScanAllKnownFolders",
                new Action(
                    async () =>
                    {
                        while (true)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));

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

                            // Actions here

                            if (this.scanState == ScanningState.Started)
                            {
                                StorageFile currentFile = await GetNextFile();
                                if (currentFile == null)
                                {
                                    await this.StopScan();
                                }
                                else
                                {
                                    string hash = await ComputeMD5(currentFile.Path);
                                    bool itemExists = this.library.DoesItemExist(hash, AnimationLibrary.AnimationType.Category1Animation);
                                    if (!itemExists)
                                    {
                                        var userImagesFolder = await CreateFolder(ApplicationData.Current.LocalFolder, hash);

                                        this.imageExtractor.GenerateThumbnails(currentFile, hash);
                                        int giveup = 0;
                                        while (imageExtractor.IsGenerationFinished() == false)
                                        {
                                            await Task.Delay(TimeSpan.FromSeconds(this.configuration.ScanRetryDelay));
                                            if (giveup++ > this.configuration.ScanRetryTimes)
                                            {
                                                giveup = 0;
                                                imageExtractor.GenerateThumbnails(currentFile, hash);
                                            }
                                        }

                                        StorageFolder storageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(hash);
                                        IReadOnlyList<StorageFile> fileList = await storageFolder.GetFilesAsync();
                                        foreach (StorageFile sampleFile in fileList)
                                        {
                                            if (!sampleFile.Name.StartsWith("R_"))
                                            {
                                                var item = await storageFolder.TryGetItemAsync("R_" + sampleFile.Name);

                                                if (item != null)
                                                {
                                                    await item.DeleteAsync();
                                                }

                                                StorageFile newFile = await storageFolder.CreateFileAsync("R_" + sampleFile.Name);
                                                await ResizePng(sampleFile, newFile, Configuration.ImageHeight, Configuration.ImageHeight);
                                                await sampleFile.DeleteAsync();
                                            }
                                        }

                                        AnimationLibrary.AnimationItem libraryEntry = new AnimationLibrary.AnimationItem(hash, hash, AnimationLibrary.AnimationType.Category1Animation);
                                        this.library.AddItem(libraryEntry);
                                    }

                                    this.currentFolderBeingScanned.SetFileAsScanned(currentFile);
                                }
                            }

                            if (this.scanState == ScanningState.Stopped)
                            {
                                break;
                            }
                        }}));
        }

        private async Task ResizePng(StorageFile sourceFile, StorageFile destinationFile, int newWidth, int newHeight)
        {
            await this.LogException(
                "ResizePng",
                new Action(
                    async () =>
                    {
                        using (var sourceStream = await sourceFile.OpenAsync(FileAccessMode.Read))
                        {
                            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(sourceStream);

                            int updatedWidth = (int)Math.Ceiling((double)decoder.PixelWidth / (double)decoder.PixelHeight * (double)newHeight);

                            BitmapTransform transform = new BitmapTransform() { ScaledHeight = (uint)newHeight, ScaledWidth = (uint)updatedWidth };


                            PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                                BitmapPixelFormat.Rgba8,
                                BitmapAlphaMode.Straight,
                                transform,
                                ExifOrientationMode.RespectExifOrientation,
                                ColorManagementMode.DoNotColorManage);

                            using (var destinationStream = await destinationFile.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, destinationStream);

                                BitmapBounds bounds = new BitmapBounds();
                                bounds.Height = (uint)newHeight;
                                bounds.Width = (uint)newWidth;
                                bounds.X = (uint)((updatedWidth - newWidth) / 2);
                                bounds.Y = 0;
                                encoder.BitmapTransform.Bounds = bounds;

                                encoder.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Premultiplied, (uint)updatedWidth, (uint)newHeight, 96, 96, pixelData.DetachPixelData());
                                await encoder.FlushAsync();
                            }
                        }
                    }));
        }

        private async Task<bool> StartScan()
        {
            await this.LogException(
                "StartScan",
                new Action(
                    async () =>
                    {
                         await SetCommand(ScanningCommand.Start, ScanningState.Stopped, ScanningState.Started);
                    }));
            return true;
        }

        private async Task<bool> StopScan()
        {
            await this.LogException(
                "StopScan",
                new Action(
                    async () =>
                    {
                        await SetCommand(ScanningCommand.Stop, ScanningState.Started, ScanningState.Stopped);
                    }));
            return true;
        }

        private void PauseScan()
        {

        }

        private void ResumeScan()
        {

        }

        private async Task<ScanningFolder> GetNextFolderToBeScanned()
        {
            ScanningFolder returnFolder = null;

            await this.LogException(
                "GetNextFolderToBeScanned",
                new Action(
                    async () =>
                    {
                        foreach(var folder in this.knownFolders)
                        {
                            if(folder.ScanState != FolderScanState.Scanned)
                            {
                                returnFolder = folder;
                            }
                        }
                        }));

            return returnFolder;
        }

        private async Task<StorageFile> GetNextFile()
        {
            StorageFile file = null;

            await this.LogException(
                "GetNextFile",
                new Action(
                    async () =>
                    {
                        if (this.currentFolderBeingScanned == null)
                        {
                            this.currentFolderBeingScanned = await GetNextFolderToBeScanned();
                        }

                        while (this.currentFolderBeingScanned != null)
                        {
                            file = this.currentFolderBeingScanned.GetNextFileToBeScanned();

                            if(file == null)
                            {
                                this.currentFolderBeingScanned = await GetNextFolderToBeScanned();
                            }
                        }
                    }));

            return file;
        }

        private async Task<bool> SetCommand(ScanningCommand command, ScanningState expectedState, ScanningState expectedFinalState)
        {
            await this.LogException(
                "SetCommand",
                new Action(
                    async () =>
                    {
                        if (this.scanState == expectedState)
                        {
                            scanCommand = command;

                            while (this.scanState == expectedFinalState)
                            {
                                await Task.Delay(TimeSpan.FromSeconds(1));
                            }
                        }
                    }));

            return true;
        }

        private async Task<StorageFolder> CreateFolder(StorageFolder storageFolder, string folderName)
        {
            StorageFolder folder = null;

            await this.LogException(
                "CreateFolder",
                new Action(
                    async () =>
                    {
                        // we do the try w/ empty catch because the GetFolderAsync will throw if not found and we cannot do await inside a catch block  
                        try
                        {
                            folder = await storageFolder.GetFolderAsync(folderName);
                        }
                        catch { }

                        if (folder == null)
                        {
                            folder = await storageFolder.CreateFolderAsync(folderName);
                        }
                    }));

            return folder;
        }

        private async Task<string> ComputeMD5(string str)
        {
            string res = string.Empty;
            await this.LogException(
                "ComputeMD5",
                new Action(
                    async () =>
                    {
                        var alg = HashAlgorithmProvider.OpenAlgorithm("MD5");
                        IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
                        var hashed = alg.HashData(buff);
                        res = CryptographicBuffer.EncodeToHexString(hashed);
                     }));
            return res;
        }

        private async Task LogException(string componentName, Action moduleCode)
        {
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, componentName));

            try
            {
                moduleCode();
            }
            catch (Exception e)
            {
                bSucceeded = false;
                exception = e;
            }
            finally
            {
                Logging.Logger.Info(string.Format("{0}::{1} - Complete", this.GetType().Name, componentName));
            }

            if (!bSucceeded)
            {
                Logging.Logger.Critical(string.Format("{0}::{1} - Failed", this.GetType().Name, exception.ToString()));
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}
