namespace IncrementalLoadingSample.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Windows.Foundation;
    using Windows.UI.Xaml.Data;

    internal abstract class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        #region Constructors

        protected IncrementalLoadingCollection()
        {
        }

        protected IncrementalLoadingCollection( IEnumerable<T> collection )
            : base( collection )
        {
        }

        #endregion // Constructors
        
        #region Implementation of ISupportIncrementalLoading

        public abstract IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync( uint count );

        public abstract bool HasMoreItems { get; }

        #endregion // Implementation of ISupportIncrementalLoading
    }
}
