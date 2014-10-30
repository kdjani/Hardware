using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Logging
{
    /// <summary>
    /// This is an advanced useage, where you want to intercept the logging messages and devert them somewhere
    /// besides ETW.
    /// </summary>
    sealed class StorageFileEventListener : EventListener
    {

        /// <summary>
        /// Name of the current event listener
        /// </summary>
        private string m_Name;

        private object myLock;

        private StorageFile m_StorageFile;

        /// <summary>
        /// The format to be used by logging.
        /// </summary>
        private string m_Format = "{0:yyyy-MM-dd HH\\:mm\\:ss\\:ffff} {1} Pid: {2} Message: '{3}'";

        private SemaphoreSlim m_SemaphoreSlim = new SemaphoreSlim(1);

        private static List<string> lines;

        public StorageFileEventListener(string name)
        {
            this.myLock = new object();
            this.m_Name = name;

            Debug.WriteLine("StorageFileEventListener for {0} has name {1}", GetHashCode(), name);

            lines = new List<string>();
        }

        private async Task AssignLocalFile()
        {
             m_StorageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(m_Name.Replace(" ", "_") + Guid.NewGuid().ToString().Replace("-", "") + ".log",
                                                                                      CreationCollisionOption.OpenIfExists);
        }

        private async void WriteToFile(List<string> logData)
        {
            await m_SemaphoreSlim.WaitAsync();
            await this.AssignLocalFile();

            await Task.Run(async () =>
                                     {
                                         try
                                         {
                                             await FileIO.AppendLinesAsync(m_StorageFile, logData); 
                                             await Task.Delay(TimeSpan.FromSeconds(5));
                                         }
                                         finally
                                         {
                                             m_SemaphoreSlim.Release();
                                         }
                                     });
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            //if (m_StorageFile == null) return;

            var newFormatedLine = string.Format(m_Format, DateTime.Now, eventData.Level, Environment.CurrentManagedThreadId, eventData.Payload[0]);

            lock (myLock)
            {
                lines.Add(newFormatedLine);
            }

            if (eventData.Level == EventLevel.Critical || lines.Count % 5000 == 0)
            {
                List<string> logData = new List<string>();
                lock (myLock)
                {
                    foreach (var log in lines)
                    {
                        logData.Add(log);
                    }

                    lines.Clear();
                }
                
                WriteToFile(logData);
            }
        }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
        }
    }
}