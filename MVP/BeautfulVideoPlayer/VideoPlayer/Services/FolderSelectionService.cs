using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App4.Abstractions;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace App4.Services
{
  class FolderSelectionService : IFolderSelectionService
  {
    public Task<StorageFolder> SelectStorageFolderAsync()
    {
      FolderPicker picker = new FolderPicker();
      picker.FileTypeFilter.Add("*");
      return (picker.PickSingleFolderAsync().AsTask());
    }
  }
}
