using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace VideoFolders
{
    public class FileSystemHelper
    {
        public static async Task GetAllFilesInFolder(StorageFolder folder, List<Tuple<StorageFolder,StorageFile>> tempList)
        {
            StorageFolder fold = folder;

            var items = await fold.GetItemsAsync();

            foreach (var item in items)
            {
                if (item.GetType() == typeof(StorageFile))
                {
                    tempList.Add(new Tuple<StorageFolder, StorageFile>(folder, item as StorageFile));
                }
                else
                {
                    await GetAllFilesInFolder(item as StorageFolder, tempList);
                }
            }
        }

        public static async Task GetAllFoldersInFolder(StorageFolder folder, List<StorageFolder> tempList)
        {
            StorageFolder fold = folder;

            var items = await fold.GetItemsAsync();

            foreach (var item in items)
            {
                if (item.GetType() == typeof(StorageFolder))
                {
                    tempList.Add(item as StorageFolder);
                    await GetAllFoldersInFolder(item as StorageFolder, tempList);
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
