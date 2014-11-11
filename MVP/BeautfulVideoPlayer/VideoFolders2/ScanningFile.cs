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
    [DataContractAttribute]
    public class ScanningFile
    {
        private FileScanState scanState;

        [XmlAttribute]
        [DataMember(Order = 1)]
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


        private DateTime lastScanned;
        [XmlAttribute]
        [DataMember(Order = 2)]
        public DateTime LastScanned
        {
            get
            {
                return this.lastScanned;
            }

            set
            {
                this.lastScanned = value;
            }
        }



        private StorageFile file;
        private long playCount;
        [XmlAttribute]
        [DataMember(Order = 3)]
        public long PlayCount
        {
            get
            {
                return this.playCount;
            }

            set
            {
                this.playCount = value;
            }
        }

        private bool favorite;
        [XmlAttribute]
        [DataMember(Order = 4)]
        public bool Favorite
        {
            get
            {
                return this.favorite;
            }

            set
            {
                this.favorite = value;
            }
        }

        private string title;
        [XmlAttribute]
        [DataMember(Order = 5)]
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }

        private string name;
        [XmlAttribute]
        [DataMember(Order = 6)]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        private string fullname;
        [XmlAttribute]
        [DataMember(Order = 7)]
        public string FullName
        {
            get
            {
                return this.fullname;
            }
            set
            {
                this.fullname = value;
            }
        }

        private string path;
        [XmlAttribute]
        [DataMember(Order = 8)]
        public string Path
        {
            get
            {
                return this.path;
            }
            set
            {
                this.path = value;
            }
        }

        private string parentPath;
        [XmlAttribute]
        [DataMember(Order = 9)]
        public string ParentPath
        {
            get
            {
                return this.parentPath;
            }
            set
            {
                this.parentPath = value;
            }
        }

        private DateTimeOffset dateCreated;
        [XmlAttribute]
        [DataMember(Order = 10)]
        public DateTimeOffset DateCreated
        {
            get
            {
                return this.dateCreated;
            }

            set
            {
                this.dateCreated = value;
            }
        }

        private DateTimeOffset datemodified;
        [XmlAttribute]
        [DataMember(Order = 11)]
        public DateTimeOffset DateModified
        {
            get
            {
                return this.datemodified;
            }

            set
            {
                this.datemodified = value;
            }
        }

        private string fileType;
        [XmlAttribute]
        [DataMember(Order = 12)]
        public string FileType
        {
            get
            {
                return this.fileType;
            }

            set
            {
                this.fileType = value;
            }
        }

        private ulong size;
        [XmlAttribute]
        [DataMember(Order = 13)]
        public ulong Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
            }
        }

        private TimeSpan duration;
        [XmlAttribute]
        [DataMember(Order = 14)]
        public TimeSpan Duration
        {
            get
            {
                return this.duration;
            }
            set
            {
                this.duration = value;
            }
        }

        private long resolutionWidth;
        [XmlAttribute]
        [DataMember(Order = 15)]
        public long ResolutionWidth
        {
            get
            {
                return this.resolutionWidth;
            }
            set
            {
                this.resolutionWidth = value;
            }
        }

        private long resolutionHeight;
        [XmlAttribute]
        [DataMember(Order = 16)]
        public long ResolutionHeight
        {
            get
            {
                return this.resolutionHeight;
            }
            set
            {
                this.resolutionHeight = value;
            }
        }

        private string hash;
        [XmlAttribute]
        [DataMember(Order = 17)]
        public string Hash
        {
            get
            {
                return this.hash;
            }

            set
            {
                this.hash = value;
            }
        }

        private bool thumbnailExists = false;
        [XmlAttribute]
        [DataMember(Order = 18)]
        public bool ThumbnailExists
        {
            get
            {
                return this.thumbnailExists;
            }

            set
            {
                this.thumbnailExists = value;
            }
        }    

        public async Task<WriteableBitmap> GetThumbnail()
        {
            WriteableBitmap bitmap = null;

            #region start
            string methodName = "GetThumbnail";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} {2} - Start", this.GetType().Name, methodName, this.path));

            try
            {
            #endregion

                if (this.scanState == FileScanState.ShallowScanned || this.scanState == FileScanState.Scanned)
                {
                    if (this.ThumbnailExists)
                    {
                        return await WritableImageFromFile();
                    }
                    else
                    {
                        return null;
                    }
                } 
                else
                {
                    StorageFile file = await this.GetStorageFile();
                    var thumbnail = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.VideosView);
                    bitmap = new WriteableBitmap((int)thumbnail.OriginalWidth, (int)thumbnail.OriginalHeight);
                    bitmap.SetSource(thumbnail);
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
                Logging.Logger.Info(string.Format("{0}::{1} {2} - Complete", this.GetType().Name, methodName, this.path));
            }

            if (!bSucceeded)
            {
                Logging.Logger.Critical(string.Format("{0}::{1} {2} - Failed", this.GetType().Name, this.path, exception.ToString()));
                await Task.Delay(TimeSpan.FromSeconds(5));
                throw exception;
            }
            #endregion

            return bitmap;
        }

        public ScanningFile(StorageFolder folder, StorageFile file)
        {
            #region start
            string methodName = "ScanningFile";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} {2} {3} - Start", this.GetType().Name, methodName, folder.Path, file.Name));

            try
            {
            #endregion
                scanState = FileScanState.None;
                this.LastScanned = DateTime.MinValue;
                this.file = file;
                this.PlayCount = 0;
                this.favorite = false;
                this.Name = this.file.Name;
                this.ParentPath = folder.Path;
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
                throw exception;
            }
            #endregion
        }

        public async Task ScanStorageFile()
        {
            #region start
            string methodName = "ScanStorageFile";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} {2} - Start", this.GetType().Name, methodName, this.file.Name));

            try
            {
            #endregion
                // TODO scanner work
                this.Hash = ScanningFile.ComputeMD5(this.file.Path);
                var properties = await this.file.Properties.GetVideoPropertiesAsync();
                var basicProperties = await this.file.GetBasicPropertiesAsync();
                this.DateCreated = this.file.DateCreated;
                this.Duration = properties.Duration;
                this.Title = properties.Title;
                this.Path = this.file.Path;
                this.Name = this.file.Name;
                this.FullName = this.file.DisplayName;
                this.resolutionWidth = properties.Width;
                this.resolutionHeight = properties.Height;
                this.Size = basicProperties.Size;
                this.DateModified = basicProperties.DateModified;
                this.FileType = file.FileType;
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
                throw exception;
            }
                #endregion
        }


        private static StorageFolder storageFolder;

        private async Task<WriteableBitmap> WritableImageFromFile()
        {
            WriteableBitmap writableBitmap = null;

            #region start
            string methodName = "WritableImageFromFile";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} {2} - Start", this.GetType().Name, methodName, this.file.Name));

            try
            {
            #endregion

                if (storageFolder == null)
                {
                    storageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Thumbnails");
                }

                if (this.ThumbnailExists)
                {
                    Logging.Logger.Info(string.Format("{0}::{1} {2} {3}", this.GetType().Name, methodName, this.file.Name, this.hash));

                    StorageFile thumbnailFile = await storageFolder.GetFileAsync(this.hash + ".png");
                    IRandomAccessStream fileStream = await thumbnailFile.OpenAsync(FileAccessMode.Read);
                    writableBitmap = new WriteableBitmap(1, 1);
                    writableBitmap.SetSource(fileStream);
                    //await fileStream.FlushAsync();
                    fileStream.Dispose();
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
                Logging.Logger.Info(string.Format("{0}::{1} {2} - Complete", this.GetType().Name, methodName, this.file.Name));
            }

            if (!bSucceeded)
            {
                Logging.Logger.Critical(string.Format("{0}::{1} {2} - Failed", this.GetType().Name, this.file.Name, exception.ToString()));
                await Task.Delay(TimeSpan.FromSeconds(5));
                throw exception;
            }
                #endregion
            return writableBitmap;
        }

        public async Task<StorageFile> GetStorageFile()
        {
            #region start
            string methodName = "GetStorageFile";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} {2} - Start", this.GetType().Name, methodName, this.file.Name));

            try
            {
            #endregion

                if (this.file != null)
                {
                    return this.file;
                }
                else
                {
                    ScanningFolder parentFolder = VideoFolders.fileLibrary.GetSavedFolder(this.ParentPath);

                    StorageFolder parentStorageFolder = await parentFolder.GetStorageFolder();

                    return await parentStorageFolder.GetFileAsync(this.Name);
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

            return null;
        }

        public static string ComputeMD5(string path)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm("MD5");
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(path, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }
    }
}
