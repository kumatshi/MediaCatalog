using MediaCatalog.Interfaces;
using MediaCatalog.Models;

namespace MediaCatalog.Patterns.Factories
{
    public class MusicFactory : IMediaFactory
    {
        public MediaItem CreateMediaItem()
        {
            var music = new Music
            {
                Status = new Patterns.States.PlannedState(),
                StatusType = "PlannedState",
                Title = "Новая музыка",
                Year = DateTime.Now.Year,
                Genre = "Не указан",
                Rating = 0,
                Artist = "",
                Album = "",
                Duration = TimeSpan.Zero,
                Format = "mp3",
                FilePath = "",
                FileSize = 0
            };
            return music;
        }

        public string GetMediaType() => "Музыка";
    }
}