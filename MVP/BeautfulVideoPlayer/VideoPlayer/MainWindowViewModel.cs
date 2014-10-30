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
    using App4.Common;
    using App4.Abstractions;
    using App4.Items;
    using TileAFile.Services;
    using Windows.Storage;
    using Windows.Storage.BulkAccess;
    using TileAFile.Services;
    using System.Linq;
    using VideoFolders;

    internal class MainWindowViewModel : BindableBase, IViewModelInitialisation
    {
        enum CommandIndex
        {
            FolderInvoked = 0,
            AddFolder,
            RemoveFolder,
            Back,
            ImageInvoked
        }

        //private FileCollection gifList;

        VideoFolders scanner;
        public static AnimationLibrary.AnimationLibrary library;

        public FileCollection fileCollection;
            
        public MainWindowViewModel()
        {
            library = new AnimationLibrary.AnimationLibrary();
            this.scanner = new VideoFolders(library);

            this.fileCollection = new FileCollection(this.scanner);

            //this.gifList = fileCollection.FileList;

            ///////////////////
            this._commands = new Dictionary<int, ICommand>()
      {
          {
            (int)CommandIndex.FolderInvoked, 
            new SimpleCommand(this.OnFolderInvoked)
          },          
          {
            (int)CommandIndex.AddFolder,
            new SimpleCommand(this.OnAddInvoked)
          },          
          {
            (int)CommandIndex.RemoveFolder,
            new SimpleCommand(this.OnRemoveInvoked)
          },          
          {
            (int)CommandIndex.Back,
            new SimpleCommand(this.OnBack)
          },          
          {
            (int)CommandIndex.ImageInvoked,
            new SimpleCommand(this.OnImageInvoked)
          }
      };

            this._upItemEntry = new CommandableItem[]
      {
        new CommandableItem("Go Up", new SimpleCommand(this.OnUp))
      };

            this._topLevelFolders = new ObservableCollection<CommandableItem>();

            this._parentFolders = new Stack<StorageFolder>();

            this.PopulateDetailsAsync();

            //////////////////
        }



        /// <summary>
        /// ///////////////////////////////////
        /// </summary>
        /// 

        T GetService<T>()
        {
            return ((T)ServiceProvider.Provider.GetService(typeof(T)));
        }

        public bool CanRemoveFolder
        {
            get
            {
                return (this.IsTop &&
                  (this.SelectedFolder != null) &&
                  (this.SelectedFolder.StorageFolder.Path != KnownFolders.VideosLibrary.Path));
            }
        }
        public FileInformation SelectedFile
        {
            get
            {
                return (this._selectedFile);
            }
            set
            {
                base.SetProperty(ref this._selectedFile, value);
            }
        }
        public FolderItem SelectedFolder
        {
            get
            {
                return (this._selectedFolder);
            }
            set
            {
                base.SetProperty(ref this._selectedFolder, value);
                this.RaiseCanRemoveFolderChanged();
            }
        }
        void RaiseCanRemoveFolderChanged()
        {
            base.OnPropertyChanged("CanRemoveFolder");
        }
        public ICommand BackCommand
        {
            get
            {
                return (this._commands[(int)CommandIndex.Back]);
            }
        }
        public ICommand AddFolderCommand
        {
            get
            {
                return (this._commands[(int)CommandIndex.AddFolder]);
            }
        }
        public ICommand RemoveFolderCommand
        {
            get
            {
                return (this._commands[(int)CommandIndex.RemoveFolder]);
            }
        }
        public ICommand ImageInvokedCommand
        {
            get
            {
                return (this._commands[(int)CommandIndex.ImageInvoked]);
            }
        }
        public bool IsFileViewVisible
        {
            get
            {
                return (this._isFileViewVisible);
            }
            set
            {
                base.SetProperty(ref this._isFileViewVisible, value);
            }
        }
        public ObservableCollection<CommandableItem> Folders
        {
            get
            {
                return (this._folders);
            }
            private set
            {
                base.SetProperty(ref this._folders, (ObservableCollection<CommandableItem>)value);
            }
        }
        public object Files
        {
            get
            {
                return (this._files);
            }
            private set
            {
                base.SetProperty(ref this._files, value);
            }
        }
        public object FlipViewFiles
        {
            get
            {
                return (this._flipViewFiles);
            }
            set
            {
                base.SetProperty(ref this._flipViewFiles, value);
            }
        }
        public bool IsTop
        {
            get
            {
                return (this._currentFolder == null);
            }
        }
        async public void Initialise()
        {
            var folderService = this.GetService<IPersistFolderAccess>();
            var folders = folderService.RetrieveAccessibleFoldersAsync();

            this.AddTopLevelFolder(KnownFolders.VideosLibrary);

            if (folders != null && folders.Count != 0)
            {
                scanner.AddNewVideoFolders(folders);

                foreach (var folder in folders)
                {
                    this.AddTopLevelFolder(folder);
                }
            }
        }
        void AddTopLevelFolder(StorageFolder folder)
        {
            scanner.AddNewVideoFolder(folder);
            this._topLevelFolders.Add(new FolderItem(folder,
            this._commands[(int)CommandIndex.FolderInvoked]));
        }
        ObservableCollection<CommandableItem> TopLevelFolders
        {
            get
            {
                return (this._topLevelFolders);
            }
        }
        void PopulateDetailsAsync()
        {
            if (this.IsTop)
            {
                this.Folders = this.TopLevelFolders;
            }
            else
            {
                this.QueryCurrentFolders();
                this.QueryCurrentFiles();
            }
        }
        async void QueryCurrentFolders()
        {
            this.Folders = null;

            var queryService = this.GetService<IQueryFileSystem>();

            var folders = await queryService.QuerySubFoldersAsync(this._currentFolder);

            var folderEntries = folders.Select(f =>
              (CommandableItem)(
                new FolderItem(f, this._commands[(int)CommandIndex.FolderInvoked])));

            this.Folders = new ObservableCollection<CommandableItem>(
                this._upItemEntry.Union(folderEntries)
              );

            this.fileCollection.UpdateFolder(this._currentFolder);
            scanner.AddNewVideoFolder(this._currentFolder);

        }
        void QueryCurrentFiles()
        {
            this.Files = null;

            var queryService = this.GetService<IQueryFileSystem>();

            this.Files = queryService.QueryImageFilesAsync(this._currentFolder);
        }
        void OnBack()
        {
            this.FlipViewFiles = null;
            this.SwitchViews();
        }
        void OnImageInvoked(object invokedImage)
        {
            this.SwitchViews();
            this.FlipViewFiles = this.Files;
            this.SelectedFile = (FileInformation)invokedImage;
        }
        void OnFolderInvoked(object param)
        {
            if (!this.IsTop)
            {
                this._parentFolders.Push(this._currentFolder);
            }
            this._currentFolder = ((FolderItem)param).StorageFolder;
            this.PopulateDetailsAsync();
            this.RaiseCanRemoveFolderChanged();
        }
        async void OnAddInvoked()
        {
            var folderPickingService = this.GetService<IFolderSelectionService>();
            var persistedFoldersService = this.GetService<IPersistFolderAccess>();

            var folder = await folderPickingService.SelectStorageFolderAsync();

            if (folder != null)
            {
                this.AddTopLevelFolder(folder);
                persistedFoldersService.StoreAccessToUserFolder(folder);
            }
        }
        async void OnRemoveInvoked()
        {
            if (this.SelectedFolder != null)
            {
                var persistedFoldersService = this.GetService<IPersistFolderAccess>();
                await persistedFoldersService.RevokeAccessToUserFolderAsync(this.SelectedFolder.StorageFolder);
                this._topLevelFolders.Remove(this.SelectedFolder);
                this.SelectedFolder = null;
                this.RaiseCanRemoveFolderChanged();
            }
        }
        void OnUp()
        {
            this._currentFolder = (this._parentFolders.Count != 0) ?
              this._parentFolders.Pop() : null;

            this.PopulateDetailsAsync();

            this.RaiseCanRemoveFolderChanged();
        }
        void SwitchViews()
        {
            this.IsFileViewVisible = !this.IsFileViewVisible;
        }
        object _files;
        object _flipViewFiles;
        bool _isFileViewVisible;
        Dictionary<int, ICommand> _commands;
        FileInformation _selectedFile;
        FolderItem _selectedFolder;
        Stack<StorageFolder> _parentFolders;
        ObservableCollection<CommandableItem> _folders;
        StorageFolder _currentFolder;
        ObservableCollection<CommandableItem> _topLevelFolders;
        CommandableItem[] _upItemEntry;

        ///////////////////////////////////////////////////

        public ObservableCollection<ScanningFile> Gifs
        {
            get
            {
                return this.fileCollection.FileList;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
