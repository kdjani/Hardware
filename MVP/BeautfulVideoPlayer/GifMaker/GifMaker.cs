using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace GifMaker
{
    public sealed class GifMaker
    {
        readonly List<StorageFile> frames = new List<StorageFile>();

        public GifMaker()
        {
            frames = new List<StorageFile>();
        }

        public void AppendNewFrame(StorageFile frame)
        {
            frames.Add(frame);
        }

        public async void ConvertToGif(int delay)
        {
            var file = await KnownFolders.PicturesLibrary.CreateFileAsync("test.gif", CreationCollisionOption.ReplaceExisting);
            var outStream = await file.OpenAsync(FileAccessMode.ReadWrite);
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.GifEncoderId, outStream);


            for (int i = 0; i < frames.Count; i++)
            {
                var frame = frames[i];
                var stream = await frame.OpenReadAsync();
                var decoder = await BitmapDecoder.CreateAsync(stream);
                var pixels = await decoder.GetPixelDataAsync();

                encoder.SetPixelData(decoder.BitmapPixelFormat, BitmapAlphaMode.Ignore,
                    decoder.OrientedPixelWidth, decoder.OrientedPixelHeight,
                    decoder.DpiX, decoder.DpiY,
                    pixels.DetachPixelData());

                if (i == 0)
                {
                    var properties = new BitmapPropertySet
                        {
                            {
                                "/grctlext/Delay",
                                new BitmapTypedValue(delay / 10, PropertyType.UInt16)
                            }
                        };

                    await encoder.BitmapProperties.SetPropertiesAsync(properties);
                }

                if (i < frames.Count - 1)
                    await encoder.GoToNextFrameAsync();
            }

            await encoder.FlushAsync();
            outStream.Dispose();
        }
    }
}