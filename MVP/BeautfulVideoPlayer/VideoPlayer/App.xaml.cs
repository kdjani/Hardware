using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace IncrementalLoadingSample
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Windows.Storage;
    using Windows.UI.Popups;
    using Windows.UI.Core;
    using App4.Abstractions;
    using App4.Services;
    using TileAFile.Services;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            //UnhandledException += Application_UnhandledException;
        }

        void SetService<I, T>() where T : new()
        {
            ((ServiceProvider)ServiceProvider.Provider).SetService(typeof(I),
              new T());
        }
        void InitialiseServices()
        {
            this.SetService<IFolderSelectionService, FolderSelectionService>();
            this.SetService<IPersistFolderAccess, PersistFolderAccessService>();
            this.SetService<IQueryFileSystem, QueryFileSystemService>();
        }

        private void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            var task = HandleException(e.Exception);
            task.Wait();

            throw e.Exception;
        }

        /// <summary>
        /// Handles failure for application exception on UI thread (or initiated from UI thread via async void handler)
        /// </summary>
        private static async Task HandleException(Exception ex)
        {
            LogException(ex);

            //Execute.OnUIThread(async () =>
            //{
                var dialog = new MessageDialog(GetDisplayMessage(ex), "Unknown Error");
                //var dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
                //await dispatcher.RunAsync(CoreDispatcherPriority.Normal, dialog.ShowAsync());
                await dialog.ShowAsync();
                if (!string.IsNullOrEmpty(ex.ToString()))
                {
                    StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Unhandled" + Guid.NewGuid().ToString().Replace("-", "") + ".log", CreationCollisionOption.OpenIfExists);
                    await FileIO.AppendTextAsync(file, ex.ToString());
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
           // });
        }

        private static void LogException(Exception ex)
        {
            Logging.Logger.Critical(string.Format("LogException Unhandled::{0}", ex.ToString()));
            // e.g. MarkedUp.AnalyticClient.Error(ex.Message, ex);
        }

        /// <summary>
        /// Gets the error message to display from an exception
        /// </summary>
        private static string GetDisplayMessage(Exception ex)
        {
            string errorMessage;
            errorMessage = (ex.Message + " " + ex.StackTrace);

            return errorMessage;
        }

      

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            this.InitialiseServices();

            // Do not repeat app initialization when already running, just ensure that
            // the window is active
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
            {
                Window.Current.Activate();
                return;
            }

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            // Create a Frame to act navigation context and navigate to the first page
            var rootFrame = new Frame();
            if (!rootFrame.Navigate(typeof(MainPage)))
            {
                throw new Exception("Failed to create initial page");
            }



            // Place the frame in the current Window and ensure that it is active
            Window.Current.Content = rootFrame;
            Window.Current.Activate();


        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
