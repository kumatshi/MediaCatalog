using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.Json;

namespace MediaCatalog.Services
{
    public class OmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "dfd21a6a"; 
        private readonly string _coversDirectory;

        public OmdbService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MediaCatalog/1.0");
            _httpClient.Timeout = TimeSpan.FromSeconds(10);

            _coversDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MediaCatalog",
                "MovieCovers");

            Directory.CreateDirectory(_coversDirectory);
        }
        /// <summary>
        /// поиск фильма с несколькими попытками
        /// </summary>
        public async Task<MovieSearchResult> SearchMovieAsync(string title, int? year = null)
        {
            if (string.IsNullOrWhiteSpace(title) || title == "Новый фильм")
                return null;

            var searchAttempts = new[]
            {
                new { Title = title.Trim(), Year = year },
                new { Title = RemoveYearFromTitle(title), Year = year },
                new { Title = title.Trim(), Year = (int?)null }, // Без года
                
            };

            foreach (var attempt in searchAttempts)
            {
                if (string.IsNullOrWhiteSpace(attempt.Title))
                    continue;

                var result = await TrySearchAsync(attempt.Title, attempt.Year);
                if (result != null)
                    return result;
            }

            return null;
        }

        private async Task<MovieSearchResult> TrySearchAsync(string title, int? year)
        {
            try
            {
                var searchTitle = Uri.EscapeDataString(title);
                var url = $"http://www.omdbapi.com/?apikey={_apiKey}&t={searchTitle}";

                if (year.HasValue)
                    url += $"&y={year}";

                var response = await _httpClient.GetStringAsync(url);

                using var doc = JsonDocument.Parse(response);
                var root = doc.RootElement;

                if (root.GetProperty("Response").GetString() == "True")
                {
                    return new MovieSearchResult
                    {
                        Title = root.GetProperty("Title").GetString() ?? title,
                        Year = root.GetProperty("Year").GetString() ?? year?.ToString() ?? "",
                        PosterUrl = root.GetProperty("Poster").GetString(),
                        Plot = root.GetProperty("Plot").GetString(),
                        Director = root.GetProperty("Director").GetString(),
                        Actors = root.GetProperty("Actors").GetString(),
                        Runtime = root.GetProperty("Runtime").GetString(),
                        Genre = root.GetProperty("Genre").GetString(),
                        ImdbRating = root.GetProperty("imdbRating").GetString(),
                        ImdbID = root.GetProperty("imdbID").GetString()
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка поиска фильма '{title}': {ex.Message}");
            }

            return null;
        }

        private string RemoveYearFromTitle(string title)
        {
            var result = System.Text.RegularExpressions.Regex.Replace(title, @"\s*\(\d{4}\)", "").Trim();
            return result.Length > 0 ? result : title;
        }



        /// <summary>
        /// Скачивание и сохранение постера
        /// </summary>
        public async Task<string> DownloadAndSavePosterAsync(string posterUrl, string movieTitle, string imdbId = null)
        {
            if (string.IsNullOrEmpty(posterUrl) || posterUrl.Contains("nopicture"))
                return null;

            try
            {
                var fileName = !string.IsNullOrEmpty(imdbId)
                    ? $"movie_{imdbId}"
                    : $"movie_{movieTitle.GetHashCode():X}";

                var extension = Path.GetExtension(posterUrl.Split('?')[0]) ?? ".jpg";
                var localPath = Path.Combine(_coversDirectory, $"{fileName}{extension}");

                if (File.Exists(localPath))
                    return localPath;

                var imageBytes = await _httpClient.GetByteArrayAsync(posterUrl);
                await File.WriteAllBytesAsync(localPath, imageBytes);

                return localPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки постера: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Полная информация о фильме с загрузкой постера
        /// </summary>
        public async Task<MovieInfo> GetMovieInfoAsync(string title, int? year = null)
        {
            var searchResult = await SearchMovieAsync(title, year);

            if (searchResult == null)
                return null;

            string posterPath = null;
            if (!string.IsNullOrEmpty(searchResult.PosterUrl))
            {
                posterPath = await DownloadAndSavePosterAsync(
                    searchResult.PosterUrl,
                    searchResult.Title,
                    searchResult.ImdbID);
            }

            return new MovieInfo
            {
                Title = searchResult.Title,
                Year = ParseYear(searchResult.Year),
                PosterPath = posterPath,
                Plot = searchResult.Plot,
                Director = searchResult.Director,
                Actors = searchResult.Actors,
                Runtime = ParseRuntime(searchResult.Runtime),
                Genre = searchResult.Genre,
                ImdbRating = searchResult.ImdbRating,
                ImdbID = searchResult.ImdbID
            };
        }

        private int ParseYear(string yearString)
        {
            if (string.IsNullOrEmpty(yearString))
                return DateTime.Now.Year;

            var cleanYear = new string(yearString.Where(char.IsDigit).ToArray());
            if (int.TryParse(cleanYear, out int year) && year >= 1800 && year <= DateTime.Now.Year + 5)
                return year;

            return DateTime.Now.Year;
        }

        private TimeSpan ParseRuntime(string runtimeString)
        {
            if (string.IsNullOrEmpty(runtimeString))
                return TimeSpan.Zero;

            var parts = runtimeString.Split(' ');
            if (parts.Length >= 1 && int.TryParse(parts[0], out int minutes))
                return TimeSpan.FromMinutes(minutes);

            return TimeSpan.Zero;
        }

        /// <summary>
        /// Загружает изображение для отображения 
        /// </summary>
        public BitmapImage LoadPosterImage(string localPath)
        {
            if (string.IsNullOrEmpty(localPath) || !File.Exists(localPath))
                return CreateDefaultPoster();

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(localPath);
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return CreateDefaultPoster();
            }
        }

        private BitmapImage CreateDefaultPoster()
        {
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(
                    Brushes.DarkRed,
                    null,
                    new Rect(0, 0, 200, 300));

                drawingContext.DrawRectangle(
                    Brushes.White,
                    new Pen(Brushes.White, 2),
                    new Rect(80, 100, 40, 60));

                drawingContext.DrawRectangle(
                    Brushes.White,
                    new Pen(Brushes.White, 2),
                    new Rect(140, 100, 40, 60));
            }

            var renderTarget = new RenderTargetBitmap(200, 300, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(drawingVisual);

            var bitmapImage = new BitmapImage();
            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            using (var stream = new MemoryStream())
            {
                bitmapEncoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            bitmapImage.Freeze();
            return bitmapImage;
        }
    }


    public class MovieSearchResult
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string PosterUrl { get; set; }
        public string Plot { get; set; }
        public string Director { get; set; }
        public string Actors { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
        public string ImdbRating { get; set; }
        public string ImdbID { get; set; }
    }

    public class MovieInfo
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public string PosterPath { get; set; }
        public string Plot { get; set; }
        public string Director { get; set; }
        public string Actors { get; set; }
        public TimeSpan Runtime { get; set; }
        public string Genre { get; set; }
        public string ImdbRating { get; set; }
        public string ImdbID { get; set; }
    }
}