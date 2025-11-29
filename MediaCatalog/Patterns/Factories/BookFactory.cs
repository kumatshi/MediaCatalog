using MediaCatalog.Interfaces;
using MediaCatalog.Models;

namespace MediaCatalog.Patterns.Factories
{
    public class BookFactory : IMediaFactory
    {
        public MediaItem CreateMediaItem()
        {
            var book = new Book
            {
                Status = new Patterns.States.PlannedState(),
                StatusType = "PlannedState",
                Title = "Новая книга",
                Year = DateTime.Now.Year,
                Genre = "Не указан",
                Rating = 0,
                Author = "",
                PageCount = 0,
                ISBN = ""
            };
            return book;
        }

        public string GetMediaType() => "Книга";
    }
}