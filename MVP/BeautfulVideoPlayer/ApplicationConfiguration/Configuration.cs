using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace ApplicationConfiguration
{
    public sealed class Configuration
    {
        #region internal
        private static int imageWidth = 300;
        private static int imageHeight = 300;

        private static int category1Frames = 10;
        private static int category2Frames = 200;
        private static int category3Frames = 600;

        private static int category1Delay = 150;
        private static int category2Delay = 1;
        private static int category3Delay = 1;

        private static int category1StartPercent = 30;
        private static int category2StartPercent = 75;
        private static int category3StartPercent = 50;

        private static int animationCreationDelay = 2;
        private static int animationCreationDelayTimeout = 3;

        private static double frameInterval = 0.005;

        private static int loggingLines = 20000;

        private static int folderScanDelay = 10;
        private static int scanRetryTimes = 10;
        private static int scanRetryDelay = 2;

        #endregion

        #region Image properties
        public static int ImageWidth
        {
            get
            {
                return imageWidth;
            }
        }

        public static int ImageHeight
        {
            get
            {
                return imageHeight;
            }
        }
        #endregion

        #region Categories

        public static int Category1Frames
        {
            get
            {
                return category1Frames;
            }
        }

        public static int Category2Frames
        {
            get
            {
                return category2Frames;
            }
        }

        public static int Category3Frames
        {
            get
            {
                return category3Frames;
            }
        }

        public static int Category1Delay
        {
            get
            {
                return category1Delay;
            }
        }

        public static int Category2Delay
        {
            get
            {
                return category2Delay;
            }
        }

        public static int Category3Delay
        {
            get
            {
                return category3Delay;
            }
        }

        public static int Category1StartPercent
        {
            get
            {
                return category1StartPercent;
            }
        }

        public static int Category2StartPercent
        {
            get
            {
                return category2StartPercent;
            }
        }

        public static int Category3StartPercent
        {
            get
            {
                return category3StartPercent;
            }
        }
        #endregion

        #region Animation properties
        public static int AnimationCreationDelay
        {
            get
            {
                return animationCreationDelay;
            }
        }

        public static int AnimationCreationDelayTimeout
        {
            get
            {
                return animationCreationDelayTimeout;
            }
        }

        public static double FrameInterval
        {
            get
            {
                return frameInterval;
            }
        }

        #endregion

        #region Logging properties
        public static int LoggingLines
        {
            get
            {
                return loggingLines;
            }
        }
        #endregion

        #region Folder Infomration

        public int FolderScanDelay
        {
            get
            {
                return folderScanDelay;
            }
        }

        public int ScanRetryTimes
        {
            get
            {
                return scanRetryTimes;
            }
        }

        public int ScanRetryDelay
        {
            get
            {
                return scanRetryDelay;
            }
        }

        #endregion
    }
}
