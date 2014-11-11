using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media.Animation;
using Windows.Storage.Streams;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using System.Threading.Tasks;
using Windows.Foundation;
using IncrementalLoadingSample;
using VideoFolders;

namespace TriGemini.Controls
{
    /// <summary>
    ///  AnimationImage control
    /// </summary>
    public sealed partial class AnimationImage : UserControl
    {
        #region Private Fields
        private static readonly DependencyProperty _videoFile = DependencyProperty.Register
        (
          "VideoFile", typeof(ScanningFile), typeof(AnimationImage), new PropertyMetadata(String.Empty, VideoFileChanged)
        );

        private readonly List<WriteableBitmap> _bitmapFrames = new List<WriteableBitmap>();
        WriteableBitmap _bitmapFrame;
        private bool _playOnLoad = true;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the url of the image e.g. "/Assets/MyAnimation.gif"
        /// </summary>
        public ScanningFile VideoFile
        {
            get
            {
                return (ScanningFile)GetValue(_videoFile);
            }
            set
            {
                SetValue(_videoFile, value);
            }
        }

        private List<WriteableBitmap> writeableBitmap;
        private List<BitmapFrame> listOfFrames;
        private BitmapTransform bitmapTransform;
        private List<DiscreteObjectKeyFrame> keyFrame;
        DiscreteObjectKeyFrame singleKeyFrame;
        private string hash;

        public bool PlayOnLoad
        {
            get
            {
                return _playOnLoad;
            }
            set
            {
                _playOnLoad = value;
            }
        }
        #endregion

        #region Constructors
        public AnimationImage()
        {
            #region start
            string methodName = "AnimationImage";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));

            try
            {
            #endregion
                this.InitializeComponent();
                this.imageChangedLock = new object();
                SetupAnimationDataStructures();
                anim = new ObjectAnimationUsingKeyFrames();
                anim2 = new ObjectAnimationUsingKeyFrames();
                keyFrame = new List<DiscreteObjectKeyFrame>();
                singleKeyFrame = new DiscreteObjectKeyFrame();

                countingLock = new object();

                for (int i = 0; i < ApplicationConfiguration.Configuration.Category1Frames; i++)
                {
                    keyFrame.Add(new DiscreteObjectKeyFrame());
                }

                ts = new TimeSpan();

                this.animationLoaded = false;

                WatchForAvaialableAnimations();

                this.thumbnailMode = true;
            #region end
            }
            catch (Exception e)
            {
                bSucceeded = false;
                exception = e;
            }
            finally
            {
                Logging.Logger.Info(string.Format("{0}::{1} - Complete", this.GetType().Name, methodName));
            }

