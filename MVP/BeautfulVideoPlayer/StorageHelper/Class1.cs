using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace StorageHelper
{
    public class FileSystemHelper
    {
        public static async Task GetAllFilesInFolder(StorageFolder folder, List<StorageFile> tempList)
        {
            StorageFolder fold = folder;

            var items = await fold.GetItemsAsync();

            foreach (var item in items)
            {
                if (item.GetType() == typeof(StorageFile))
                {
                    tempList.Add(item as StorageFile);
                }
                else
                {
                    await GetAllFilesInFolder(item as StorageFolder, tempList);
                }
            }
        }
        private void SaveFileToCache(StorageFile file)
        {
            // TODO
        }
        private StorageFile GetFileFromCache(string hash)
        {
            //TODO
            return null;
        }
    }
}
