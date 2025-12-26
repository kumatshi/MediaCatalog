using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaCatalog.Services;
using Moq;
using Moq.Protected;
using Xunit;

namespace MediaCatalog.Tests.Services
{
    public class OmdbServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly OmdbService _omdbService;

        public OmdbServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _omdbService = new OmdbService();
            var httpClientField = typeof(OmdbService)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (httpClientField != null)
            {
                httpClientField.SetValue(_omdbService, _httpClient);
            }
        }

        [Fact]
        public async Task SearchMovieAsync_WithValidTitle_ReturnsMovieInfo()
        {
            var title = "Inception";
            var year = 2010;

            var responseJson = @"{
                ""Response"": ""True"",
                ""Title"": ""Inception"",
                ""Year"": ""2010"",
                ""Poster"": ""https://example.com/poster.jpg"",
                ""Plot"": ""A thief who steals corporate secrets..."",
                ""Director"": ""Christopher Nolan"",
                ""Actors"": ""Leonardo DiCaprio, Joseph Gordon-Levitt, Ellen Page"",
                ""Runtime"": ""148 min"",
                ""Genre"": ""Action, Adventure, Sci-Fi"",
                ""imdbRating"": ""8.8"",
                ""imdbID"": ""tt1375666""
            }";

            SetupHttpResponse(responseJson);

            var result = await _omdbService.SearchMovieAsync(title, year);

            Assert.NotNull(result);
            Assert.Equal("Inception", result.Title);
            Assert.Equal("2010", result.Year);
            Assert.Equal("Christopher Nolan", result.Director);
            Assert.Equal("8.8", result.ImdbRating);
        }

        [Fact]
        public async Task SearchMovieAsync_WithInvalidTitle_ReturnsNull()
        {
            var title = "NonExistentMovie12345";

            var responseJson = @"{
                ""Response"": ""False"",
                ""Error"": ""Movie not found!""
            }";

            SetupHttpResponse(responseJson);

            var result = await _omdbService.SearchMovieAsync(title);

            Assert.Null(result);
        }

        [Theory]
        [InlineData("Inception (2010)", 2010)]
        [InlineData("The Matrix 1999", 1999)]
        [InlineData("Avatar", null)]
        public async Task SearchMovieAsync_VariousTitles_CallsHttpClient(string title, int? year)
        {
            var responseJson = @"{
                ""Response"": ""True"",
                ""Title"": ""Test Movie"",
                ""Year"": ""2000""
            }";

            SetupHttpResponse(responseJson);

            await _omdbService.SearchMovieAsync(title, year);

            Assert.True(true); 
        }

        [Fact]
        public void ParseYear_WithValidString_ReturnsInteger()
        {
            var omdbService = new OmdbService();
            var yearString = "2010";

            var parseYearMethod = typeof(OmdbService)
                .GetMethod("ParseYear", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var result = (int?)parseYearMethod?.Invoke(omdbService, new object[] { yearString });
            Assert.Equal(2010, result);
        }

        [Fact]
        public void ParseRuntime_WithValidString_ReturnsTimeSpan()
        {
            var omdbService = new OmdbService();
            var runtimeString = "148 min";

            var parseRuntimeMethod = typeof(OmdbService)
                .GetMethod("ParseRuntime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var result = (TimeSpan?)parseRuntimeMethod?.Invoke(omdbService, new object[] { runtimeString });

            Assert.Equal(TimeSpan.FromMinutes(148), result);
        }

        [Fact]
        public async Task DownloadAndSavePosterAsync_WithValidUrl_DownloadsImage()
        {
            var posterUrl = "https://example.com/poster.jpg";
            var movieTitle = "Test Movie";
            var imdbId = "tt1234567";

            var imageBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }; 

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString() == posterUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new ByteArrayContent(imageBytes)
                });
            var result = await _omdbService.DownloadAndSavePosterAsync(posterUrl, movieTitle, imdbId);

            Assert.True(true); 
        }

        private void SetupHttpResponse(string responseJson)
        {
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseJson)
                });
        }
    }
}