            if (!bSucceeded)
            {
                Logging.Logger.Critical(string.Format("{0}::{1} - Failed", this.GetType().Name, exception.ToString()));
                throw exception;
            }
            #endregion
        }


        async private void SetupAnimationDataStructures()
        {
            #region start
            string methodName = "SetupAnimationDataStructures";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));

            try
            {
            #endregion
                this.writeableBitmap = new List<WriteableBitmap>();

                for (int i = 0; i < ApplicationConfiguration.Configuration.Category1Frames; i++)
                {
                    this.writeableBitmap.Add(new WriteableBitmap(ApplicationConfiguration.Configuration.ImageWidth, ApplicationConfiguration.Configuration.ImageHeight));
                }

                this.bitmapTransform = new BitmapTransform();

                listOfFrames = new List<BitmapFrame>();
            #region end
            }
            catch (Exception e)
            {
                bSucceeded = false;
                exception = e;
            }
            finally
            {
                Logging.Logger.Info(string.Format("{0}::{1} - Complete", this.GetType().Name, methodName));
            }

            if (!bSucceeded)
            {
                Logging.Logger.Critical(string.Format("{0}::{1} - Failed", this.GetType().Name, exception.ToString()));
                await Task.Delay(TimeSpan.FromSeconds(5));
                throw exception;
            }
            #endregion
        }

        private ObjectAnimationUsingKeyFrames anim;
        private ObjectAnimationUsingKeyFrames anim2;

        private TimeSpan ts;

        private bool animationLoaded;

        #endregion

        #region Private Methods

        private Point oldScreenCoords;

        private object countingLock;
        private static int maxConcurrentAnimationLoads = 1;
        private static int currentConcurrentAnimationLoads = 0;

        private object imageChangedLock;

        private async void WatchForAvaialableAnimations()
        {
            #region start
            string methodName = "WatchForAvaialableAnimations";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));

            try
            {
            #endregion
                oldScreenCoords = new Point(0,0);
                while (true)
                {
                    bool positionChanged = true;

                    await Task.Delay(TimeSpan.FromSeconds(3)); // TODO

                    var ttv = image.TransformToVisual(Window.Current.Content);
                    Point screenCoords = ttv.TransformPoint(new Point(0, 0));

                    if (oldScreenCoords.X == 0 && oldScreenCoords.Y == 0)
                    {
                        // First time
                        oldScreenCoords = screenCoords;
                    }
                    else
                    {
                        if (oldScreenCoords.X != screenCoords.X || oldScreenCoords.Y != screenCoords.Y)
                        {
                            oldScreenCoords = screenCoords;
                        }
                        else //same as last time, so we are not moving
                        {
                            positionChanged = false;
                        }
                    }

                    bool isLessthanMaxConcurrent = false;

                    lock (countingLock)
                    {
                        if(currentConcurrentAnimationLoads < maxConcurrentAnimationLoads)
                        {
                            if (!string.IsNullOrEmpty(this.hash) && !this.animationLoaded && !positionChanged)
                            {
                                if (IncrementalLoadingSample.MainPage.library != null)
                                {
                                    bool itemExists = IncrementalLoadingSample.MainPage.library.DoesItemExist(hash, AnimationLibrary.AnimationType.Category1Animation);
                                    if (itemExists)
                                    {
                                        currentConcurrentAnimationLoads++;
                                        isLessthanMaxConcurrent = true;
                                    }
                                }
                            }
                        }
                    }


                    if (isLessthanMaxConcurrent)
                    {
                        //if (IncrementalLoadingSample.MainPage.library != null)
                        {
                            bool itemExists = IncrementalLoadingSample.MainPage.library.DoesItemExist(hash, AnimationLibrary.AnimationType.Category1Animation);
                            if (itemExists)
                            {
                                AnimationLibrary.AnimationItem animation = IncrementalLoadingSample.MainPage.library.GetItem(hash, AnimationLibrary.AnimationType.Category1Animation);

                                if (_bitmapFrames != null)
                                {
                                    _bitmapFrames.Clear();
                                    StorageFolder storageFolder = null;
                                    storageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync(animation.AnimationPath);
                                    IReadOnlyList<StorageFile> fileList = await storageFolder.GetFilesAsync();

                                    int frameIndex = 0;

                                    foreach (StorageFile sampleFile in fileList)
                                    {
                                        //await Task.Delay(TimeSpan.FromMilliseconds(100)); // TODO

                                        if (frameIndex >= ApplicationConfiguration.Configuration.Category1Frames)
                                        {
                                            break;
                                        }

                                        using (var res = await sampleFile.OpenAsync(FileAccessMode.Read))
                                        {
                                            var decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.PngDecoderId, res);
                                            var frame = await decoder.GetFrameAsync(0);
                                            //var writeableBitmap = new WriteableBitmap((int)decoder.OrientedPixelWidth, (int)decoder.OrientedPixelHeight);

                                            //  Extract the pixel data and fill the WriteableBitmap with them
                                            var bitmapTransform = new BitmapTransform();
                                            var pixelDataProvider = await frame.GetPixelDataAsync(BitmapPixelFormat.Bgra8, decoder.BitmapAlphaMode, bitmapTransform, ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);
                                            var pixels = pixelDataProvider.DetachPixelData();

                                            writeableBitmap[frameIndex].Invalidate();

                                            using (var stream = writeableBitmap[frameIndex].PixelBuffer.AsStream())
                                            {
                                                stream.Write(pixels, 0, pixels.Length);
                                            }

                                            //  Finally we have a frame (WriteableBitmap) that can internally be stored.
                                            _bitmapFrames.Add(writeableBitmap[frameIndex]);
                                        }

                                        frameIndex++;
                                    }

                                    this.animationLoaded = true;
                                    this.thumbnailMode = false;
                                    await SetupAnimation();
                                }
                            }
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(3)); // TODO

                    lock (countingLock)
                    {
                        if (currentConcurrentAnimationLoads > 0)
                        {
                            currentConcurrentAnimationLoads--;
                            isLessthanMaxConcurrent = false;
                        }
                    }

                }
                #region end
            }
            catch (Exception e)
            {
                bSucceeded = false;
                exception = e;
            }
            finally
            {
                Logging.Logger.Info(string.Format("{0}::{1} - Complete", this.GetType().Name, methodName));
            }

            if (!bSucceeded)
            {
                Logging.Logger.Critical(string.Format("{0}::{1} - Failed", this.GetType().Name, exception.ToString()));
                await Task.Delay(TimeSpan.FromSeconds(5));
                throw exception;
            }
            #endregion
        }

        private bool thumbnailMode;

        private async Task SetupAnimation()
        {
            #region start
            string methodName = "SetupAnimation";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));

            try
            {
            #endregion
                await BuildStoryBoard();

                if (PlayOnLoad)
                {
                    storyboard.Begin();

                    if (ImageLoaded != null)
                    {
                        ImageLoaded(this, null);
                    }
                }
            #region end
            }
            catch (Exception e)
            {
                bSucceeded = false;
                exception = e;
            }
            finally
            {
                Logging.Logger.Info(string.Format("{0}::{1} - Complete", this.GetType().Name, methodName));
            }

            if (!bSucceeded)
            {
                Logging.Logger.Critical(string.Format("{0}::{1} - Failed", this.GetType().Name, exception.ToString()));
                await Task.Delay(TimeSpan.FromSeconds(5));
                throw exception;
            }
            #endregion
        }

        private bool imageBeingLoaded = false;

        private async void LoadNewAnimation()
        {
            #region start
            string methodName = "LoadNewAnimation";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));

            try
            {
            #endregion
                
                while (this.imageBeingLoaded)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(10));
                }

                // Future optimization is to load more and then detect which one are in screen and load their animations.
                // Also maybe load thumbnails in batches 

                lock (this.imageChangedLock)
                {
                    imageBeingLoaded = true;
                }

                if (VideoFile == null)
                {
                    return;
                }

                string calculatedHash = ComputeMD5(VideoFile.Path);
                if (calculatedHash != this.hash)
                {
                    this.hash = calculatedHash;
                }
                else
                {
                    return;
                }

                this.image.Source = await this.VideoFile.GetThumbnail();

                this.thumbnailMode = true;

                    
                lock (this.imageChangedLock)
                {
                    imageBeingLoaded = false;
                }
            #region end
            }
            catch (Exception e)
            {
                bSucceeded = false;
                exception = e;
            }
            finally
            {
                Logging.Logger.Info(string.Format("{0}::{1} - Complete", this.GetType().Name, methodName));
            }

            if (!bSucceeded)
            {
                Logging.Logger.Critical(string.Format("{0}::{1} - Failed", this.GetType().Name, exception.ToString()));
                await Task.Delay(TimeSpan.FromSeconds(5));
                throw exception;
            }
            #endregion
        }

        public static string ComputeMD5(string str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm("MD5");
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }

        private async Task BuildStoryBoard()
        {
            #region start
            string methodName = "BuildStoryBoard";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));

            try
            {
            #endregion
                ts = TimeSpan.Zero;

                //  Now create the animation as a set of ObjectAnimationUsingKeyFrames (I love this name!)
                if (this.thumbnailMode)
                {
                    anim.BeginTime = TimeSpan.FromSeconds(0);
                }
                else
                {
                    anim2.BeginTime = TimeSpan.FromSeconds(0);
                }

                var speed = TimeSpan.FromMilliseconds(ApplicationConfiguration.Configuration.Category1Delay);

                //  Clear the story board, if it has previously been filled
                if (storyboard.Children.Count > 0)
                {
                    storyboard.Stop();
                    storyboard.Children.Clear();
                }

                // Create each DiscreteObjectKeyFrame and advance the KeyTime by 100 ms (=10 fps) and add it to the storyboard.
                if (this.thumbnailMode)
                {
                    anim.KeyFrames.Clear();
                }
                else
                {
                    anim2.KeyFrames.Clear();
                }

                if (this.thumbnailMode)
                {
                    singleKeyFrame.KeyTime = KeyTime.FromTimeSpan(ts);
                    singleKeyFrame.Value = _bitmapFrame; ;
                    ts = ts.Add(speed);
                    anim.KeyFrames.Add(singleKeyFrame);
                }
                else
                {
                    for (int frameIndex = 0; frameIndex < _bitmapFrames.Count; frameIndex++)
                    {

                        keyFrame[frameIndex].KeyTime = KeyTime.FromTimeSpan(ts);
                        keyFrame[frameIndex].Value = _bitmapFrames[frameIndex];
                        ts = ts.Add(speed);
                        anim2.KeyFrames.Add(keyFrame[frameIndex]);
                    }
                }

                //  Connect the image control with the story board
                if (this.thumbnailMode)
                {
                    Storyboard.SetTarget(anim, image);
                    Storyboard.SetTargetProperty(anim, "Source");
                }
                else
                {
                    Storyboard.SetTarget(anim2, image);
                    Storyboard.SetTargetProperty(anim2, "Source");
                }

                if (this.thumbnailMode)
                {

                    if (storyboard.Children.Count == 0)
                    {
                        storyboard.Children.Add(anim);
                    }
                    else
                    {
                        storyboard.Children[0] = anim;
                    }
                }
                else
                {
                    if (storyboard.Children.Count == 0)
                    {
                        storyboard.Children.Add(anim2);
                    }
                    else
                    {
                        storyboard.Children[0] = anim2;
                    }

                }
            #region end
            }
            catch (Exception e)
            {
                bSucceeded = false;
                exception = e;
            }
            finally
            {
                Logging.Logger.Info(string.Format("{0}::{1} - Complete", this.GetType().Name, methodName));
            }

            if (!bSucceeded)
            {
                Logging.Logger.Critical(string.Format("{0}::{1} - Failed", this.GetType().Name, exception.ToString()));
                await Task.Delay(TimeSpan.FromSeconds(5));
                throw exception;
            }
            #endregion
        }

        private static void VideoFileChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            #region start
            string methodName = "VideoFileChanged";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", "AnimationImage", methodName));

            try
            {
            #endregion
            if ((((Windows.UI.Xaml.Controls.UserControl)(sender)).Content).Visibility == Visibility.Visible)
            {
                //http://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh780657.aspx
                var control = (AnimationImage)sender;
                control.animationLoaded = false;
                control.LoadNewAnimation();
                control.oldScreenCoords.X = 0;
                control.oldScreenCoords.Y = 0;

                if (control.storyboard.Children.Count > 0)
                {
                    control.storyboard.Stop();
                    control.storyboard.Children.Clear();
                }

                // Create each DiscreteObjectKeyFrame and advance the KeyTime by 100 ms (=10 fps) and add it to the storyboard.
                if (control.thumbnailMode)
                {
                    control.anim.KeyFrames.Clear();
                }
                else
                {
                    control.anim2.KeyFrames.Clear();
                }
            }
            else
            {

            }
                #region end
            }
            catch (Exception e)
            {
                bSucceeded = false;
                exception = e;
            }
            finally
            {
                Logging.Logger.Info(string.Format("{0}::{1} - Complete", "AnimationImage", methodName));
            }

            if (!bSucceeded)
            {
                Logging.Logger.Critical(string.Format("{0}::{1} - Failed", "AnimationImage", exception.ToString()));
                throw exception;
            }
                #endregion
        }
        #endregion

        #region Public Events
        /// <summary>
        /// Fired whenever the image has loaded
        /// </summary>
        public EventHandler ImageLoaded;
        #endregion
    }
}