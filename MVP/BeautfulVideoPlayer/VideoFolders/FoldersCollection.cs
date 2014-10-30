using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace VideoFolders
{
    public sealed class FoldersCollection: IList<StorageFolder>
    {
        private readonly IList<StorageFolder> _list = new List<StorageFolder>();

        #region Implementation of IEnumerable

        public IEnumerator<StorageFolder> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection<StorageFolder>

        public void Add(StorageFolder item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(StorageFolder item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(StorageFolder[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(StorageFolder item)
        {
            return _list.Remove(item);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

        #endregion

        #region Implementation of IList<StorageFolder>

        public int IndexOf(StorageFolder item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, StorageFolder item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public StorageFolder this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        #endregion
    }
}
