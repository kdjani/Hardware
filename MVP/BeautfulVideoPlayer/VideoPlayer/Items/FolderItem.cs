namespace App4.Items
{
  using System.Windows.Input;
  using Windows.Storage;

  class FolderItem : CommandableItem
  {
    public FolderItem(StorageFolder folder, ICommand command)
      : base(null, command)
    {
      this.StorageFolder = folder;
    }
    public override string Name
    {
      get
      {
        return (this.StorageFolder.Name);
      }
    }

    public string DetailedName
    {
        get
        {
            return (string.Format("{0} ({1})", this.StorageFolder.Name, string.IsNullOrEmpty(this.StorageFolder.Path) ? "*" : this.StorageFolder.Path));
        }
    }

    public StorageFolder StorageFolder { get; private set; }
  }
}
