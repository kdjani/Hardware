using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Graphics.Imaging;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using ImageProcessing;
using ApplicationConfiguration;

namespace VideoFolders
{
    /// <summary>
    /// Shallow scanner. Contains concurrent queue to process scan. After done, updates the scan state.
    /// </summary>
    public class DeepFileScanner : Scanner<Tuple<ScanningFile, ScanningFolder>>
    {
        private ImageExtractor imageExtractor;
        private Configuration configuration;
        private AnimationLibrary.AnimationLibrary library;

        public DeepFileScanner(AnimationLibrary.AnimationLibrary library)
        {
            this.imageExtractor = new ImageExtractor();
            this.configuration = new Configuration();
            this.library = library;
        }
        public override bool IsItemAlreadyScanned(Tuple<ScanningFile, ScanningFolder> item)
        {
            Logging.Logger.Info(string.Format("DeepFileScanner::IsItemAlreadyScanned - {0}", item.Item1.Name));

            if (this.fileLibrary.DoesFileExistInLibrary(item.Item1.Hash))
            {
                Logging.Logger.Info(string.Format("DeepFileScanner::IsItemAlreadyScanned In library- {0}", item.Item1.Name));
                ScanningFile alreadyExistingFile = this.fileLibrary.GetFileFromLibrary(item.Item1.Hash);
                if (alreadyExistingFile.ScanState == FileScanState.Scanned)
                {
                    Logging.Logger.Info(string.Format("DeepFileScanner::IsItemAlreadyScanned - Already scanned {0}", item.Item1.Name));
                    return true;
                }
                else
                {
                    Logging.Logger.Info(string.Format("DeepFileScanner::IsItemAlreadyScanned - Not already scanned {0}", item.Item1.Name));
                }
            }
            else
            {
                Logging.Logger.Info(string.Format("DeepFileScanner::IsItemAlreadyScanned Not in library - {0}", item.Item1.Name));
            }

            return false;
        }
        public override async Task ProcessQueueItem(Tuple<ScanningFile, ScanningFolder> entry)
        {
            ScanningFile item = entry.Item1;

            Logging.Logger.Info(string.Format("DeepFileScanner::ProcessQueueItem - Considering: {0}", item.Name));
            string hash = await ComputeMD5(item.Path);
            Logging.Logger.Info(string.Format("DeepFileScanner::ProcessQueueItem - Hash: {0}:{1}", hash, item.Name));
            var userImagesFolder = await CreateFolder(ApplicationData.Current.LocalFolder, hash);

            this.imageExtractor.GenerateThumbnails(await item.GetStorageFile(), hash);
            int giveup = 0;
            while (imageExtractor.IsGenerationFinished() == false)
            {
                Logging.Logger.Info(string.Format("DeepFileScanner::ProcessQueueItem - Waiting to finish"));
                await Task.Delay(TimeSpan.FromSeconds(this.configuration.ScanRetryDelay));
                if (giveup++ > this.configuration.ScanRetryTimes)
                {
                    Logging.Logger.Info(string.Format("DeepFileScanner::ProcessQueueItem - Giving up on {0} ...", item.Path));

                    //giveup = 0;
                    //imageExtractor.GenerateThumbnails(item, hash);
                    break;
                }
            }

            if (imageExtractor.IsUnrecoverableError() || (giveup++ > this.configuration.ScanRetryTimes))
            {
                Logging.Logger.Info(string.Format("DeepFileScanner::ProcessQueueItem - Skipping file because of unrecoverable error."));
            }
            else
            {

                Logging.Logger.Info(string.Format("DeepFileScanner::ProcessQueueItem - Starting resizing"));

                StorageFolder storageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(hash);
                IReadOnlyList<StorageFile> fileList = await storageFolder.GetFilesAsync();
                foreach (StorageFile sampleFile in fileList)
                {
                    Logging.Logger.Info(string.Format("DeepFileScanner::ProcessQueueItem - Considering file for resize: {0}", sampleFile.Name));

                    if (!sampleFile.Name.StartsWith("R_"))
                    {
                        var resizedItem = await storageFolder.TryGetItemAsync("R_" + sampleFile.Name);

                        if (resizedItem != null)
                        {
                            Logging.Logger.Info(string.Format("DeepFileScanner::ProcessQueueItem - Deleting resize olf file: {0}", sampleFile.Name));
                            await resizedItem.DeleteAsync();
                        }

                        Logging.Logger.Info(string.Format("DeepFileScanner::ProcessQueueItem - Creating resized: {0}", sampleFile.Name));

                        StorageFile newFile = await storageFolder.CreateFileAsync("R_" + sampleFile.Name);
                        await ResizePng(sampleFile, newFile, Configuration.ImageHeight, Configuration.ImageHeight);
                        Logging.Logger.Info(string.Format("DeepFileScanner::ProcessQueueItem - Deleting resize original: {0}", sampleFile.Name));
                        await sampleFile.DeleteAsync();

                        Logging.Logger.Info(string.Format("DeepFileScanner::ProcessQueueItem Test"));
                    }
                }

                Logging.Logger.Info(string.Format("DeepFileScanner::ProcessQueueItem-  Adding to library: {0}:{1}", hash, item.Name));
                AnimationLibrary.AnimationItem libraryEntry = new AnimationLibrary.AnimationItem(hash, hash, AnimationLibrary.AnimationType.Category1Animation);
                this.library.AddItem(libraryEntry);
                Logging.Logger.Info(string.Format("DeepFileScanner::ProcessQueueItem - Setting as deep scanned: {0}", item.Name));
                await entry.Item2.SetFileAsScanned(item, false);
            }
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

        private async Task<StorageFolder> CreateFolder(StorageFolder storageFolder, string folderName)
        {
            StorageFolder folder = null;

            #region start
            string methodName = "CreateFolder";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));
            
            try
            {
            #endregion
                // we do the try w/ empty catch because the GetFolderAsync will throw if not found and we cannot do await inside a catch block  
                try
                {
                    folder = await storageFolder.GetFolderAsync(folderName);
                }
                catch { }

                if (folder == null)
                {
                    Logging.Logger.Info(string.Format("DeepFileScanner::CreateFolder - Creating"));
                    folder = await storageFolder.CreateFolderAsync(folderName);
                }

                Logging.Logger.Info(string.Format("DeepFileScanner::CreateFolder - {0}", folder.Name));

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

            return folder;
        }

        private async Task ResizePng(StorageFile sourceFile, StorageFile destinationFile, int newWidth, int newHeight)
        {
            #region start
            string methodName = "ResizePng";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));

            try
            {
            #endregion
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
