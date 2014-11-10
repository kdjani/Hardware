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

namespace VideoFolders
{
    /// <summary>
    /// Shallow scanner. Contains concurrent queue to process scan. After done, updates the scan state.
    /// </summary>
    public class ShallowFileScanner : Scanner<ScanningFile>
    {
        public override bool IsItemAlreadyScanned(ScanningFile item)
        {
            if(this.fileLibrary.DoesFileExistInLibrary(item.Hash))
            {
                ScanningFile alreadyExistingFile = this.fileLibrary.GetFileFromLibrary(item.Hash);
                if(alreadyExistingFile.ScanState == FileScanState.ShallowScanned || alreadyExistingFile.ScanState == FileScanState.Scanned)
                {
                    return true;
                }
            }

            return false;
        }
        public override async Task ProcessQueueItem(ScanningFile item)
        {
            StorageFile file = await item.GetStorageFile();

            item.Hash = ScanningFile.ComputeMD5(file.Path);
            var properties = await file.Properties.GetVideoPropertiesAsync();
            var basicProperties = await file.GetBasicPropertiesAsync();
            item.DateCreated = file.DateCreated;
            item.Duration = properties.Duration;
            item.Title = properties.Title;
            item.Path = file.Path;
            item.Name = file.Name;
            item.FullName = file.DisplayName;
            item.ResolutionWidth = properties.Width;
            item.ResolutionHeight = properties.Height;
            item.Size = basicProperties.Size;
            item.DateModified = basicProperties.DateModified;
            item.FileType = file.FileType;

            await this.SaveThumbnailToFile(item);

            item.ScanState = FileScanState.ShallowScanned;
        }

        private async Task SaveThumbnailToFile(ScanningFile file)
        {
            #region start
            string methodName = "SaveThumbnailToFile";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} {2} - Start", this.GetType().Name, methodName, file.Name));

            try
            {
            #endregion
                    bool fileFoundOnDisk = true;
                    try
                    {
                        StorageFolder storageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Thumbnails", CreationCollisionOption.OpenIfExists);
                        StorageFile thumbnailFile = await storageFolder.GetFileAsync(file.Hash + ".png");
                        IRandomAccessStream fileStream = await thumbnailFile.OpenAsync(FileAccessMode.Read);
                        await fileStream.FlushAsync();
                        fileStream.Dispose();
                    }
                    catch (Exception e)
                    {
                        fileFoundOnDisk = false;
                    }

                    if (fileFoundOnDisk)
                    {
                        return;
                    }

                    StorageFile storageFile = await file.GetStorageFile();

                    var thumbnail = await storageFile.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.VideosView);
                    if (thumbnail != null)
                    {
                        WriteableBitmap bitmap = new WriteableBitmap((int)thumbnail.OriginalWidth, (int)thumbnail.OriginalHeight);
                        bitmap.SetSource(thumbnail);
                        StorageFolder storageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Thumbnails", CreationCollisionOption.OpenIfExists);
                        StorageFile thumbnailFile = await storageFolder.CreateFileAsync(file.Hash + ".png", CreationCollisionOption.FailIfExists);
                        IRandomAccessStream stream = await thumbnailFile.OpenAsync(FileAccessMode.ReadWrite);
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
                        Stream pixelStream = bitmap.PixelBuffer.AsStream();
                        byte[] pixels = new byte[pixelStream.Length];
                        await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, 96.0, 96.0, pixels);
                        await encoder.FlushAsync();
                        await stream.FlushAsync();
                        await pixelStream.FlushAsync();
                        stream.Dispose();
                        pixelStream.Dispose();
                        Logging.Logger.Info(string.Format("{0}::{1} {2} {3} - Saved", this.GetType().Name, methodName, file.Name, file.Hash));

                    }
                #region end
            }
            catch (UnauthorizedAccessException e)
            {
                Logging.Logger.Info(string.Format("{0}::{1} {2} - Already being written", this.GetType().Name, methodName, file.Name));
            }
            catch (Exception e)
            {
                if (!e.Message.StartsWith("Cannot create a file when that file already exists."))
                {
                    bSucceeded = false;
                    exception = e;
                }
                else
                {
                    Logging.Logger.Info(string.Format("{0}::{1} {2} {3} - Already saved", this.GetType().Name, methodName, file.Name, file.Hash));
                }
            }
            finally
            {
                Logging.Logger.Info(string.Format("{0}::{1} {2} - Complete", this.GetType().Name, methodName, file.Name));
            }

            if (!bSucceeded)
            {
                Logging.Logger.Critical(string.Format("{0}::{1} {2} - Failed", this.GetType().Name, file.Name, exception.ToString()));
                await Task.Delay(TimeSpan.FromSeconds(5));
                throw exception;
            }
            #endregion
        }
    }
}
