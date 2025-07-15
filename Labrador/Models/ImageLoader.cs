using Android.Content.Res;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Labrador.Models
{
    public class ImageLoader
    {

        /* 
         * This class is used for loading and converting images from the Personal directory of the
         * device. Most of the functionality contained will be offloaded onto the companion app.
         */


        string dpiPath;

        public ImageLoader()
        {
            dpiPath = GetPath();
        }

        public List<string> GetImageFiles()
        {
            /* Method: GetImageFile()
             * 
             * This method gets a list of filenames from the Personal/pictogram directory and returns it.
             * 
             * This will be deprecated in the final product, as editing and creating words and images will be offloaded
             * onto the companion app.
             */

            List<string> fileNames = new List<string>();
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "pictograms");
            foreach(var f in Directory.GetFiles(path))
            {
                string p = string.Concat(path, "/");
                string tmp = f.Replace(p, "");
                fileNames.Add(tmp);
            }
            return fileNames;
        }

        public string GetPath()
        {
            /* Method: GetPath()
             * 
             * This method emulates Xamarin.Forms' auto scaling, as to reduce the APK size, not all possible pictogram images
             * will be downloaded with the base app. This app, or the companion app, will store all pictograms in Personal/pictograms
             * but depending on the OS will alter the suffix and DPI settings on the loaded files. At this point, only android
             * has been written, all other OS' will throw a NotImplementedException().
             */

            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "pictograms");
            var dpi = DeviceDisplay.MainDisplayInfo.Density;

            if (Device.RuntimePlatform == Device.iOS) throw new NotImplementedException();
            if (Device.RuntimePlatform == Device.UWP) throw new NotImplementedException();
            if (Device.RuntimePlatform == Device.Android) {
                if (dpi >= 4)
                {
                    path = Path.Combine(path, "xxxhdpi");
                }
                else if (dpi >= 3)
                {
                    path = Path.Combine(path, "xxhdpi");
                }
                else if (dpi >= 2)
                {
                    path = Path.Combine(path, "xhdpi");
                }
                else if (dpi >= 1.5)
                {
                    path = Path.Combine(path, "hdpi");
                }
                else if (dpi < 1)
                    path = Path.Combine(path, "ldpi");
                }
            return path;
        }

        public void WriteImage(Stream imageSource, string fileName)
        {
            /* Method: WriteImage()
             * 
             * This method crops the image sent from the camera into a 1:1 ratio image, then creates the appropriate
             * sized images based on operating system.
             * 
             * This method may be partially offloaded onto the companion app, as users will still require access to the phone's
             * camera.
             */

            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "pictograms");
            SKBitmap origPhoto = SKBitmap.Decode(imageSource) ;

            //Work out short edge of photo
            int top, left, bottom, right;
            if(origPhoto.Width > origPhoto.Height)
            {
                top = 0;
                left = (int)(origPhoto.Width * 0.25);
                bottom = origPhoto.Height;
                right = (int)(origPhoto.Width * 0.75);
            } else
            {
                top = (int)(origPhoto.Height * 0.25);
                left = 0;
                bottom = (int)(origPhoto.Height * 0.75);
                right = origPhoto.Width;
            }

            SKBitmap croppedPhoto = new SKBitmap();
            origPhoto.ExtractSubset(croppedPhoto, SKRectI.Create(left, top, right, bottom));

            if (Device.RuntimePlatform == Device.iOS) throw new NotImplementedException();
            if (Device.RuntimePlatform == Device.UWP) throw new NotImplementedException();
            if (Device.RuntimePlatform == Device.Android)
            {
                using (var streamOut = new FileStream(Path.Combine(path,"xxxhdpi",fileName), FileMode.Create))
                {
                    SKBitmap xxxhdpi = croppedPhoto.Resize(new SKSizeI(192, 192), SKFilterQuality.High);
                    var sKImage = SKImage.FromBitmap(xxxhdpi);
                    var skData = sKImage.Encode();

                    skData.SaveTo(streamOut);
                }
                using (var streamOut = new FileStream(Path.Combine(path, "xxhdpi", fileName), FileMode.Create))
                {
                    SKBitmap xxhdpi = croppedPhoto.Resize(new SKSizeI(144, 144), SKFilterQuality.High);
                    var sKImage = SKImage.FromBitmap(xxhdpi);
                    var skData = sKImage.Encode();

                    skData.SaveTo(streamOut);
                }
                using (var streamOut = new FileStream(Path.Combine(path, "xhdpi", fileName), FileMode.Create))
                {
                    SKBitmap xhdpi = croppedPhoto.Resize(new SKSizeI(96, 96), SKFilterQuality.High);
                    var sKImage = SKImage.FromBitmap(xhdpi);
                    var skData = sKImage.Encode();

                    skData.SaveTo(streamOut);
                }
                using (var streamOut = new FileStream(Path.Combine(path, "hdpi", fileName), FileMode.Create))
                {
                    SKBitmap hdpi = croppedPhoto.Resize(new SKSizeI(72, 72), SKFilterQuality.High);
                    var sKImage = SKImage.FromBitmap(hdpi);
                    var skData = sKImage.Encode();

                    skData.SaveTo(streamOut);
                }
                using (var streamOut = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    SKBitmap mdpi = croppedPhoto.Resize(new SKSizeI(48, 48), SKFilterQuality.High);
                    var sKImage = SKImage.FromBitmap(mdpi);
                    var skData = sKImage.Encode();

                    skData.SaveTo(streamOut);
                }
                using (var streamOut = new FileStream(Path.Combine(path, "ldpi", fileName), FileMode.Create))
                {
                    SKBitmap ldpi = croppedPhoto.Resize(new SKSizeI(36, 36), SKFilterQuality.High);
                    var sKImage = SKImage.FromBitmap(ldpi);
                    var skData = sKImage.Encode();

                    skData.SaveTo(streamOut);
                }
            }
        }

        public bool ImageFound(string filename)
        {
            if (File.Exists(Path.Combine(dpiPath, filename))) return true;
            return false;
        }

        protected SKImage RecolourImage(SKBitmap bitmap)
        {
            /* Method: RecolourImage()
             * 
             * This method checks the provided bitmap for Chromakey green, pixel-by-pixel, and replaces those pixels with the
             * current configured skin tone.
             * 
             * This method will be offloaded entirely to the companion app in future releases.
             */


            Color chromakey = Xamarin.Forms.Color.FromHex("00b140"); // Chromakey Green is #00B140... alpha is implied full opacity
            Color skinTone = (Application.Current as App).configuration.GetSkinTone();

            byte red = (byte)(skinTone.R * 255);
            byte green = (byte)(skinTone.G * 255);
            byte blue = (byte)(skinTone.B * 255);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color currentPixel = Color.FromRgb(bitmap.GetPixel(x, y).Red, bitmap.GetPixel(x, y).Green, bitmap.GetPixel(x, y).Blue);
                    if (currentPixel == chromakey)
                    {
                        // Replace Pixels
                        bitmap.SetPixel
                            (x, y, new SKColor(
                                    (byte)(skinTone.R * 255),
                                    (byte)(skinTone.G * 255),
                                    (byte)(skinTone.B * 255),
                                    bitmap.GetPixel(x, y).Alpha
                                )
                            );
                    }
                }
            }

            return SKImage.FromBitmap(bitmap);
        }

        public Stream GetImage(string fileName)
        {
            /* Method: HasChildren()
             * 
             * This method checks for the existence of the requested filename, if it exists, it runs it through
             * the recolour image method and returns an image stream. If it fails, it returns an empty image as a
             * stream.
             */

            string fullPath = Path.Combine(dpiPath,fileName);

            if (File.Exists(fullPath)) { 
                using (Stream inStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    SKBitmap skBitmap = SKBitmap.Decode(inStream);
                    SKImage skImage = RecolourImage(skBitmap);
                    return skImage.Encode().AsStream();
                }
            }
            else
            {
                return SKImage.FromBitmap(new SKBitmap()).Encode().AsStream();
            }
        }

        public Stream GetResourceImage(string fileName)
        {
            /* This does not work at the moment. Recolouring of asset images will have to wait.
             */
            Stream assetFile;
            string path;
            var dpi = DeviceDisplay.MainDisplayInfo.Density;

            if (Device.RuntimePlatform == Device.iOS) throw new NotImplementedException();
            if (Device.RuntimePlatform == Device.UWP) throw new NotImplementedException();
            if (Device.RuntimePlatform == Device.Android)
            {
                if (dpi >= 4)
                {
                    path = Path.Combine("drawable-xxxhdpi", fileName);
                }
                else if (dpi >= 3)
                {
                    path = Path.Combine("drawable-xxhdpi", fileName);
                }
                else if (dpi >= 2)
                {
                    path = Path.Combine("drawable-xhdpi", fileName); ;
                }
                else if (dpi >= 1.5)
                {
                    path = Path.Combine("drawable-hdpi", fileName); ;
                }
                else if (dpi >= 1)
                {
                    path = Path.Combine("drawable", fileName); ;
                }
                else if (dpi < 1) Path.Combine("drawable-ldpi", fileName);

                string assetPath = Path.Combine(dpiPath, fileName);

                using (StreamReader sr = new StreamReader(Android.App.Application.Context.Assets.Open(assetPath)))
                {
                    assetFile = sr.BaseStream;
                    SKBitmap skBitmap = SKBitmap.Decode(assetFile);
                    SKImage skImage = RecolourImage(skBitmap);
                    return skImage.Encode().AsStream();
                }
            }

            //No image found
            return SKImage.FromBitmap(new SKBitmap()).Encode().AsStream();
        }


        public Xamarin.Forms.Image Image(string imageName)
        {
            return new Image
            {
                Source = ImageSource.FromStream(() => GetImage(imageName))
            } ;
        }
        public Xamarin.Forms.Image ResourceImage(string imageName)
        {
            Xamarin.Forms.Image image = new Xamarin.Forms.Image() { Source = imageName };
            return image;
        }
    }
}
