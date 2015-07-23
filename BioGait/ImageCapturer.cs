using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BioGait
{
    public static class ImageCapturer
    {
        public static void SaveToBmp(FrameworkElement visual, string fileName)
        {
            var bitmapEncoder = new BmpBitmapEncoder();
            SaveUsingEncoder(visual, fileName, bitmapEncoder);
        }

        public static void SaveToPng(FrameworkElement visual, string fileName)
        {
            var pngBitmapEncoder = new PngBitmapEncoder();
            SaveUsingEncoder(visual, fileName, pngBitmapEncoder);
        }

        private static void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            var renderTargetBitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 100, 100, PixelFormats.Pbgra32);
            System.Console.WriteLine((int)visual.ActualWidth + "\n" + (int)visual.ActualHeight);
            renderTargetBitmap.Render(visual);
            var bitmapFrame = BitmapFrame.Create(renderTargetBitmap);
            encoder.Frames.Add(bitmapFrame);
            using (var fileStream = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                encoder.Save(fileStream);
            }
        }
    }
}
