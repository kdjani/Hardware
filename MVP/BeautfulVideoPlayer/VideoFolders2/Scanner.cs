using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoFolders
{
    abstract class Scanner<T> : IScanner
    {
        internal FileLibrary fileLibrary;
        internal ConcurrentQueue<T> queue;
        bool scanStarted = false;

        public void Initialize(FileLibrary fileLibrary)
        {
            scanStarted = false;
            this.fileLibrary = fileLibrary;
            ProcessFiles();
        }

        public void AddItemToScan(T item)
        {
            if(!IsItemAlreadyScanned(item))
            {
                this.queue.Enqueue(item);
            }
        }

        void StartScan(FileLibrary fileLibrary)
        {
            this.scanStarted = true;
        }
        void PauseScan(FileLibrary fileLibrary)
        {
            this.scanStarted = false;
        }
        void ResumeScan(FileLibrary fileLibrary)
        {
            this.scanStarted = true;
        }
        void StopScan(FileLibrary fileLibrary)
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
                            await ProcessQueueItem(item);
                        }
                    }
                }
                else
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
        }

        void AddItemToScan();

        public abstract async Task ProcessQueueItem(T item)
        {
            await Task.Delay(TimeSpan.FromSeconds(0));
        }

        public abstract bool IsItemAlreadyScanned(T item)
        {
            throw new InvalidOperationException("Abstract method called.");
        }
    }
}
