using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace App4.Abstractions
{
  interface IFolderSelectionService
  {
    Task<StorageFolder> SelectStorageFolderAsync(); 
  }
}
