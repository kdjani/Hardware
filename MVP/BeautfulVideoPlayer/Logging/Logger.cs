using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Logging
{
    public sealed class Logger
    {
        private static EventListener verboseListener;
        //private static EventListener informationListener;

        public static void SetupLogging(string filename)
        {
            verboseListener = new StorageFileEventListener(filename + "_Verbose");
            //informationListener = new StorageFileEventListener(filename + "_Information");

            verboseListener.EnableEvents(MetroEventSource.Log, EventLevel.Verbose);
            //informationListener.EnableEvents(MetroEventSource.Log, EventLevel.Informational);
        }

        public static void Debug(string message)
        {
            MetroEventSource.Log.Debug(message);
        }

        public static void Info(string message)
        {
            MetroEventSource.Log.Info(message);
        }

        public static void Warn(string message)
        {
            MetroEventSource.Log.Warn(message);
        }

        public static void Error(string message)
        {
            MetroEventSource.Log.Error(message);
        }

        public static void Critical(string message)
        {
            MetroEventSource.Log.Critical(message);
        }
    }
}
