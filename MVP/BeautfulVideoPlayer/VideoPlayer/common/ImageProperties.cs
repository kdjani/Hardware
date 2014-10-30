namespace App4.Common
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using Windows.Graphics.Imaging;
  using Windows.Storage.BulkAccess;
  using Windows.Storage.Streams;
  using Windows.UI.Xaml;
  using Windows.UI.Xaml.Controls;
  using Windows.UI.Xaml.Media.Imaging;

  class ImageProperties : DependencyObject
  {
    static Dictionary<Image, CancellationTokenSource> _imageLoads;

    public static DependencyProperty FileSourceProperty =
      DependencyProperty.RegisterAttached("FileInformationSource",
        typeof(FileInformation), typeof(ImageProperties),
        new PropertyMetadata(null, OnFileSourceChanged));

    static ImageProperties()
    {
      _imageLoads = new Dictionary<Image, CancellationTokenSource>();
    }
    public static void SetFileInformationSource(Image image, FileInformation value)
    {
      image.SetValue(FileSourceProperty, value);
    }
    public static FileInformation GetFileInformationSource(Image image)
    {
      return ((FileInformation)image.GetValue(FileSourceProperty));
    }
    static async void OnFileSourceChanged(DependencyObject sender,
      DependencyPropertyChangedEventArgs args)
    {
      Image image = (Image)sender;
      FileInformation newValue = (FileInformation)args.NewValue;
      image.Source = null;

      if (_imageLoads.ContainsKey(image))
      {
        // We're already loading this thing.
        CancellationTokenSource source = _imageLoads[image];

        // TODO: this throws on me?
        source.Cancel();
      }

      if (newValue != null)
      {
        CancellationTokenSource source = new CancellationTokenSource();
        _imageLoads[image] = source;

        try
        {
          await ResizeAsync(image, newValue, source);
        }
        catch (TaskCanceledException)
        {
        }
      }
    }
    static async Task ResizeAsync(Image image, FileInformation fileInformation,
      CancellationTokenSource source)
    {
      using (var stream = await fileInformation.OpenReadAsync().AsTask(source.Token))
      {
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream).AsTask(source.Token);
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
          outStream, decoder).AsTask(source.Token);

        encoder.BitmapTransform.ScaledHeight = (uint)height;
        encoder.BitmapTransform.ScaledWidth = (uint)width;
        await encoder.FlushAsync().AsTask(source.Token);
        outStream.Seek(0);

        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.SetSource(outStream);
        image.Source = bitmapImage;

        _imageLoads.Remove(image);
      }
    }
  }
}