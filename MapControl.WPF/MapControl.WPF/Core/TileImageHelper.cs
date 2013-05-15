using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MapControl.WPF.Core
{
    internal static class TileImageHelper
    {
        internal static BitmapImage DownloadImage(Uri uri, bool isInDesignMode)
        {
            var image = LoadImageFromCache(uri);
            if (image != null)
            {
                return image;
            }

            var imageStream = new MemoryStream();
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            responseStream.CopyTo(imageStream);
                            responseStream.Close();
                        }
                    }
                }
                imageStream.Position = 0;
                if (!isInDesignMode)
                {
                    SaveCacheImage(imageStream, uri);
                    imageStream.Position = 0;
                }
                return ConvertStreamToImage(imageStream);
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            catch (NotSupportedException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            finally
            {
                imageStream.Dispose();
            }
            return null;
        }

        private static BitmapImage LoadImageFromCache(Uri uri)
        {
            var localName = GetTileFileName(uri);

            if (File.Exists(localName))
            {
                try
                {
                    using (var file = File.OpenRead(localName))
                    {
                        return ConvertStreamToImage(file);
                    }
                }
                catch (NotSupportedException e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
                catch (IOException e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }
            return null;
        }

        private static BitmapImage ConvertStreamToImage(Stream stream)
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();

            bitmap.Freeze();
            return bitmap;
        }

        private static string GetTileFileName(Uri uri)
        {
            var cacheFolder = Directory.GetCurrentDirectory();
            return Path.Combine(cacheFolder, uri.LocalPath.TrimStart('/'));
        }

        private static void SaveCacheImage(Stream stream, Uri uri)
        {
            var path = GetTileFileName(uri);
            FileStream file = null;
            try
            {
                var dirName = Path.GetDirectoryName(path);
                if (dirName != null)
                {
                    Directory.CreateDirectory(dirName);
                    file = File.Create(path);

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(stream));
                    encoder.Save(file);
                }
            }
            catch (IOException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            finally
            {
                if (file != null)
                {
                    file.Dispose();
                }
            }
        }
    }
}
