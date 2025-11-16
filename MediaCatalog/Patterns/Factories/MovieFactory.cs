using MediaCatalog.Interfaces;
using MediaCatalog.Models;

namespace MediaCatalog.Patterns.Factories
{
    public class MovieFactory : IMediaFactory
    {
        public MediaItem CreateMediaItem()
        {
            return new Movie
            {
                Status = new Patterns.States.PlannedState()
            };
        }

        public string GetMediaType() => "Фильм";
    }
}