using Windows.Storage;
namespace IncrementalLoadingSample.Data
{
    internal class SearchResult
    {
        public SearchResult(StorageFile file)
        {
            this.file = file;
        }

        public StorageFile file 
        { 
            get; 
            private set; 
        }
    }
}
