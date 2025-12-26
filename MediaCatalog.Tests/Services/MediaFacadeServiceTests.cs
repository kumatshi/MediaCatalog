using System;
using System.Collections.ObjectModel;
using MediaCatalog.Models;
using MediaCatalog.Patterns.Services;
using Xunit;

namespace MediaCatalog.Tests
{
    public class MediaFacadeServiceTests
    {
        [Fact]
        public void CreateMedia_BookType_ReturnsBook()
        {
            var collection = new ObservableCollection<MediaItem>();
            var service = new MediaFacadeService(collection);

            var result = service.CreateMedia("Книга");

            Assert.NotNull(result);
            Assert.IsType<Book>(result);
        }

        [Fact]
        public void GetAvailableMediaTypes_ReturnsAllTypes()
        {
            var collection = new ObservableCollection<MediaItem>();
            var service = new MediaFacadeService(collection);

            var types = service.GetAvailableMediaTypes();

            Assert.Contains("Книга", types);
            Assert.Contains("Фильм", types);
            Assert.Contains("Игра", types);
            Assert.Contains("Музыка", types);
        }
    }
}