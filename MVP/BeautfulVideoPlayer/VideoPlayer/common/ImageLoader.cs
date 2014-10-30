namespace App4.Common
{
  using System;
  using System.ComponentModel;
  using System.Threading;
  using System.Threading.Tasks;
  using Windows.Graphics.Imaging;
  using Windows.Storage.BulkAccess;
  using Windows.Storage.Streams;
  using Windows.UI.Xaml;
  using Windows.UI.Xaml.Media.Imaging;

  public class ImageLoader : DependencyObject, INotifyPropertyChanged
  {
    public static DependencyProperty FileInformationSourceProperty =
      DependencyProperty.Register("FileInformationSource",
       typeof(FileInformation), typeof(ImageLoader),
        new PropertyMetadata(null, OnFileInformationSourceChanged));

    public ImageLoader()
    {
    }
    public bool HasLoaded
    {
      get
      {
        return (this._hasLoaded);
      }
      private set
      {
        this._hasLoaded = value;
        this.FirePropertyChanged("HasLoaded");
      }
    }
    public BitmapImage Image
    {
      get
      {
        return (_image);
      }
      private set
      {
        this._image = value;
        this.FirePropertyChanged("Image");
      }
    }
    void FirePropertyChanged(string property)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(property));
      }
    }
    public FileInformation FileInformationSource
    {
      get
      {
        return ((FileInformation)base.GetValue(FileInformationSourceProperty));
      }
      set
      {
        base.SetValue(FileInformationSourceProperty, value);
      }
    }
    static void OnFileInformationSourceChanged(DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      ImageLoader loader = (ImageLoader)sender;
      loader.Reload();
    }
    void Reload()
    {
      this.Image = null;
      this.HasLoaded = false;

      if (this._tokenSource != null)
      {
        this._tokenSource.Cancel();
        this._tokenSource = null;
      }

      if (this.FileInformationSource == null)
      {
        this.HasLoaded = true;
      }
      else
      {
        this.ResizeAsync();
      }
    }
    async void ResizeAsync()
    {
      this._tokenSource = new CancellationTokenSource();

      try
      {
        using (var stream = await this.FileInformationSource.OpenReadAsync().AsTask(
          this._tokenSource.Token))
        {
          BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream).AsTask(
            this._tokenSource.Token);
          double width = decoder.PixelWidth;
          double height = decoder.PixelHeight;

          if (width > Window.Current.Bounds.Width)
          {
            width = Window.Current.Bounds.Width;
            height = height * (width / decoder.PixelWidth);
          }
          if (height > Window.Current.Bounds.Height)
          {
            width = width * (Window.Current.Bounds.Height / height);
            height = Window.Current.Bounds.Height;
          }
          InMemoryRandomAccessStream outStream = new InMemoryRandomAccessStream();

          BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(
            outStream, decoder).AsTask(this._tokenSource.Token);

          encoder.BitmapTransform.ScaledHeight = (uint)height;
          encoder.BitmapTransform.ScaledWidth = (uint)width;
          await encoder.FlushAsync().AsTask(this._tokenSource.Token);
          outStream.Seek(0);

          BitmapImage bitmapImage = new BitmapImage();
          bitmapImage.SetSource(outStream);
          this.Image = bitmapImage;
          this.HasLoaded = true;
          this._tokenSource = null;
        }
      }
      catch (TaskCanceledException)
      {
      }
    }
    CancellationTokenSource _tokenSource;
    bool _hasLoaded;
    BitmapImage _image;
    public event PropertyChangedEventHandler PropertyChanged;
  }
}