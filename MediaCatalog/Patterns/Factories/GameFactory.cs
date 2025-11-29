using MediaCatalog.Interfaces;
using MediaCatalog.Models;

namespace MediaCatalog.Patterns.Factories
{
    public class GameFactory : IMediaFactory
    {
        public MediaItem CreateMediaItem()
        {
            var game = new Game
            {
                Status = new Patterns.States.PlannedState(),
                StatusType = "PlannedState",
                Title = "Новая игра",
                Year = DateTime.Now.Year,
                Genre = "Не указан",
                Rating = 0,
                Platform = "",
                Developer = "",
                PlayTime = 0
            };
            return game;
        }

        public string GetMediaType() => "Игра";
    }
}