using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

namespace VideoFolders
{
    [DataContractAttribute]
    public class SortedList
    {
        private ObservableCollection<ScanningFile> fileList;
        private Sorting sorting;
        private object listLock;
        private bool sorted;
        private Dictionary<string, ScanningFile> fileMap;

        [DataMember(Order = 1)]
        public List<ScanningFile> FileListToSave
        {
            get
            {
                return this.fileList.ToList();
            }

            set
            {
                this.fileList = ToObservableCollection <ScanningFile> (value);
            }
        }

        private static ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> thisCollection)
        {
            if (thisCollection == null) return null;
            var oc = new ObservableCollection<T>();

            foreach (var item in thisCollection)
            {
                oc.Add(item);
            }

            return oc;
        }

        //[DataMember(Order = 1)]
        public ObservableCollection<ScanningFile> FileList
        {
            get
            {
                return this.fileList;
            }

            set
            {
                this.fileList = value;
            }
        }

        [DataMember(Order = 2)]
        public Dictionary<string, ScanningFile> FileMap
        {
            get
            {
                return this.fileMap;
            }

            set
            {
                this.fileMap = value;
            }
        }


        [DataMember(Order = 3)]
        public Sorting ListSorting
        {
            get
            {
                return this.sorting;
            }

            set
            {
                this.sorting = value;
            }
        }

        public void LoadFromXml(XmlDocument document)
        {
            lock (this.listLock)
            {
            }
        }

        public XmlDocument SaveToXml()
        {
            lock (this.listLock)
            {
            }
            XmlDocument document = new XmlDocument();
            //TODO
            return document;
        }

        public SortedList()
        {
            InitializeClass();
        }

        [OnDeserialized()]
        private void OnDeserialized(StreamingContext c)
        {
            InitializeClass();
        }

        private void InitializeClass()
        {
            this.listLock = new object();
            this.sorting = Sorting.None;
            this.sorted = false;
            if (this.fileList == null)
            {
                this.fileList = new ObservableCollection<ScanningFile>();
            }
            if (this.fileMap == null)
            {
                this.fileMap = new Dictionary<string, ScanningFile>();
            }
            this.filePathToListIndex = new Dictionary<string, int>();
            this.PopulateDictionary();
        }

        private void PopulateDictionary()
        {
            for (int i = 0; i < this.fileList.Count; i++)
            {
                this.filePathToListIndex[this.fileList[i].Path] = i;
            }
        }

        public void AddFileToList(ScanningFile file)
        {
            lock (this.listLock)
            {
                this.fileList.Add(file);
                this.filePathToListIndex[file.Path] = this.fileList.Count - 1;

                if(!this.fileMap.ContainsKey(file.Hash))
                {
                    this.fileMap[file.Hash] = file;
                }
            }
        }

        private Dictionary<string, int> filePathToListIndex;

        public ScanningFile GetPreviousFile(ScanningFile currentFile)
        {
            if (!this.filePathToListIndex.ContainsKey(currentFile.Path))
            {
                throw new InvalidOperationException();
            }

            int index = this.filePathToListIndex[currentFile.Path];

            if (index - 1 >= 0)
            {
                return this.fileList[index - 1];
            }
            else
            {
                return null;
            }
        }

        public ScanningFile GetNextFile(ScanningFile currentFile)
        {
            if (!this.filePathToListIndex.ContainsKey(currentFile.Path))
            {
                throw new InvalidOperationException();
            }

            int index = this.filePathToListIndex[currentFile.Path];

            if (index + 1 < this.fileList.Count)
            {
                return this.fileList[index + 1];
            }
            else
            {
                return null;
            }
        }

        public bool IsItemAlreadyInList(ScanningFile file)
        {
            if(this.fileMap.ContainsKey(file.Hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public ObservableCollection<ScanningFile> GetFileList()
        {
            lock (this.listLock)
            {
                return this.fileList;            
            }
        }

        public Sorting Sort
        {
            set
            {
                if(value != this.sorting)
                {
                    this.sorting = value;
                }
            }
        }

        public SortedList(ObservableCollection<ScanningFile> files)
        {
            this.listLock = new object();
            sorted = false;

            lock (this.listLock)
            {
                this.fileList = files;
            }
        }

        public void SortFiles(Sorting sorting)
        {
            lock (this.listLock)
            {
                this.Sort = sorting;
            }
        }
    }
}
