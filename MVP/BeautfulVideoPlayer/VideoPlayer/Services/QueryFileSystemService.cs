using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App4.Abstractions;
using Windows.Storage;
using Windows.Storage.BulkAccess;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;

namespace App4.Services
{
  class QueryFileSystemService : IQueryFileSystem
  {
    public async Task<IReadOnlyList<StorageFolder>> QuerySubFoldersAsync(
      StorageFolder folder)
    {
      var queryOptions = new QueryOptions(CommonFolderQuery.DefaultQuery);
      queryOptions.FolderDepth = FolderDepth.Shallow;
      queryOptions.IndexerOption = IndexerOption.UseIndexerWhenAvailable;

      var folderQuery = folder.CreateFolderQueryWithOptions(queryOptions);
      var folders = await folderQuery.GetFoldersAsync();

      return (folders);
    }
    public object QueryImageFilesAsync(StorageFolder folder)
    {
      var queryOptions = new QueryOptions(CommonFileQuery.DefaultQuery,
        FILE_EXTENSIONS);

      queryOptions.FolderDepth = FolderDepth.Shallow;

      queryOptions.SetThumbnailPrefetch(ThumbnailMode.PicturesView, THUMBNAIL_SIZE,
        ThumbnailOptions.ResizeThumbnail);

      var fileQuery = folder.CreateFileQueryWithOptions(queryOptions);

      FileInformationFactory factory = new FileInformationFactory(
        fileQuery,
        ThumbnailMode.PicturesView,
        THUMBNAIL_SIZE,
        ThumbnailOptions.ResizeThumbnail);

      return (factory.GetVirtualizedFilesVector());
    }
    const int THUMBNAIL_SIZE = 192;
    static readonly string[] FILE_EXTENSIONS = { ".png" };
  }
}
