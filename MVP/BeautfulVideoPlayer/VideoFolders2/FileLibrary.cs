using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.Storage.Streams;

namespace VideoFolders
{
    public class FileLibrary
    {
        private Dictionary<string, SortedList> folderFileMap;
        private Dictionary<string, ScanningFile> fileMap;
        private Dictionary<string, ScanningFolder> folderMap;
        private SortedList fileList;
        private Dictionary<string, SortedList> savedSearches;
        private object libraryLock;

        public FileLibrary()
        {
            this.libraryLock = new object();
            this.folderMap = new Dictionary<string, ScanningFolder>();
            this.folderFileMap = new Dictionary<string, SortedList>();
            this.fileMap = new Dictionary<string, ScanningFile>();
            this.fileList = new SortedList();
            this.savedSearches = new Dictionary<string, SortedList>();
            this.dirty = true;

            this.loaded = false;
            this.LoadFromXml();
        }

        private const string savedFolders = "SavedFolders.xml";
        private const string savedFoldersFiles = "SavedFoldersFiles.xml";
        private const string savedFileMap = "SavedFileMap.xml";
        private const string savedFileList = "SavedFileList.xml";
        private const string savedSearchesList = "SavedSearchesList.xml";

        private async Task<object> LoadFromXmlInternal<T>(string fileName)
        {
            #region start
            string methodName = "LoadFromXmlInternal";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} {2} - Start", this.GetType().Name, methodName, fileName));

