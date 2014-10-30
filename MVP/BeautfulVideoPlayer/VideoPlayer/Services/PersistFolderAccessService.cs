using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App4.Abstractions;
using Windows.Storage;
using Windows.Storage.AccessCache;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;

namespace App4.Services
{

    public class PersistedFolder
    {
        private string path;
        private string tokrn;

        public PersistedFolder()
        {
        }
        public PersistedFolder(string path, string token)
        {
            this.tokrn = token;
            this.path = path;
        }

        [XmlAttribute]
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

        [XmlAttribute]
        public string Token
        {
            get
            {
                return this.tokrn;
            }
            set
            {
                this.tokrn = value;
            }
        }
    }
  class PersistFolderAccessService : IPersistFolderAccess
  {
    private List<PersistedFolder> savedFolderPaths;
    private List<StorageFolder> savedFoldders; 

    public PersistFolderAccessService()
    {
        this.savedFolderPaths = new List<PersistedFolder>();
        this.savedFoldders = new List<StorageFolder>();
        LoadSavedFolders();
    }

      private async void LoadSavedFolders()
      {
          await this.DeserializeFolderList();

          foreach(var folder in this.savedFolderPaths)
          {
              try
              {
                  var saved = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(folder.Token);
                  //this.savedFoldders.Add(saved);
              }
              catch (Exception e)
              {

              }
          }
      }


    private async Task SerializeFolderList()
    {
        var serializer = new XmlSerializer(typeof(List<PersistedFolder>));

        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        StorageFile file = await localFolder.CreateFileAsync("SavedKnownFolders.xml", CreationCollisionOption.ReplaceExisting);

        using (var stream = await file.OpenStreamForWriteAsync())
        {
            XmlSerializer xmlIzer = new XmlSerializer(typeof(List<PersistedFolder>));
            xmlIzer.Serialize(stream, this.savedFolderPaths);
        }
    }

    private async Task DeserializeFolderList()
    {
        List<string> ausgabe = new List<string>();
        XmlSerializer serializer = new XmlSerializer(typeof(List<PersistedFolder>));

        StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        StorageFile sampleFile = await storageFolder.CreateFileAsync("SavedKnownFolders.xml", CreationCollisionOption.OpenIfExists);

        var file = await sampleFile.OpenAsync(FileAccessMode.Read);

        try
        {
            this.savedFolderPaths = (List<PersistedFolder>)serializer.Deserialize(file.AsStreamForRead());
        }
        catch (Exception e)
        {

        }

        file.Dispose();
    }
     
    public async Task StoreAccessToUserFolder(StorageFolder folder)
    {
      string token = StorageApplicationPermissions.FutureAccessList.Add(folder);
      this.savedFolderPaths.Add(new PersistedFolder(folder.Path, token));
      await this.SerializeFolderList();
    }
    public async Task RevokeAccessToUserFolderAsync(StorageFolder folder)
    {
        var persisted = this.savedFolderPaths.SingleOrDefault(x => x.Path == folder.Path);
        if (persisted != null)
        {
            this.savedFolderPaths.Remove(persisted);
        }

        var item = this.savedFoldders.SingleOrDefault(x => x.Path == folder.Path);
        if (item != null)
        {
            this.savedFoldders.Remove(item);
        }

        await this.SerializeFolderList();   
    }
    public List<StorageFolder> RetrieveAccessibleFoldersAsync()
    {
        return this.savedFoldders;
    }
  }
}
