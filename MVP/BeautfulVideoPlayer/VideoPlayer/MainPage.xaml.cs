using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using VideoFolders;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IncrementalLoadingSample
{
    using IncrementalLoadingSample.Data;
    using Windows.Storage;
    using Windows.Storage.Streams;
    using System.Threading.Tasks;
    using Windows.Storage.Pickers;
    using App4.Abstractions;
    using Windows.UI.Core;
    using Windows.System;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        ///     The view model used by this window
        /// </summary>
        private MainWindowViewModel viewModel;

        public MainPage()
        {
            #region start
            string methodName = "MainPage";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));
            
            try
            {
            #endregion
                Logging.Logger.SetupLogging("Monkey");
                this.InitializeComponent();
                this.InitializeLocally();

                this.viewModel = new MainWindowViewModel();
                library = MainWindowViewModel.library;
                this.DataContext = this.viewModel;
                contentList.ItemsSource = this.viewModel.Gifs;
                hack();

                btnKeboardCollector.Loaded += MyBoard_Loaded;
                btnKeboardCollector.LostFocus += btnKeboardCollector_LostFocus;
                btnKeboardCollector.KeyUp += KeyUpHandler;
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

        void MyBoard_Loaded(object sender, RoutedEventArgs e)
        {
            // I do other initialization here

            btnKeboardCollector.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        void btnKeboardCollector_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Scenario1MediaElement.IsFullWindow)
            {
                btnKeboardCollector.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            }
        }

        private bool IsShiftKeyPressed()
        {
            var state = CoreWindow.GetForCurrentThread().GetKeyState(VirtualKey.Shift);
            return (state & CoreVirtualKeyStates.Down) == CoreVirtualKeyStates.Down;
        }

        private void KeyUpHandler(object sender, KeyRoutedEventArgs e)
        {
            if (Scenario1MediaElement.IsFullWindow)
            {
                if (e.Key == Windows.System.VirtualKey.Escape)
                {
                    if (Scenario1MediaElement.IsFullWindow)
                    {
                        Scenario1MediaElement.IsFullWindow = false;
                    }
                }

                if (e.Key == Windows.System.VirtualKey.P)
                {
                    if (this.Scenario1MediaElement.CurrentState == MediaElementState.Playing)
                    {
                        this.Scenario1MediaElement.Pause();
                    }

                    if (this.Scenario1MediaElement.CurrentState == MediaElementState.Paused)
                    {
                        this.Scenario1MediaElement.Play();
                    }
                }

                if (e.Key == Windows.System.VirtualKey.R)
                {
                    if (this.Scenario1MediaElement.CurrentState == MediaElementState.Playing)
                    {
                        this.Scenario1MediaElement.Position = new TimeSpan(0);
                        //this.Scenario1MediaElement.Play();
                    }
                }


                if (e.Key == Windows.System.VirtualKey.Right && IsShiftKeyPressed())
                {
                    if (this.Scenario1MediaElement.CurrentState == MediaElementState.Playing)
                    {
                        this.Scenario1MediaElement.Position += new TimeSpan(0, 0, 5);
                        //this.Scenario1MediaElement.Play();
                    }
                }

                if (e.Key == Windows.System.VirtualKey.Left && IsShiftKeyPressed())
                {
                    if (this.Scenario1MediaElement.CurrentState == MediaElementState.Playing)
                    {
                        this.Scenario1MediaElement.Position -= new TimeSpan(0, 0, 5);
                        //this.Scenario1MediaElement.Play();
                    }
                }

                if (e.Key == Windows.System.VirtualKey.Left && !IsShiftKeyPressed())
                {
                    if (this.Scenario1MediaElement.CurrentState == MediaElementState.Playing)
                    {
                        this.RetrieveAndPlayPreviousFile();
                    }
                }

                if (e.Key == Windows.System.VirtualKey.Right && !IsShiftKeyPressed())
                {
                    if (this.Scenario1MediaElement.CurrentState == MediaElementState.Playing)
                    {
                        this.RetrieveAndPlayNextFile();
                    }
                }

                if (e.Key == Windows.System.VirtualKey.Up && !IsShiftKeyPressed())
                {
                    if (this.Scenario1MediaElement.CurrentState == MediaElementState.Playing)
                    {
                        if (this.Scenario1MediaElement.Volume < 1.0)
                        {
                            this.Scenario1MediaElement.Volume += 0.1;
                        }
                    }
                }

                if (e.Key == Windows.System.VirtualKey.Down && !IsShiftKeyPressed())
                {
                    if (this.Scenario1MediaElement.CurrentState == MediaElementState.Playing)
                    {
                        if (this.Scenario1MediaElement.Volume > 0.0)
                        {
                            this.Scenario1MediaElement.Volume -= 0.1;
                        }
                    }
                }
            }
        }

        private void RetrieveAndPlayNextFile()
        {
            ScanningFile newFile = this.viewModel.fileCollection.GetNextFile(currentFile);
            if (newFile != null)
            {
                PlayFile(newFile);
            }
        }

        private void RetrieveAndPlayPreviousFile()
        {
            ScanningFile newFile = this.viewModel.fileCollection.GetPreviousFile(currentFile);
            if (newFile != null)
            {
                PlayFile(newFile);
            }
        }

        private async void PlayFile(ScanningFile file)
        {
            StorageFile newFile = await file.GetStorageFile();
            if (newFile != null)
            {
                IRandomAccessStream stream = await newFile.OpenAsync(FileAccessMode.Read);
                Scenario1MediaElement.SetSource(stream, newFile.ContentType);
                currentFile = file;
            }
            else
            {

            }
        }

        private void KeyUp(object sender, KeyRoutedEventArgs e)
        {
        }

        private async void hack()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                contentList.ItemsSource = this.viewModel.Gifs;
            }
        }

        public static AnimationLibrary.AnimationLibrary library;

        public static StorageFolder deleteMe;

        private static ScanningFile currentFile;

        private async void InitializeLocally()
        {
            #region start
            string methodName = "InitializeLocally";
            bool bSucceeded = true;
            Exception exception = null;
            Logging.Logger.Info(string.Format("{0}::{1} - Start", this.GetType().Name, methodName));
            
            try
            {
            #endregion
                
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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //DataContext = new MainWindowViewModel();

            IViewModelInitialisation initialiser = this.DataContext as IViewModelInitialisation;

            if (initialiser != null)
            {
                initialiser.Initialise();
            }
        }

        private async void AnimationImage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Logging.Logger.Info("AnimationImage_Tapped");
            try
            {
                TriGemini.Controls.AnimationImage animation = (TriGemini.Controls.AnimationImage)sender;
                PlayFile(animation.VideoFile);
            }
            catch(Exception ex)
            {

            }
        }

        private void Scenario1MediaElement_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Scenario1MediaElement.IsFullWindow = !Scenario1MediaElement.IsFullWindow;
            btnKeboardCollector.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        private void Scenario1MediaElement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.Scenario1MediaElement.CurrentState == MediaElementState.Playing)
            {
                this.Scenario1MediaElement.Pause();
            }

            if (this.Scenario1MediaElement.CurrentState == MediaElementState.Paused)
            {
                this.Scenario1MediaElement.Play();
            }
        }

        private void Scenario1MediaElement_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Escape)
            {
                if(Scenario1MediaElement.IsFullWindow)
                {
                    Scenario1MediaElement.IsFullWindow = false;
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.viewModel.fileCollection.Sort();
        }

        private void Scenario1MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.RetrieveAndPlayNextFile();
        }
    }
}
