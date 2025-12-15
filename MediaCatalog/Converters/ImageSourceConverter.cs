using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;

namespace MediaCatalog
{
    public class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is string path && !string.IsNullOrWhiteSpace(path) && path != "N/A")
                {
                    try
                    {
                        if (Uri.TryCreate(path, UriKind.Absolute, out Uri uri) &&
                            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = uri;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            bitmap.Freeze(); 
                            return bitmap;
                        }
                        else if (File.Exists(path))
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(path, UriKind.Absolute);
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            bitmap.Freeze();
                            return bitmap;
                        }
                    }
                    catch
                    {
                        return GetDefaultImage();
                    }
                }

                return GetDefaultImage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ImageSourceConverter: {ex.Message}");
                return GetDefaultImage();
            }
        }

        private BitmapImage GetDefaultImage()
        {
            try
            {
                string[] possiblePaths =
                {
                    "pack://application:,,,/Resources/default-cover.png",
                    "pack://application:,,,/MediaCatalog;component/Resources/default-cover.png",
                    "/Resources/default-cover.png",
                    "Resources/default-cover.png"
                };

                foreach (var path in possiblePaths)
                {
                    try
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        return bitmap;
                    }
                    catch
                    {
                        continue;
                    }
                }

                return new BitmapImage();
            }
            catch
            {
                return new BitmapImage();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}