            try
            {
            #endregion

                List<Type> knownTypes = new List<Type>();
                knownTypes.Add(typeof(SortedList));
                knownTypes.Add(typeof(ScanningFile));
                knownTypes.Add(typeof(Dictionary<string, SortedList>));
                knownTypes.Add(typeof(Dictionary<string, ScanningFile>));
                knownTypes.Add(typeof(List<ScanningFile>));
                knownTypes.Add(typeof(ObservableCollection<ScanningFile>));

                StorageFile sessionFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                var basicProperties = await sessionFile.GetBasicPropertiesAsync(); ;
                if (sessionFile != null && basicProperties.Size != 0)
                {
                    IInputStream sessionInputStream = await sessionFile.OpenReadAsync();
                    DataContractSerializer sessionSerializer = new DataContractSerializer(typeof(T), knownTypes.ToArray());
                    lock (this.libraryLock)
                    {
                        return sessionSerializer.ReadObject(sessionInputStream.AsStreamForRead());
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

            return null;
        }

        public async Task SaveToXmlInternal<T>(string fileName, object obj)
        {
            #region start
            string methodName = "SaveToXmlInternal";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} {2} - Start", this.GetType().Name, methodName, fileName));

            try
            {
            #endregion
                using (MemoryStream memStm = new MemoryStream())
                {
                    StorageFile sessionFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                    IRandomAccessStream sessionRandomAccess = await sessionFile.OpenAsync(FileAccessMode.ReadWrite);
                    IOutputStream sessionOutputStream = sessionRandomAccess.GetOutputStreamAt(0);

                    List<Type> knownTypes = new List<Type>();
                    knownTypes.Add(typeof(SortedList));
                    knownTypes.Add(typeof(ScanningFile));
                    knownTypes.Add(typeof(Dictionary<string, SortedList>));
                    knownTypes.Add(typeof(Dictionary<string, ScanningFile>));
                    knownTypes.Add(typeof(List<ScanningFile>));
                    knownTypes.Add(typeof(ObservableCollection<ScanningFile>));
                    var sessionSerializer = new DataContractSerializer(typeof(T), knownTypes.ToArray());

                    lock (this.libraryLock)
                    {
                        sessionSerializer.WriteObject(sessionOutputStream.AsStreamForWrite(), obj);
                    }

                    await sessionOutputStream.FlushAsync();
                    sessionOutputStream.Dispose();
                    sessionRandomAccess.Dispose();
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

        private bool loaded;

        public bool Loaded
        {
            get
            {
                return this.loaded;
            }
        }

        public async void LoadFromXml()
        {
            #region start
            string methodName = "LoadFromXml";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));

            try
            {
            #endregion
                var tempFolderMap = (Dictionary<string, ScanningFolder>)await LoadFromXmlInternal<Dictionary<string, ScanningFolder>>(savedFolders);
                var tempFolderFileMap = (Dictionary<string, SortedList>)await LoadFromXmlInternal<Dictionary<string, SortedList>>(savedFoldersFiles);
                var tempFileMap = (Dictionary<string, ScanningFile>)await LoadFromXmlInternal<Dictionary<string, ScanningFile>>(savedFileMap);
                var tempFileList = (SortedList)await LoadFromXmlInternal<SortedList>(savedFileList);
                var tempSavedSearches = (Dictionary<string, SortedList>)await LoadFromXmlInternal<Dictionary<string, SortedList>>(savedSearchesList);

                if (tempFolderMap != null)
                {
                    this.folderMap = tempFolderMap;
                }
                if (tempFolderFileMap != null)
                {
                    this.folderFileMap = tempFolderFileMap;
                }
                if (tempFileMap != null)
                {
                    this.fileMap = tempFileMap;
                }
                if (tempFileList != null)
                {
                    this.fileList = tempFileList;
                }
                if (tempSavedSearches != null)
                {
                    this.savedSearches = tempSavedSearches;
                }

                this.loaded = true;

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

        private bool dirty;

        public async Task SaveToXml()
        {
            #region start
            string methodName = "SaveToXml";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));

            try
            {
            #endregion

                if (this.dirty)
                {
                    Logging.Logger.Info(string.Format("{0}::{1} - Dirty", this.GetType().Name, methodName));

                    await SaveToXmlInternal<Dictionary<string, ScanningFolder>>(savedFolders, this.folderMap);
                    await SaveToXmlInternal<Dictionary<string, SortedList>>(savedFoldersFiles, this.folderFileMap);
                    await SaveToXmlInternal<Dictionary<string, ScanningFile>>(savedFileMap, this.fileMap);
                    await SaveToXmlInternal<SortedList>(savedFileList, this.fileList);
                    await SaveToXmlInternal<Dictionary<string, SortedList>>(savedSearchesList, this.savedSearches);
                }

                this.dirty = false;
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

        public async Task<bool> AddFileToLibrary(ScanningFolder folder, ScanningFile file)
        {
            ScanningFile scanningFile = null;
            bool fileAlreadExists = true;

            #region start
            string methodName = "AddFileToLibrary";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} {2} {3} - Start", this.GetType().Name, methodName, folder.Folder.Path, file.Path));

            try
            {
            #endregion

                string hash = ScanningFile.ComputeMD5(file.Path);

                if (this.fileMap.ContainsKey(hash))
                {
                    scanningFile = this.fileMap[hash];
                }
                else
                {
                    fileAlreadExists = false;
                    scanningFile = file;

                    lock (this.libraryLock)
                    {
                        this.fileList.AddFileToList(scanningFile);
                        this.fileMap[scanningFile.Hash] = scanningFile;
                        this.dirty = true;
                    }
                }

                lock (this.libraryLock)
                {
                    if (this.folderFileMap.ContainsKey(folder.Folder.Path))
                    {
                        SortedList list = this.folderFileMap[folder.Folder.Path];
                        if (!list.IsItemAlreadyInList(scanningFile))
                        {
                            list.AddFileToList(scanningFile);
                        }
                    }
                    else
                    {
                        SortedList list = new SortedList();
                        list.AddFileToList(scanningFile);
                        this.folderFileMap[folder.Folder.Path] = list;
                    }

                    this.dirty = true;
                }

                if (this.fileList.FileList.Count % 100 == 0)
                {
                    await this.SaveToXml();
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

            return fileAlreadExists;
        }

        public async Task<bool> AddFolderToLibrary(ScanningFolder folder)
        {
            bool folderAlreadExists = true;

            #region start
            string methodName = "AddFolderToLibrary";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} {2} - Start", this.GetType().Name, methodName, folder.Folder.Path));

            try
            {
            #endregion

                if (!this.folderMap.ContainsKey(folder.Folder.Path))
                {
                    folderAlreadExists = false;

                    lock (this.libraryLock)
                    {
                        this.folderMap[folder.Folder.Path] = folder;
                        this.dirty = true;
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

            return folderAlreadExists;
        }

        public ScanningFile GetFileFromLibrary(string hash)
        {
            Logging.Logger.Info(string.Format("{0}::{1} {2} - GetFileFromLibrary", this.GetType().Name, "GetFileFromLibrary", hash));

            lock (this.libraryLock)
            {
                if(this.fileMap.ContainsKey(hash))
                {
                    Logging.Logger.Info(string.Format("{0}::{1} {2} - Found file", this.GetType().Name, "GetFileFromLibrary", hash));
                    return this.fileMap[hash];
                }
                else
                {
                    return null;
                }
            }
        }

        public bool DoesFileExistInLibrary(string hash)
        {
            Logging.Logger.Info(string.Format("{0}::{1} {2} - DoesFileExistInLibrary", this.GetType().Name, "DoesFileExistInLibrary", hash));

            lock (this.libraryLock)
            {
                if (this.fileMap.ContainsKey(hash))
                {
                    Logging.Logger.Info(string.Format("{0}::{1} {2} - Found file", this.GetType().Name, "DoesFileExistInLibrary", hash));

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public SortedList GetFilesForFolder(StorageFolder folder, Sorting sorting)
        {
            Logging.Logger.Info(string.Format("{0}::{1} {2} {3} - GetFilesForFolder", this.GetType().Name, "GetFilesForFolder", folder.Path, sorting));

            SortedList sortedList = null;
            lock (this.libraryLock)
            {
                if(this.folderFileMap.ContainsKey(folder.Path) && this.loaded)
                {
                    Logging.Logger.Info(string.Format("{0}::{1} - Found files in cache", this.GetType().Name, "GetFilesForFolder"));
                    sortedList = folderFileMap[folder.Path];
                    return sortedList;
                }
            }

            //if (!this.folderFileMap.ContainsKey(folder.Path))
            {
                Logging.Logger.Info(string.Format("{0}::{1} - Di not find files in cache", this.GetType().Name, "GetFilesForFolder"));
                sortedList = new SortedList();
                //Async on purpose
                this.AddToList(sortedList, folder, sorting);
            }

            return sortedList;
        }

        private async void AddToList(SortedList sortedList, StorageFolder folder, Sorting sorting)
        {
            #region start
            string methodName = "AddToList";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} {2} {3} - Start", this.GetType().Name, methodName, folder.Path, sorting));

            try
            {
            #endregion

                List<Tuple<StorageFolder, StorageFile>> tempList = new List<Tuple<StorageFolder, StorageFile>>();
                await FileSystemHelper.GetAllFilesInFolder(folder, tempList);
                foreach (var file in tempList)
                {
                    ScanningFile scanningFile = new ScanningFile(file.Item1, file.Item2);
                    await scanningFile.ScanStorageFile();
                    sortedList.AddFileToList(scanningFile);
                }

                if (sorting != Sorting.None)
                {
                    sortedList.SortFiles(sorting);
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

        public List<string> GetSavedSearches()
        {
            lock (this.libraryLock)
            {

            }
            return null;
        }

        public async Task<List<ScanningFile>> GetSavedSearchResults()
        {
            lock (this.libraryLock)
            {

            }
            return null;
        }

        public List<ScanningFile> GetTopPlayed()
        {
            lock (this.libraryLock)
            {
                return this.fileList.GetFileList().OrderByDescending(x => x.PlayCount).ToList();
            }
        }

        public ScanningFolder GetSavedFolder(string folderPath)
        {
            if (this.folderMap.ContainsKey(folderPath))
            {
                return this.folderMap[folderPath];
            }
            else
            {
                throw new InvalidOperationException(string.Format("Folder: {0} not found.", folderPath));
            }
        }

        public List<ScanningFile> GetFavorites()
        {
            List<ScanningFile> favoriteFiles = new List<ScanningFile>();

            lock (this.libraryLock)
            {
                foreach(var file in this.fileList.GetFileList())
                {
                    if(file.Favorite)
                    {
                        favoriteFiles.Add(file);
                    }
                }
            }
            return favoriteFiles;
        }

        private void UpdatesSearchResults()
        {
            //Update results in a loop with configurable delay
        }
    }
}
