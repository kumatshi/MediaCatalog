using MediaCatalog.Interfaces;
using MediaCatalog.Models;

namespace MediaCatalog.Patterns.Factories
{
    public class BookFactory : IMediaFactory
    {
        public MediaItem CreateMediaItem()
        {
            return new Book
            {
                Status = new Patterns.States.PlannedState()
            };
        }

        public string GetMediaType() => "Книга";
    }
}