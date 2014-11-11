using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFolders
{
    abstract public class Scanner<T> : IScanner
    {
        internal FileLibrary fileLibrary;
        internal ConcurrentQueue<T> queue;
        bool scanStarted = false;

        public void Initialize(FileLibrary fileLibrary)
        {
            scanStarted = false;
            this.fileLibrary = fileLibrary;
            this.queue = new ConcurrentQueue<T>();
            ProcessFiles();
        }

        public void AddItemToScan(T item)
        {
            if(!IsItemAlreadyScanned(item))
            {
                this.queue.Enqueue(item);
                this.StartScan();
            }
        }

        public void StartScan()
        {
            this.scanStarted = true;
        }
        public void PauseScan()
        {
            this.scanStarted = false;
        }
        public void ResumeScan()
        {
            this.scanStarted = true;
        }
        public void StopScan()
        {
            this.scanStarted = false;
            T ignored; while (this.queue.TryDequeue(out ignored));
        }

        async void ProcessFiles()
        {
            while(true)
            {
                if(this.scanStarted)
                {
                    if (this.queue.Count == 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5));
                    }
                    else
                    {
                        T item;
                        if(this.queue.TryDequeue(out item))
                        {
                            if (!IsItemAlreadyScanned(item))
                            {
                                await ProcessQueueItem(item);
                            }
                        }
                    }
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
        }

        public abstract Task ProcessQueueItem(T item);

        public abstract bool IsItemAlreadyScanned(T item);
    }
}
