using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace App4.Abstractions
{
  interface IPersistFolderAccess
  {
    Task StoreAccessToUserFolder(StorageFolder folder);
    Task RevokeAccessToUserFolderAsync(StorageFolder folder);
    List<StorageFolder> RetrieveAccessibleFoldersAsync();
  }
}
