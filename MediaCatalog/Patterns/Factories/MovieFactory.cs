using MediaCatalog.Interfaces;
using MediaCatalog.Models;

namespace MediaCatalog.Patterns.Factories
{
    public class MovieFactory : IMediaFactory
    {
        public MediaItem CreateMediaItem()
        {
            var movie = new Movie
            {
                Status = new Patterns.States.PlannedState(),
                StatusType = "PlannedState",
                Title = "Новый фильм",
                Year = DateTime.Now.Year,
                Genre = "Не указан",
                Rating = 0,
                Director = "",
                Duration = TimeSpan.Zero,
                Studio = ""
            };
            return movie;
        }

        public string GetMediaType() => "Фильм";
    }
}